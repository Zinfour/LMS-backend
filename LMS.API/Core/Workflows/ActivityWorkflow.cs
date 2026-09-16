using LMS.API.Core.Types;

namespace LMS.API.Core.Workflows
{
    public static class ActivityWorkflow
    {
        public static WorkflowResult<ActivityWrite> ValidateForCreate(ActivityContext ctx)
        {
            return ValidateForCreateAndUpdate(ctx);
        }

        public static WorkflowResult<ActivityWrite> ValidateForUpdate(ActivityContext ctx)
        {
            if(!ctx.Write.ActivityExists)
                return new WorkflowResult<ActivityWrite>.NotFound(
                    $"Activity with ID {ctx.Write.Id} not found.");

            return ValidateForCreateAndUpdate(ctx);
        }

        private static WorkflowResult<ActivityWrite> ValidateForCreateAndUpdate(ActivityContext ctx)
        {
            if (!ctx.ParentModuleExists)
                return new WorkflowResult<ActivityWrite>.NotFound(
                    $"Parent module with ID {ctx.ParentModuleId} not found.");

            if (DateOnly.FromDateTime(ctx.Write.StartTime) < ctx.ModuleStartDate || DateOnly.FromDateTime(ctx.Write.EndTime) > ctx.ModuleEndDate)
                return new WorkflowResult<ActivityWrite>.BadRequest(
                    $"Activity dates must be within the module's date range ({ctx.ModuleStartDate} to {ctx.ModuleEndDate}).");

            if (ctx.Write.StartTime > ctx.Write.EndTime)
                return new WorkflowResult<ActivityWrite>.BadRequest(
                    $"Activity StartDate ({ctx.Write.StartTime}) cannot be after EndDate ({ctx.Write.EndTime}).");

            var overlappingActivity = ctx.ExistingActivities?.FirstOrDefault(a =>
                a.Id != ctx.Write.Id && (
                    (ctx.Write.StartTime >= a.StartTime && ctx.Write.StartTime <= a.EndTime) ||
                    (ctx.Write.EndTime >= a.StartTime && ctx.Write.EndTime <= a.EndTime) ||
                    (ctx.Write.StartTime <= a.StartTime && ctx.Write.EndTime >= a.EndTime)
                ));

            if (overlappingActivity != null)
                return new WorkflowResult<ActivityWrite>.BadRequest(
                    $"Activity dates overlap with existing activity '{overlappingActivity.Name}' ({overlappingActivity.Id}).");

            return Validate(ctx.Write);
        }

        public static WorkflowResult<ActivityWrite> Validate(ActivityWrite write)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(write.Name)) errors.Add("Name is required.");
            if (string.IsNullOrWhiteSpace(write.Description)) errors.Add("Description is required.");
            if (write.StartTime == DateTime.MinValue) errors.Add("StartTime is required.");
            if (write.EndTime == DateTime.MinValue) errors.Add("EndTime is required.");

            return errors.Count == 0
                ? new WorkflowResult<ActivityWrite>.Ok(write)
                : new WorkflowResult<ActivityWrite>.ValidationFailed(errors);
        }
    }
}
