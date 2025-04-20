using Microsoft.AspNetCore.Mvc;
using DynamicTimeTableGenerator.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DynamicTimeTableGenerator.Controllers
{
    public class TimetableController : Controller
    {
        /// <summary>
        /// Initial screen to input basic timetable info
        /// </summary>
        public IActionResult CreateTimeTable()
        {
            return View();
        }

        /// <summary>
        /// Displays the form to enter subject names and hours
        /// </summary>

        [HttpPost]
        public IActionResult EnterSubjects(string className, int workingDays, int subjectsPerDay, int totalSubjects)
        {
            var model = new TimeTableViewModel
            {
                ClassName = className,
                WorkingDays = workingDays,
                SubjectsPerDay = subjectsPerDay,
                TotalSubjects = totalSubjects,
                Subjects = new List<SubjectHour>(new SubjectHour[totalSubjects]) // initializes empty list of subject-hours
            };

            return View(model);
        }

        /// <summary>
        /// Validates and generates the dynamic timetable
        /// </summary>

        [HttpPost]
        public IActionResult GenerateTimeTable(TimeTableViewModel model)
        {
            int totalRequiredHours = model.WorkingDays * model.SubjectsPerDay;
            int totalEnteredHours = model.Subjects.Sum(x => x.Hours);

            // Check if total entered hours match expected
            if (totalEnteredHours != totalRequiredHours)
            {
                ModelState.AddModelError("", "Total hours must match total weekly hours.");
                return View("EnterSubjects", model);
            }

            // Step 1: Create a pool of subjects with remaining hours
            var subjectPool = model.Subjects.ToDictionary(s => s.SubjectName, s => s.Hours);

            // Step 2: Prepare an empty timetable (one list per working day)
            var timetable = new List<List<string>>();
            for (int i = 0; i < model.WorkingDays; i++)
            {
                timetable.Add(new List<string>());
            }

            Random rand = new Random();

            // Step 3: Fill the timetable with subjects from the pool
            while (subjectPool.Values.Sum() > 0)
            {
                for (int day = 0; day < model.WorkingDays; day++)
                {
                    var usedSubjects = new HashSet<string>(timetable[day]);

                    // Get available subjects for the day (not repeated yet)
                    var availableSubjects = subjectPool
                        .Where(s => s.Value > 0 && !usedSubjects.Contains(s.Key))
                        .Select(s => s.Key)
                        .ToList();

                    // If all subjects are already used once, allow any with remaining hours
                    if (availableSubjects.Count == 0)
                    {
                        availableSubjects = subjectPool
                            .Where(s => s.Value > 0)
                            .Select(s => s.Key)
                            .ToList();
                    }

                    // Fill the day's timetable until the subject limit is reached
                    while (timetable[day].Count < model.SubjectsPerDay && availableSubjects.Any())
                    {
                        string selectedSubject = availableSubjects[rand.Next(availableSubjects.Count)];

                        timetable[day].Add(selectedSubject);
                        subjectPool[selectedSubject]--;

                        // Remove subject from pool if exhausted
                        if (subjectPool[selectedSubject] == 0)
                        {
                            subjectPool.Remove(selectedSubject);
                        }

                        // Refresh available subjects for the next iteration
                        availableSubjects = subjectPool
                            .Where(s => s.Value > 0 && !timetable[day].Contains(s.Key))
                            .Select(s => s.Key)
                            .ToList();
                    }
                }
            }

            return View("DisplayTimeTable", timetable);
        }
    }
}
