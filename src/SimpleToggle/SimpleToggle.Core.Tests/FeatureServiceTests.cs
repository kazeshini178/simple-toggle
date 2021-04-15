using System;
using Microsoft.Extensions.Options;
using SimpleToggle.Sources;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.Collections.Generic;
using static SimpleToggle.Core.Tests.Util;
using SimpleToggle.Sources.AWS;
using Amazon.SimpleSystemsManagement;
using Moq;
using Amazon.SimpleSystemsManagement.Model;
using System.Linq.Expressions;

namespace SimpleToggle.Core.Tests
{
    public class FeatureServiceTests
    {
        // Function could get large is all sources get added/mocked here 
        public static IEnumerable<object[]> FeatureServices()
        {
            var toggles = new OptionsWrapper<FeatureToggles>(new FeatureToggles()
            {
                ["SetFeatureToggle"] = "true",
                ["ParameterStoreToggle"] = "parameterStore/toggleLocation"
            });
            var mockParameterStore = new Mock<IAmazonSimpleSystemsManagement>();
            _ = mockParameterStore.Setup(m => m.GetParameterAsync(It.Is<GetParameterRequest>(request => request.Name == "parameterStore/toggleLocation"), default))
                                  .ReturnsAsync(new GetParameterResponse() { Parameter = new Parameter() { Value = "true" } });

            return new List<object[]>()
            {
                new object[] { new FeatureService<IToggleSource>(new InMemoryToggleSource(toggles), CreateLogger<FeatureService<IToggleSource>>()),"SetFeatureToggle" },
                new object[] { new FeatureService<IToggleSource>(new ParameterStoreToggleSource(mockParameterStore.Object, toggles), CreateLogger<FeatureService<IToggleSource>>()), "ParameterStoreToggle" }
            };
        }

        [Theory]
        [MemberData(nameof(FeatureServices))]
        public async Task When_Requesting_An_Existing_Toggle_Then_Get_Value(IFeatureService service, string toggle)
        {
            var result = await service.GetToggleValue(toggle);
            _ = result.Should().BeTrue();
        }

        //[Fact]
        //public async Task When_Requesting_An_Existing_Toggle_Then_Get_Value()
        //{
        //    var result = await Service.GetToggleValue("SetFeatureToggle");
        //    _ = result.Should().BeTrue();
        //}

        [Fact]
        public async Task When_Requesting_An_NonExisting_Toggle_Then_Return_False()
        {
            var toggles = new OptionsWrapper<FeatureToggles>(new FeatureToggles());
            var service = new FeatureService<IToggleSource>(new InMemoryToggleSource(toggles), CreateLogger<FeatureService<IToggleSource>>());

            var result = await service.GetToggleValue("FakeFeatureToggle");
            _ = result.Should().BeFalse();
        }

        [Fact]
        public async Task When_Requesting_An_Toggle_Multiple_Times_Then_Cache_Is_Used()
        {
            Expression<Func<IToggleSource, Task<bool>>> setupMethodCall = m => m.GetToggleValue(It.IsAny<string>());

            var mockSource = new Mock<IToggleSource>();
            mockSource.Setup(setupMethodCall).ReturnsAsync(true).Verifiable();

            var service = new FeatureService<IToggleSource>(mockSource.Object, CreateLogger<FeatureService<IToggleSource>>());

            _ = await service.GetToggleValue("randomToggle");
            _ = await service.GetToggleValue("randomToggle");
            mockSource.Verify(setupMethodCall, Times.Once);
        }
    }
}
