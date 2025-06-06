﻿using FluentValidation;
using Microsoft.Extensions.Localization;
using SuperAbp.Exam.Localization;

namespace SuperAbp.Exam.Admin.QuestionManagement.QuestionBanks;

public class QuestionBankCreateOrUpdateDtoBaseValidator : AbstractValidator<QuestionBankCreateOrUpdateDtoBase>
{
    public QuestionBankCreateOrUpdateDtoBaseValidator(IStringLocalizer<ExamResource> local)
    {
        RuleFor(q => q.Title)
            .NotNull()
            .NotEmpty()
            .WithMessage(local["The {0} field is required.", "{PropertyName}"]);
    }
}