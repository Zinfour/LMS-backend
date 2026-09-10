namespace LMS.API.DTOs;
public class ModuleDto
{
    public int Id {get; set;}
    public string Name {get; set;} = null!;
    public string Description {get; set;} = null!;
    public DateOnly StartDate {get; set;}
    public DateOnly EndDate {get; set;}
    public string? ImageURL {get; set;}
    public int CourseId {get; set;}
    public int ActivitiesNumber {get; set;}
    public int ResourcesNumber {get; set;}
    public int NumberOfCompletedActivities { get; set; }
}