using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Application.Features.AssessmentAnswer.DeleteAssessmentAnswer
{
    public class DeleteAssessmentAnswerCommandHandler : ICommandHandler<DeleteAssessmentAnswerCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assessmentAnswer:all";

        public DeleteAssessmentAnswerCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<bool>> Handle(DeleteAssessmentAnswerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var answerEntity = await _unitOfWork.AssessmentAnswerRepository.GetByIdAsync(command.AnswerId);
                if (answerEntity is null)
                {
                    return ObjectResponse<bool>.Response("404", "Assessment Answer Not Found", false);
                }

                var attemptId = answerEntity.AttemptsId;

                _unitOfWork.AssessmentAnswerRepository.Remove(answerEntity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Invalidate cache
                await _redisService.RemoveAsync(CacheKey);
                await _redisService.RemoveAsync($"assessmentAnswers:attemptId:{attemptId}");
                await _redisService.RemoveAsync($"assessmentAnswer:{command.AnswerId}");
                await _redisService.RemoveAsync($"grading:attempt:{attemptId}");

                return ObjectResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return ObjectResponse<bool>.FailureResponse(ex);
            }
        }
    }
}
