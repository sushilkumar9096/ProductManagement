using System;
using System.Linq;
using System.Threading.Tasks;
using Product.Domain.Entities;
using Product.Domain.Exceptions;
using Product.Application.DTOs;
using Product.Application.Interfaces;

namespace Product.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<TokenResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            var existingUser = await _unitOfWork.Users.GetUserByUsernameWithRefreshTokensAsync(registerRequest.Username);
            if (existingUser != null)
            {
                throw new BadRequestException($"Username '{registerRequest.Username}' is already taken.");
            }

            var user = new User
            {
                Username = registerRequest.Username,
                PasswordHash = _passwordHasher.HashPassword(registerRequest.Password),
                Role = string.IsNullOrWhiteSpace(registerRequest.Role) ? "User" : registerRequest.Role
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            user.RefreshTokens.Add(refreshToken);
            await _unitOfWork.CompleteAsync();

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<TokenResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _unitOfWork.Users.GetUserByUsernameWithRefreshTokensAsync(loginRequest.Username);

            if (user == null || !_passwordHasher.VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                throw new BadRequestException("Invalid username or password.");
            }

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            // Revoke other active tokens for this user
            var activeTokens = user.RefreshTokens.Where(rt => rt.IsActive).ToList();
            foreach (var token in activeTokens)
            {
                token.RevokedOn = DateTime.UtcNow;
            }

            user.RefreshTokens.Add(refreshToken);
            await _unitOfWork.CompleteAsync();

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            var principal = _jwtTokenGenerator.GetPrincipalFromExpiredToken(refreshTokenRequest.AccessToken);
            if (principal == null)
            {
                throw new BadRequestException("Invalid access token or claim structure.");
            }

            var username = principal.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                throw new BadRequestException("Invalid token claims.");
            }

            var user = await _unitOfWork.Users.GetUserByUsernameWithRefreshTokensAsync(username);
            if (user == null)
            {
                throw new BadRequestException("User associated with token not found.");
            }

            var savedRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshTokenRequest.RefreshToken);

            if (savedRefreshToken == null)
            {
                throw new BadRequestException("Refresh token does not exist.");
            }

            // Reuse detection: If the refresh token has already been revoked, it indicates potential token theft!
            if (savedRefreshToken.RevokedOn != null)
            {
                // Immediately revoke all other active refresh tokens for the user to secure the account
                var activeTokens = user.RefreshTokens.Where(rt => rt.IsActive).ToList();
                foreach (var token in activeTokens)
                {
                    token.RevokedOn = DateTime.UtcNow;
                }
                await _unitOfWork.CompleteAsync();
                throw new BadRequestException("Warning: Refresh token has already been used! Revoking all sessions for security.");
            }

            if (savedRefreshToken.IsExpired)
            {
                throw new BadRequestException("Refresh token has expired. Please log in again.");
            }

            var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var newRefreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();

            // Revoke current token (rotation)
            savedRefreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenString,
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            user.RefreshTokens.Add(newRefreshToken);
            await _unitOfWork.CompleteAsync();

            return new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenString,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var user = await _unitOfWork.Users.GetUserByRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                throw new BadRequestException("Invalid token.");
            }

            var foundToken = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
            if (foundToken == null || !foundToken.IsActive)
            {
                throw new BadRequestException("Token is invalid or already revoked.");
            }

            foundToken.RevokedOn = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();
        }
    }
}
