FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files and restore dependencies
COPY ["ProductManagement.slnx", "./"]
COPY ["src/Product.Domain/Product.Domain.csproj", "src/Product.Domain/"]
COPY ["src/Product.Application/Product.Application.csproj", "src/Product.Application/"]
COPY ["src/Product.Infrastructure/Product.Infrastructure.csproj", "src/Product.Infrastructure/"]
COPY ["src/Product.Api/Product.Api.csproj", "src/Product.Api/"]
COPY ["tests/Product.Application.Tests/Product.Application.Tests.csproj", "tests/Product.Application.Tests/"]
COPY ["tests/Product.Infrastructure.Tests/Product.Infrastructure.Tests.csproj", "tests/Product.Infrastructure.Tests/"]
COPY ["tests/Product.Api.Tests/Product.Api.Tests.csproj", "tests/Product.Api.Tests/"]

RUN dotnet restore

# Copy the entire source code and build it
COPY . .
RUN dotnet build -c Release

# Publish stage
FROM build AS publish
RUN dotnet publish "src/Product.Api/Product.Api.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "Product.Api.dll"]
