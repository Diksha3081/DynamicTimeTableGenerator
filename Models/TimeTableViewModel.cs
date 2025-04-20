using System.ComponentModel.DataAnnotations;

namespace DynamicTimeTableGenerator.Models
{
    public class TimeTableViewModel
    {
        public string ClassName { get; set; }
        public int WorkingDays { get; set; }
        public int SubjectsPerDay { get; set; }
        public int TotalHours { get; set; }
        public int TotalSubjects { get; set; }
        public List<SubjectHour> Subjects { get; set; } = new List<SubjectHour>();
    }

    public class SubjectHour
    {
        [Required(ErrorMessage = "Subject name is required.")]
        [StringLength(100, ErrorMessage = "Subject name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\s]+$", ErrorMessage = "Subject name can only contain alphanumeric characters and spaces.")]
        public string SubjectName { get; set; }
        public int Hours { get; set; }
    }

}
