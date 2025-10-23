
using AssessmentService.Dao;
using AssessmentService.Dao.DAOs;
using AssessmentService.Repository;
using AssessmentService.Repository.Imp;
using AssessmentService.Service;
using AssessmentService.Service.Imp;
using Microsoft.EntityFrameworkCore;

namespace AssessmentService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register DAOs (scoped)
            builder.Services.AddScoped<AssessmentAnswerDAO>();
            builder.Services.AddScoped<AssessmentQuestionDAO>();
            builder.Services.AddScoped<AssessmentDAO>();
            builder.Services.AddScoped<AssignmentAttemptDAO>();
            builder.Services.AddScoped<GradingFeedbackDAO>();

            // Register repositories and services
            builder.Services.AddScoped<IAssessmentAnswerRepository, AssessmentAnswerRepository>();
            builder.Services.AddScoped<IAssessmentAnswerService, AssessmentAnswerService>();
            builder.Services.AddScoped<IAssessmentQuestionRepository, AssessmentQuestionRepository>();
            builder.Services.AddScoped<IAssessmentQuestionService, AssessmentQuestionService>();
            builder.Services.AddScoped<IAssessmentRepository, AssessmentRepository>();
            builder.Services.AddScoped<IAssessmentService, Service.Imp.AssessmentService>();
            builder.Services.AddScoped<IAssignmentAttemptRepository, AssignmentAttemptRepository>();
            builder.Services.AddScoped<IAssignmentAttemptService, AssignmentAttemptService>();
            builder.Services.AddScoped<IGradingFeedbackRepository, GradingFeedbackRepository>();
            builder.Services.AddScoped<IGradingFeedbackService, GradingFeedbackService>();

            // Register DbContext
            var conn = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AssessmentDbContext>(options =>
                options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
