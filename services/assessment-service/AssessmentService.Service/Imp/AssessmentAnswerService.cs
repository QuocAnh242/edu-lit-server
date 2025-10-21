using AssessmentService.Database.Models;
using AssessmentService.Database.Models.DTOs;
using AssessmentService.Repository;
using System.Text.RegularExpressions;

namespace AssessmentService.Service.Imp
{
    public class AssessmentAnswerService : IAssessmentAnswerService
    {
        private readonly IAssessmentAnswerRepository repo;
        private readonly IAssessmentQuestionRepository questrepo;

        public async Task<ObjectResponse<AssessmentAnswer>> CreateAsync(AssessmentAnswerDTO assedto)
        {
            try
            {
                if (assedto == null)
                {
                    return ObjectResponse<AssessmentAnswer>.Response("400", "Invalid input: AssessmentAnswerDTO is null", null);
                }

                // validate SelectedAnswer chỉ chọn A, B, C, D
                var selected = (assedto.SelectedAnswer ?? string.Empty).Trim().ToUpperInvariant();
                if (!Regex.IsMatch(selected, "^[ABCD]$"))
                    return ObjectResponse<AssessmentAnswer>.FailureResponse(new ArgumentException("SelectedAnswer must be one of 'A', 'B', 'C' or 'D'."));

                AssessmentAnswer answer = new AssessmentAnswer
                {
                    AssessmentQuestionId = assedto.AssessmentQuestionId,
                    AttemptsId = assedto.AttemptsId,
                    SelectedAnswer = assedto.SelectedAnswer
                };

                // kt kết quả đúng sai
                AssessmentQuestion quest = await questrepo.GetAssessmentQuestionByIdAsync(assedto.AssessmentQuestionId);

                if ( quest.CorrectAnswer == assedto.SelectedAnswer)
                {
                    answer.IsCorrect = true;
                }
                else
                {
                    answer.IsCorrect = false;
                }

                await repo.CreateAssessmentAnswerAsync(answer);
                return ObjectResponse<AssessmentAnswer>.SuccessResponse(answer);
            }
            catch (Exception ex)
            {
                return ObjectResponse<AssessmentAnswer>.FailureResponse(ex);
            }
        }

        public async Task<ObjectResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                bool deleted = await repo.DeleteAssessmentAnswerAsync(id);
                return ObjectResponse<bool>.SuccessResponse(deleted);
            }
            catch (Exception ex)
            {
                return ObjectResponse<bool>.FailureResponse(ex);
            }
        }

        public async Task<ObjectResponse<IEnumerable<AssessmentAnswer>>> GetAllAsync()
        {   try
            {
                IEnumerable<AssessmentAnswer> answers = await repo.GetAllAssessmentAnswerAsync();
                return ObjectResponse<IEnumerable<AssessmentAnswer>>.SuccessResponse(answers);
            }
            catch (Exception ex)
            {
                return ObjectResponse<IEnumerable<AssessmentAnswer>>.FailureResponse(ex);
            }
        }

        public async Task<ObjectResponse<AssessmentAnswer>> GetByIdAsync(int id)
        {
            try
            {
                AssessmentAnswer answer = await repo.GetAssessmentAnswerByIdAsync(id);
                if (answer == null)
                {
                    return ObjectResponse<AssessmentAnswer>.Response("404", "AssessmentAnswer not found", null);
                }
                return ObjectResponse<AssessmentAnswer>.SuccessResponse(answer);
            }
            catch (Exception ex)
            {
                return ObjectResponse<AssessmentAnswer>.FailureResponse(ex);
            }
        }

        public async Task<ObjectResponse<bool>> UpdateAsync(AssessmentAnswerDTO assedto)
        {
            try
            {
                if (assedto == null)
                {
                    return ObjectResponse<bool>.Response("400", "Invalid input: AssessmentAnswerDTO is null", false);
                }

                // validate SelectedAnswer chỉ chọn A, B, C, D
                var selected = (assedto.SelectedAnswer ?? string.Empty).Trim().ToUpperInvariant();
                if (!Regex.IsMatch(selected, "^[ABCD]$"))
                    return ObjectResponse<bool>.FailureResponse(new ArgumentException("SelectedAnswer must be one of 'A', 'B', 'C' or 'D'."));
                
                AssessmentAnswer existingAnswer = await repo.GetAssessmentAnswerByIdAsync(assedto.AssessmentQuestionId);
                if (existingAnswer == null)
                {
                    return ObjectResponse<bool>.Response("404", "AssessmentAnswer not found", false);
                }
                existingAnswer.SelectedAnswer = assedto.SelectedAnswer;

                // kt kết quả đúng sai
                AssessmentQuestion quest = await questrepo.GetAssessmentQuestionByIdAsync(assedto.AssessmentQuestionId);
                if (quest.CorrectAnswer == assedto.SelectedAnswer)
                {
                    existingAnswer.IsCorrect = true;
                }
                else
                {
                    existingAnswer.IsCorrect = false;
                }
                bool updated = await repo.UpdateAssessmentAnswerAsync(existingAnswer);
                return ObjectResponse<bool>.SuccessResponse(updated);
            }
            catch (Exception ex)
            {
                return ObjectResponse<bool>.FailureResponse(ex);
            }
        }
    }
}
