using Amazon.SimpleSystemsManagement.Model;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.Options;
using Moq;
using SimpleToggle.Sources.AWS;
using static SimpleToggle.Core.Tests.Util;
using SimpleToggle.Core;
using Xunit;
using System.Threading.Tasks;
using FluentAssertions;

namespace SimpleToggle.Tests
{
    public class AWSSourceTests
    {
        public IFeatureService Service { get; }

        public AWSSourceTests()
        {
            var toggles = new OptionsWrapper<FeatureToggles>(new FeatureToggles()
            {
                ["SetFeatureToggle"] = "parameterStore/toggleLocation"
            });

            var mockParameterStore = new Mock<IAmazonSimpleSystemsManagement>();
            _ = mockParameterStore.Setup(m => m.GetParameterAsync(It.Is<GetParameterRequest>(request => request.Name == "parameterStore/toggleLocation"), default))
                                  .ReturnsAsync(new GetParameterResponse() { Parameter = new Parameter() { Value = "true" } });
            Service = new FeatureService<ParameterStoreToggleSource>(new ParameterStoreToggleSource(mockParameterStore.Object, toggles), CreateLogger<FeatureService<ParameterStoreToggleSource>>());
        }

        [Fact]
        public async Task When_Requesting_An_Existing_Toggle_Then_Get_Value()
        {
            var result = await Service.GetToggleValue("SetFeatureToggle");
            _ = result.Should().BeTrue();
        }

        [Fact]
        public async Task When_Requesting_An_NonExisting_Toggle_Then_Return_False()
        {
            var result = await Service.GetToggleValue("FakeFeatureToggle");
            _ = result.Should().BeFalse();
        }
    }
}
