using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.Features.AssessmentQuestion.CreateAssessmentQuestion;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Application.Features.AssessmentQuestion.UpdateAssessmentQuestion
{
    public class UpdateAssessmentQuestionCommandHandler : ICommandHandler<UpdateAssessmentQuestionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateAssessmentQuestionCommand> _updateAssessmentCommandValidator;
        private readonly IMapper _mapper;
        public UpdateAssessmentQuestionCommandHandler(IUnitOfWork unitOfWork, IValidator<UpdateAssessmentQuestionCommand> updateAssessmentCommandValidator, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _updateAssessmentCommandValidator = updateAssessmentCommandValidator;
            _mapper = mapper;
        }
        public async Task<ObjectResponse<bool>> Handle(UpdateAssessmentQuestionCommand request, CancellationToken cancellationToken)
        {
            // validation
            var validationResult = await _updateAssessmentCommandValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("AssessmentQuestion.Update.Validation", e.ErrorMessage))
                    .ToList();
                return ObjectResponse<bool>.Response("400", errors.First().Message, false);
            }

            try
            {
                var existingAssessmentQuestion = await _unitOfWork.AssessmentQuestionRepository.GetByIdAsync(request.AssessmentQuestionId);
                if (existingAssessmentQuestion == null)
                {
                    return ObjectResponse<bool>.Response("404", "AssessmentQuestion not found", false);
                }
                // Map updated fields
                _mapper.Map(request, existingAssessmentQuestion);
                _unitOfWork.AssessmentQuestionRepository.Update(existingAssessmentQuestion);
                await _unitOfWork.SaveChangesAsync();
                return ObjectResponse<bool>.SuccessResponse(true);
            }
            catch (Exception e)
            {
                return ObjectResponse<bool>.Response("400", e.Message, false);
            }
        }
    }
}
