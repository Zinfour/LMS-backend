using LMS.API.Core.Types;
using LMS.API.Core.Workflows;

namespace LMS.API.Test.Workflows
{
    public class ResourceWorkflowTests
    {
        [Fact]
        public void Validate_ReturnsOk_WhenValid()
        {
            var write = new ResourceWrite(
                URL: null,
                ResourceType: "link",
                Name: "My resource",
                Description: "desc");

            var result = ResourceWorkflow.Validate(write);

            var ok = Assert.IsType<WorkflowResult<ResourceWrite>.Ok>(result);
            Assert.Equal(write, ok.Value);
        }

        [Fact]
        public void Validate_ReturnsValidationFailed_WhenNameMissing()
        {
            var write = new ResourceWrite(
                URL: null,
                ResourceType: "video",
                Name: "   ",
                Description: "desc");

            var result = ResourceWorkflow.Validate(write);

            var vf = Assert.IsType<WorkflowResult<ResourceWrite>.ValidationFailed>(result);
            Assert.Single(vf.Errors);
            Assert.Contains("Name is required.", vf.Errors);
        }

        [Fact]
        public void Validate_ReturnsValidationFailed_WhenResourceTypeMissing()
        {
            var write = new ResourceWrite(
                URL: null,
                ResourceType: " ",
                Name: "Valid name",
                Description: "desc");

            var result = ResourceWorkflow.Validate(write);

            var vf = Assert.IsType<WorkflowResult<ResourceWrite>.ValidationFailed>(result);
            Assert.Single(vf.Errors);
            Assert.Contains("ResourceType is required.", vf.Errors);
        }

        [Fact]
        public void Validate_ReturnsValidationFailed_WithBothErrors_WhenBothMissing()
        {
            var write = new ResourceWrite(
                URL: null,
                ResourceType: " ",
                Name: "",
                Description: "desc");

            var result = ResourceWorkflow.Validate(write);

            var vf = Assert.IsType<WorkflowResult<ResourceWrite>.ValidationFailed>(result);
            Assert.Equal(2, vf.Errors.Count);
            Assert.Equal("Name is required.", vf.Errors[0]);
            Assert.Equal("ResourceType is required.", vf.Errors[1]);
        }
    }
}