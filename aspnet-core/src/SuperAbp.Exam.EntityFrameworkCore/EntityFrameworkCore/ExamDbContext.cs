﻿using SuperAbp.Exam.QuestionManagement.QuestionAnswers;
using SuperAbp.Exam.QuestionManagement.Questions;
using Microsoft.EntityFrameworkCore;
using SuperAbp.Exam.ExamManagement.Exams;
using SuperAbp.Exam.ExamManagement.UserExamQuestions;
using SuperAbp.Exam.ExamManagement.UserExams;
using SuperAbp.Exam.Favorites;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using SuperAbp.MenuManagement.EntityFrameworkCore;
using SuperAbp.Exam.PaperManagement.Papers;
using SuperAbp.Exam.TrainingManagement;
using SmartEnum.EFCore;
using SuperAbp.Exam.ExamManagement.UserExamQuestionReviews;
using SuperAbp.Exam.PaperManagement.PaperQuestionRules;
using SuperAbp.Exam.QuestionManagement.QuestionBanks;
using SuperAbp.Exam.KnowledgePoints;
using SuperAbp.Exam.QuestionManagement.QuestionKnowledgePoints;
using SuperAbp.Exam.QuestionManagement.Questions.QuestionAnswers;

namespace SuperAbp.Exam.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class ExamDbContext :
    AbpDbContext<ExamDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext, IExamDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }

    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }

    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion Entities from the modules

    public DbSet<Question> Questions { get; set; }
    public DbSet<KnowledgePoint> KnowledgePoints { get; set; }
    public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
    public DbSet<QuestionBank> QuestionBanks { get; set; }
    public DbSet<Paper> Papers { get; set; }

    public DbSet<QuestionKnowledgePoint> QuestionKnowledgePoints { get; set; }
    public DbSet<PaperQuestionRule> PaperQuestionRules { get; set; }

    public DbSet<Examination> Exams { get; set; }

    public DbSet<UserExam> UserExams { get; set; }
    public DbSet<UserExamQuestion> UerExamQuestions { get; set; }
    public DbSet<UserExamQuestionReview> UserExamQuestionReviews { get; set; }

    public DbSet<Training> Trains { get; set; }

    public DbSet<Favorite> Favorites { get; set; }

    public ExamDbContext(DbContextOptions<ExamDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();
        builder.ConfigureMenuManagement();

        builder.Entity<Question>(b =>
        {
            b.ToTable(ExamConsts.DbTablePrefix + "Questions", ExamConsts.DbSchema);
            b.ConfigureByConvention();
            b.ConfigureAuditedAggregateRoot();

            b.Property(p => p.QuestionType).IsRequired();
            b.Property(p => p.Content).IsRequired().HasMaxLength(QuestionConsts.MaxContentLength);
            b.Property(p => p.Analysis).HasMaxLength(QuestionConsts.MaxAnalysisLength);
        });

        builder.Entity<KnowledgePoint>(b =>
        {
            b.ToTable(ExamConsts.DbTablePrefix + "KnowledgePoints", ExamConsts.DbSchema);
            b.ConfigureByConvention();
            b.ConfigureFullAudited();

            b.Property(p => p.Name).IsRequired().HasMaxLength(KnowledgePointConsts.MaxNameLength);
        });

        builder.Entity<QuestionAnswer>(b =>
        {
            b.ToTable(ExamConsts.DbTablePrefix + "QuestionAnswers", ExamConsts.DbSchema);
            b.ConfigureByConvention();
            b.ConfigureAuditedAggregateRoot();

            b.Property(p => p.Content).IsRequired().HasMaxLength(QuestionAnswerConsts.MaxContentLength);
            b.Property(p => p.Analysis).HasMaxLength(QuestionAnswerConsts.MaxAnalysisLength);
            b.Property(p => p.Sort).IsRequired().HasDefaultValue(0);
        });

        builder.Entity<QuestionBank>(b =>
        {
            b.ToTable(ExamConsts.DbTablePrefix + "QuestionBanks", ExamConsts.DbSchema);
            b.ConfigureByConvention();
            b.ConfigureAuditedAggregateRoot();

            b.Property(p => p.Title).IsRequired().HasMaxLength(QuestionBankConsts.MaxTitleLength);
            b.Property(p => p.Remark).HasMaxLength(QuestionBankConsts.MaxRemarkLength);
        });

        builder.Entity<QuestionKnowledgePoint>(b =>
        {
            b.ToTable(ExamConsts.DbTablePrefix + "QuestionKnowledgePoints", ExamConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasKey(qk => new { qk.QuestionId, qk.KnowledgePointId });
            b.HasIndex(qk => new { qk.QuestionId, qk.KnowledgePointId });
        });

        builder.Entity<Paper>(b =>
        {
            b.ToTable(ExamConsts.DbTablePrefix + "Papers", ExamConsts.DbSchema);
            b.ConfigureByConvention();
            b.ConfigureAuditedAggregateRoot();

            b.Property(p => p.Name).IsRequired().HasMaxLength(PaperConsts.MaxNameLength);
            b.Property(p => p.Description).HasMaxLength(PaperConsts.MaxDescriptionLength);
        });

        builder.Entity<PaperQuestionRule>(b =>
        {
            b.ToTable(ExamConsts.DbTablePrefix + "PaperQuestionRules", ExamConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<Examination>(b =>
        {
            b.ToTable(ExamConsts.DbTablePrefix + "Examination", ExamConsts.DbSchema);
            b.ConfigureByConvention();
            b.ConfigureAuditedAggregateRoot();

            b.Property(p => p.Name).IsRequired().HasMaxLength(PaperConsts.MaxNameLength);

            b.HasIndex(p => p.PaperId);
        });

        builder.Entity<UserExam>(b =>
        {
            b.ToTable(ExamConsts.DbTablePrefix + "UserExam", ExamConsts.DbSchema);
            b.ConfigureByConvention();
            b.ConfigureAuditedAggregateRoot();
        });

        builder.Entity<UserExamQuestion>(b =>
        {
            b.ToTable(ExamConsts.DbTablePrefix + "UserExamQuestion", ExamConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(p => p.Answers).HasMaxLength(UserExamQuestionConsts.MaxAnswersLength);
            b.Property(p => p.Reason).HasMaxLength(UserExamQuestionConsts.MaxReasonLength);
        });

        builder.Entity<UserExamQuestionReview>(b =>
        {
            b.ToTable(ExamConsts.DbTablePrefix + "UserExamQuestionReview", ExamConsts.DbSchema);
            b.ConfigureByConvention();
            b.ConfigureFullAudited();

            b.Property(p => p.Reason).HasMaxLength(UserExamQuestionReviewConsts.MaxReasonLength);
        });

        builder.Entity<Training>(b =>
        {
            b.ToTable(ExamConsts.DbTablePrefix + "Training", ExamConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<Favorite>(b =>
        {
            b.ToTable(ExamConsts.DbTablePrefix + "Favorites", ExamConsts.DbSchema);
            b.ConfigureByConvention();
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.ConfigureSmartEnum();
    }
}