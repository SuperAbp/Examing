﻿using SuperAbp.Exam.Exams;
using Xunit;

namespace SuperAbp.Exam.EntityFrameworkCore.Applications;

[Collection(ExamTestConsts.CollectionDefinitionName)]
public class EfCoreExaminationAppServiceTests : ExaminationAppServiceTests<ExamEntityFrameworkCoreTestModule>
{
}