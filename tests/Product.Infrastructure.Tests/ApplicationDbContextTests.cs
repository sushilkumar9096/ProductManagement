using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Product.Application.Interfaces;
using Product.Domain.Entities;
using Product.Infrastructure.Data;
using Xunit;

namespace Product.Infrastructure.Tests
{
    public class ApplicationDbContextTests
    {
        [Fact]
        public async Task SaveChangesAsync_OnAdded_PopulatesCreatedAuditFields()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockCurrentUserService = new Mock<ICurrentUserService>();
            mockCurrentUserService.Setup(s => s.Username).Returns("testuser");

            using var context = new ApplicationDbContext(options, mockCurrentUserService.Object);
            var product = new Domain.Entities.Product { ProductName = "Audited Product" };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            Assert.Equal("testuser", product.CreatedBy);
            Assert.True((DateTime.UtcNow - product.CreatedOn).TotalSeconds < 5);
            Assert.Null(product.ModifiedBy);
            Assert.Null(product.ModifiedOn);
        }

        [Fact]
        public async Task SaveChangesAsync_OnModified_PopulatesModifiedAuditFields()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockCurrentUserService = new Mock<ICurrentUserService>();
            mockCurrentUserService.Setup(s => s.Username).Returns("testuser");

            using (var context = new ApplicationDbContext(options, mockCurrentUserService.Object))
            {
                var product = new Domain.Entities.Product { ProductName = "Audited Product" };
                context.Products.Add(product);
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options, mockCurrentUserService.Object))
            {
                var product = await context.Products.FirstAsync();
                product.ProductName = "Updated Name";
                await context.SaveChangesAsync();

                Assert.Equal("testuser", product.CreatedBy);
                Assert.Equal("testuser", product.ModifiedBy);
                Assert.NotNull(product.ModifiedOn);
                Assert.True((DateTime.UtcNow - product.ModifiedOn.Value).TotalSeconds < 5);
            }
        }
    }
}
