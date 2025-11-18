using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonService.Application.Features.Syllabus.CreateSyllabus
{
    public class CreateSyllabusCommandValidator : AbstractValidator<CreateSyllabusCommand>
    {
        public CreateSyllabusCommandValidator()
        { //e.g validation rules
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MinimumLength(5).WithMessage("Title must be at least 5 characters.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
            RuleFor(x => x.AcademicYear)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Academic year is required.")
                // Sử dụng .Matches() để kiểm tra định dạng
                .Matches(@"^\d{4}-\d{4}$").WithMessage("Academic year must be in the format 'YYYY-YYYY', for example '2010-2012'.")
                .Must(BeAValidYearRange).WithMessage("Academic year must be in the format 'YYYY - YYYY', for example 'currentYear-currentYear + 5'.");
            RuleFor(x => x.Semester)
                .NotNull().WithMessage("Semester is required.")
                .IsInEnum().WithMessage("Semester is required and must be one of the allowed values.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Author is required.");
        }
        
        private bool BeAValidYearRange(string academicYear)
        {
            var years = academicYear.Split("-");
            if (int.TryParse(years[0], out int startYear) && int.TryParse(years[1], out int endYear))
            {
                // Check 1: Năm kết thúc phải lớn hơn năm bắt đầu
                if (endYear <= startYear)
                {
                    return false;
                }

                // Check 2: Giới hạn năm hợp lệ (ví dụ: từ năm 2000 đến 5 năm tới)
                int currentYear = DateTime.UtcNow.Year; // Lấy năm hiện tại
                if (startYear < 2000 || endYear > currentYear + 5)
                {
                    return false;
                }

                return true;
            }
            return false;
        }
    }
}
