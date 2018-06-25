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

    public class HiveSectionAutoFixtureTests
    {
        public HiveSectionAutoFixtureTests()
        {
            MappingProfile.Initialize();
        }

        [Fact]
        public void Ctor_ContextIsNull_ExceptionThrown()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new HiveSectionService(null, new Mock<IUserContext>().Object));

            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }

        [Fact]
        public void Ctor_UserContextIsNull_ExceptionThrown()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new HiveSectionService(new Mock<IProductStoreHiveContext>().Object, null));

            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }

        [Theory]
        [AutoMoqData]
        public async Task GetHiveSectionAsync_EmptyListReturned([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service)
        {
            context.Setup(c => c.Sections).ReturnsEntitySet(new StoreHiveSection[] { });

            var sections = await service.GetHiveSectionsAsync();

            sections.Should().BeEmpty();
        }

        [Theory]
        [AutoMoqData]
        public async Task GetHiveSectionAsync_TenItemReturned([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var data = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(c => c.Sections).ReturnsEntitySet(data);

            var sections = await service.GetHiveSectionsAsync();

            sections.Count.Should().Be(data.Count);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetHiveSectionAsync_RequestedResourceNotFoundExceptionThrown([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service)
        {
            context.Setup(s => s.Sections).ReturnsEntitySet(new StoreHiveSection[] { });

            var exception = await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.GetHiveSectionAsync(0));

            Assert.Equal(typeof(RequestedResourceNotFoundException), exception.GetType());
        }

        [Theory]
        [AutoMoqData]
        public async Task GetHiveSectionAsync_FirstItemReturn([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var sections = fixture.CreateMany<StoreHiveSection>(10).ToList();
            context.Setup(s => s.Sections).ReturnsEntitySet(sections);

            var section = await service.GetHiveSectionAsync(sections.First().Id);

            Assert.Equal(sections.First().Id, section.Id);
        }

        [Theory]
        [AutoMoqData]
        public async Task SetStatusAsync_RequestedResourceNotFoundExceptionThrown([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service)
        {
            context.Setup(s => s.Sections).ReturnsEntitySet(new StoreHiveSection[] { });

            var exception = await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.SetStatusAsync(0, false));

            Assert.Equal(typeof(RequestedResourceNotFoundException), exception.GetType());
        }

        [Theory]
        [AutoMoqData]
        public async Task SetStatusAsync_SetDeletedInFirstItem([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var sections = fixture.CreateMany<StoreHiveSection>(10).ToList();
            sections.First().IsDeleted = false;
            context.Setup(s => s.Sections).ReturnsEntitySet(sections);

            await service.SetStatusAsync(sections.First().Id, true);

            Assert.True(sections.First().IsDeleted);
        }

        [Theory]
        [AutoMoqData]
        public async Task DeleteHiveSectionAsync_RequestedResourceNotFoundExceptionThrown([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service)
        {
            context.Setup(s => s.Sections).ReturnsEntitySet(new StoreHiveSection[] { });

            var exception = await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.DeleteHiveSectionAsync(0));

            Assert.Equal(typeof(RequestedResourceNotFoundException), exception.GetType());
        }

        [Theory]
        [AutoMoqData]
        public async Task UpdateHiveSectionAsync_RequestedResourceNotFoundExceptionThrown([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        {
            context.Setup(s => s.Sections).ReturnsEntitySet(new StoreHiveSection[] { });

            var exception = await Assert.ThrowsAsync<RequestedResourceNotFoundException>(
                () => service.UpdateHiveSectionAsync(0, fixture.Create<UpdateHiveSectionRequest>()));

            Assert.Equal(typeof(RequestedResourceNotFoundException), exception.GetType());
        }

        //[Theory]
        //[AutoMoqData]
        //public async Task CreateHiveSectionAsync_AddSectionToHiveSection([Frozen] Mock<IProductStoreHiveContext> context, HiveSectionService service, IFixture fixture)
        //{
        //    fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        //    var hives = fixture.CreateMany<StoreHive>(2).ToList();
        //    context.Setup(s => s.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
        //    context.Setup(s => s.Hives).ReturnsEntitySet(hives);
        //    var countSection = hives.First().Sections.Count;

        //    await service.CreateHiveSectionAsync(hives.First().Id, fixture.Create<UpdateHiveSectionRequest>());

        //    Assert.Equal(countSection, hives.First().Sections.Count);
        //}
    }
}
