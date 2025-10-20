using LessonService.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonService.Application.Features.Syllabus.Create
{
    public class CreateCommand : ICommand<Guid>
    {
        //e.g properties
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }

    }
}
