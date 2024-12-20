﻿using SuperAbp.Exam.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SuperAbp.Exam.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ExamEntityFrameworkCoreModule),
    typeof(ExamApplicationContractsModule)
    )]
public class ExamDbMigratorModule : AbpModule
{
}
