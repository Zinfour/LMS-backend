using System;
using LMS.API.Core.Workflows;
using LMS.API.Core.Types;
using Xunit;

namespace LMS.API.Test.Workflows
{
    public class ModuleStatusWorkflowTests
    {
        [Fact]
        public void Calculate_ReturnsLocked_WhenTodayIsBeforeStart()
        {
            var start = new DateOnly(2026, 9, 20);
            var end = new DateOnly(2026, 9, 30);
            var today = new DateOnly(2026, 9, 19);

            var result = ModuleStatusWorkflow.Calculate(start, end, activityCount: 0, completeActivityCount: 0, today);

            Assert.Equal(ModuleStatus.locked, result);
        }

        [Fact]
        public void Calculate_ReturnsInProgress_WhenBetweenStartAndEnd_AndNoActivities()
        {
            var start = new DateOnly(2026, 9, 1);
            var end = new DateOnly(2026, 9, 30);
            var today = new DateOnly(2026, 9, 15);

            var result = ModuleStatusWorkflow.Calculate(start, end, activityCount: 0, completeActivityCount: 0, today);

            Assert.Equal(ModuleStatus.inProgress, result);
        }

        [Fact]
        public void Calculate_ReturnsCompleted_WhenAllActivitiesAreCompleted()
        {
            var start = new DateOnly(2026, 9, 1);
            var end = new DateOnly(2026, 9, 30);
            var today = new DateOnly(2026, 9, 15);

            var result = ModuleStatusWorkflow.Calculate(start, end, activityCount: 5, completeActivityCount: 5, today);

            Assert.Equal(ModuleStatus.completed, result);
        }

        [Fact]
        public void Calculate_ReturnsOverdue_WhenIncompleteAndNotInProgressOrLocked()
        {
            var start = new DateOnly(2026, 8, 1);
            var end = new DateOnly(2026, 8, 31);
            var today = new DateOnly(2026, 9, 1);

            var result = ModuleStatusWorkflow.Calculate(start, end, activityCount: 3, completeActivityCount: 1, today);

            Assert.Equal(ModuleStatus.overdue, result);
        }

        [Fact]
        public void Calculate_ReturnsOverdue_WhenTodayEqualsStart_AndNoActivities()
        {
            var start = new DateOnly(2026, 9, 15);
            var end = new DateOnly(2026, 9, 30);
            var today = new DateOnly(2026, 9, 15);

            var result = ModuleStatusWorkflow.Calculate(start, end, activityCount: 0, completeActivityCount: 0, today);

            Assert.Equal(ModuleStatus.overdue, result);
        }
    }
}