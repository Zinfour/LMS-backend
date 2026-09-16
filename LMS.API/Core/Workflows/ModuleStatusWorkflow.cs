using LMS.API.Core.Types;

namespace LMS.API.Core.Workflows;

public static class ModuleStatusWorkflow
{
    public static ModuleStatus Calculate(
        DateOnly start, DateOnly end,
        int activityCount, int completeActivityCount,
        DateOnly today)
    {
        if (today < start) return ModuleStatus.locked;
        if(start < today && today < end && activityCount == 0) return ModuleStatus.inProgress;
        if (activityCount > 0 && completeActivityCount == activityCount) return ModuleStatus.completed;
        
        return ModuleStatus.overdue;
    }
}