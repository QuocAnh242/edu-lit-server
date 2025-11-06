using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Application.Features.AssessmentQuestion.DeleteAssessmentQuestion
{
    public class DeleteAssessmentQuestionCommandHandler : ICommandHandler<DeleteAssessmentQuestionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assessmentQuestions:all";

        public DeleteAssessmentQuestionCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<bool>> Handle(DeleteAssessmentQuestionCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var asseQuestEntity = await _unitOfWork.AssessmentQuestionRepository.GetByIdAsync(command.Id);
                if (asseQuestEntity is null)
                {
                    return ObjectResponse<bool>.Response("404", "Assessment Question Not Found", false);
                }

                _unitOfWork.AssessmentQuestionRepository.Remove(asseQuestEntity);
                // Invalidate cache
                await _redisService.RemoveAsync(CacheKey);

                return ObjectResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return ObjectResponse<bool>.FailureResponse(ex);
            }
        }
    }
}
