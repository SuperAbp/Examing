﻿using SuperAbp.Exam.Exams;
using Xunit;

namespace SuperAbp.Exam.EntityFrameworkCore.Applications;

[Collection(ExamTestConsts.CollectionDefinitionName)]
public class UserExamAdminAppServiceTests : UserExamAdminAppServiceTests<ExamEntityFrameworkCoreTestModule>
{
}