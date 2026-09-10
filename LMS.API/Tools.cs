using LMS.API.Models.Resources;

namespace LMS.API
{
    public static class Tools
    {
        public static ResourceType ParseResourceType(string resourceType)
        {
            return resourceType.ToLower() switch
            {
                "instruction" => ResourceType.Instruction,
                "textmaterial" => ResourceType.TextMaterial,
                "link" => ResourceType.Link,
                "summary" => ResourceType.Summary,
                "reference" => ResourceType.Reference,
                _ => ResourceType.TextMaterial
            };
        }

        public static string ResourceTypeToString(ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Instruction => "Instruction",
                ResourceType.TextMaterial => "TextMaterial",
                ResourceType.Link => "Link",
                ResourceType.Summary => "Summary",
                ResourceType.Reference => "Reference",
                _ => "TextMaterial"
            };
        }
    }
}
