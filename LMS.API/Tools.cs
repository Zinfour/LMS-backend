using LMS.API.Models.Resources;
using LMS.API.Models;
using LMS.API.Core.Types;

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

        
        public static ModuleStatus calculateStatus(Module m, string userId)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            if(currentDate < m.StartDate)
            {
                return ModuleStatus.locked;
            } else if(m.StartDate < currentDate && currentDate < m.EndDate)
            {
                return ModuleStatus.inProgress;
            } else if(m.Activities.All(a => a.CompletedUsers.Any(u => u.Id == userId)))
            {
                return ModuleStatus.completed;
            } else
            {
                return ModuleStatus.overdue;
            }
        }
    }

}
