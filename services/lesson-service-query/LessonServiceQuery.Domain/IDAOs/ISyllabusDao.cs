﻿using LessonServiceQuery.Domain.Entities;
namespace LessonServiceQuery.Domain.IDAOs;
public interface ISyllabusDao
{
    Task<Syllabus?> GetByIdAsync(Guid syllabusId);
    Task<List<Syllabus>> GetAllAsync();
    Task<List<Syllabus>> GetByCreatorIdAsync(Guid creatorId);
    Task<List<Syllabus>> GetBySubjectAsync(string subject);
    Task<List<Syllabus>> GetByGradeLevelAsync(string gradeLevel);
    Task<List<Syllabus>> GetByStatusAsync(string status);
    Task<Syllabus> CreateAsync(Syllabus syllabus);
    Task<Syllabus> UpdateAsync(Syllabus syllabus);
    Task DeleteAsync(Guid syllabusId);
    Task<bool> ExistsAsync(Guid syllabusId);
    Task DeactivateAllByIdAsync(Guid syllabusId);
}
