using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveAnalytics
{
    public class HiveServiceTests
    {
        public HiveServiceTests()
        {
            MappingProfile.Initialize();
        }

        [Fact]
        public void Ctor_ContextIsNull_ExceptionThrown()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new HiveService(null, new Mock<IUserContext>().Object));

            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }

        [Fact]
        public void Ctor_UserContextIsNull_ExceptionThrown()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new HiveService(new Mock<IProductStoreHiveContext>().Object, null));

            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }

        [Theory]
        [AutoMoqData]
        public async Task GetHiveAsync_EmptyListReturned([Frozen] Mock<IProductStoreHiveContext> context, HiveService service)
        {
            context.Setup(c => c.Hives).ReturnsEntitySet(new StoreHive[] { });

            var hives = await service.GetHivesAsync();

            hives.Should().BeEmpty();
        }

        [Theory]
        [AutoMoqData]
        public async Task GetHiveAsync_TenItemReturned([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var data = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(c => c.Hives).ReturnsEntitySet(data);
            context.Setup(s => s.Sections).ReturnsEntitySet(new List<StoreHiveSection>());

            var hives = await service.GetHivesAsync();

            hives.Count.Should().Be(data.Count);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetHiveAsync_FirstItemReturn([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hives = fixture.CreateMany<StoreHive>(10).ToList();
            context.Setup(s => s.Hives).ReturnsEntitySet(hives);
            context.Setup(s => s.Sections).ReturnsEntitySet(new List<StoreHiveSection>());

            var hive = await service.GetHiveAsync(hives.First().Id);

            Assert.Equal(hives.First().Id, hive.Id);
        }

        [Theory]
        [AutoMoqData]
        public async Task SetStatusAsync_RequestedResourceNotFoundExceptionThrown([Frozen] Mock<IProductStoreHiveContext> context, HiveService service)
        {
            context.Setup(s => s.Hives).ReturnsEntitySet(new StoreHive[] { });

            var exception = await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.SetStatusAsync(0, false));

            Assert.Equal(typeof(RequestedResourceNotFoundException), exception.GetType());
        }

        [Theory]
        [AutoMoqData]
        public async Task SetStatusAsync_SetDeletedInFirstItem([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hives = fixture.CreateMany<StoreHive>(10).ToList();
            hives.First().IsDeleted = false;
            context.Setup(s => s.Hives).ReturnsEntitySet(hives);
            context.Setup(s => s.Sections).ReturnsEntitySet(new List<StoreHiveSection>());

            await service.SetStatusAsync(hives.First().Id, true);
        }

        [Theory]
        [AutoMoqData]
        public async Task DeleteHiveAsync_RequestedResourceNotFoundExceptionThrown([Frozen] Mock<IProductStoreHiveContext> context, HiveService service)
        {
            context.Setup(s => s.Hives).ReturnsEntitySet(new StoreHive[] { });

            var exception = await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.DeleteHiveAsync(0));

            Assert.Equal(typeof(RequestedResourceNotFoundException), exception.GetType());
        }

        [Theory]
        [AutoMoqData]
        public async Task UpdateHiveAsync_RequestedResourceNotFoundExceptionThrown([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            context.Setup(s => s.Hives).ReturnsEntitySet(new StoreHive[] { });

            var exception = await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.UpdateHiveAsync(0, fixture.Create<UpdateHiveRequest>()));

            Assert.Equal(typeof(RequestedResourceNotFoundException), exception.GetType());
        }
    }
}
