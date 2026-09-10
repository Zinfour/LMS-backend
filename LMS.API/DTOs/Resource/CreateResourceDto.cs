namespace LMS.API.DTOs.Resource
{
    public class CreateResourceDto
    {
        public string? URL { get; set; }
        public string ResourceType { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
