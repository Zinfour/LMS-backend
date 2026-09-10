using LMS.API.Models.Resources;
using LMS.API.DTOs;
using System.Reflection;
using Module = LMS.API.Models.Module;
using Status = LMS.API.DTOs.ModuleDto.Status;
using LMS.API.Models;

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

        
        public static Status calculateStatus(Module m, ApplicationUser user)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            if(currentDate < m.StartDate)
            {
                return Status.locked;
            } else if(m.StartDate < currentDate && currentDate < m.EndDate)
            {
                return Status.inProgress;
            } else if(m.Activities.All(a => a.CompletedUsers.Any(u => u.Id == user.Id)))
            {
                return Status.completed;
            } else
            {
                return Status.overdue;
            }
        }
    }

}
