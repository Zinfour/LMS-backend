using LMS.API.Core.Types;

namespace LMS.API.Core.Workflows;

public static class SubmissionWorkflow
{
    public static WorkflowResult<NewSubmission> Prepare(
        CallerContext caller,
        SubmissionWrite write,
        AssignmentMeta? assignment,
        DateTime nowUtc)
    {
        if (!caller.IsStudent)
            return new WorkflowResult<NewSubmission>.Forbidden("Only students can submit.");

        if (assignment is null)
            return new WorkflowResult<NewSubmission>.NotFound(
                $"Assignment with ID {write.AssignmentId} not found.");

        if (string.IsNullOrWhiteSpace(write.Text))
            return new WorkflowResult<NewSubmission>.ValidationFailed(["Text is required."]);

        return new WorkflowResult<NewSubmission>.Ok(new NewSubmission(
            Text: write.Text,
            StudentId: caller.UserId,
            AssignmentId: write.AssignmentId,
            Overdue: nowUtc > assignment.Deadline,
            CreatedAt: nowUtc));
    }
}