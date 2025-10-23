using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonService.Application.Features.Syllabus.CreateSyllabus
{
    public class CreateSyllabusCommand : ICommand<Guid>
    {
        //e.g properties
        public string Title { get; set; }
        public string AcademicYear { get; set; }
        public Semester Semester { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
    }
}
