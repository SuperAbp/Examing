﻿using SuperAbp.Exam.Questions;
using Xunit;

namespace SuperAbp.Exam.EntityFrameworkCore.Applications;

[Collection(ExamTestConsts.CollectionDefinitionName)]
public class EfCoreQuestionBankAdminAppServiceTests : QuestionBankAdminAppServiceTests<ExamEntityFrameworkCoreTestModule>
{
}