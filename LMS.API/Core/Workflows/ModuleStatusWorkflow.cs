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
        if (today > end) return ModuleStatus.overdue;
        if (activityCount > 0 && completeActivityCount == activityCount) return ModuleStatus.completed;
        // if (start < today && today < end) return ModuleStatus.inProgress;
        
        return ModuleStatus.inProgress;
    }
}