using AssessmentService.Domain.Entities;
using AssessmentService.Domain.Interfaces;
using AssessmentService.Infrastructure.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Infrastructure.Repositories
{
    public class AssignmentAttemptRepository : IAssignmentAttemptRepository
    {
        private readonly AssignmentAttemptDAO _assignmentAttemptDAO;
        public AssignmentAttemptRepository(AssignmentAttemptDAO assignmentAttemptDAO)
        {
            _assignmentAttemptDAO = assignmentAttemptDAO;
        }
        public async Task CreateAssignmentAttemptAsync(AssignmentAttempt assessment) => await _assignmentAttemptDAO.AddAssignmentAttemptAsync(assessment);

        public async Task<bool> DeleteAssignmentAttemptAsync(int id)
        {
            try
            {
                await _assignmentAttemptDAO.DeleteAssignmentAttemptAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<AssignmentAttempt>> GetAllAssignmentAttemptAsync() => await _assignmentAttemptDAO.GetAllAssignmentAttemptsAsync();

        public async Task<AssignmentAttempt> GetAssignmentAttemptByIdAsync(int id) => await _assignmentAttemptDAO.GetAssignmentAttemptByIdAsync(id);

        public async Task<bool> UpdateAssignmentAttemptAsync(AssignmentAttempt assessment)
        {
            try
            {
                await _assignmentAttemptDAO.UpdateAssignmentAttemptAsync(assessment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
