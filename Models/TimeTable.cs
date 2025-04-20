using System.ComponentModel.DataAnnotations;

namespace DynamicTimeTableGenerator.Models
{
    public class TimeTable
    {
        [Required(ErrorMessage = "Class name is required.")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "Class name cannot be only numbers.")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Working days are required.")]
        [Range(1, 7, ErrorMessage = "Working days must be between 1 and 7.")]
        public int WorkingDays { get; set; }

        [Required(ErrorMessage = "Subjects per day are required.")]
        [Range(1, 8, ErrorMessage = "Subjects per day must be between 1 and 8.")]
        public int SubjectsPerDay { get; set; }

        [Required(ErrorMessage = "Total subjects are required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Total subjects must be a positive number.")]
        public int TotalSubjects { get; set; }
    }

}
