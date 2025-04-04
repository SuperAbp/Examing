﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SuperAbp.Exam.ExamManagement.UserExams
{
    /// <summary>
    /// 用户考试
    /// </summary>
    public interface IUserExamRepository : IRepository<UserExam, Guid>
    {
        /// <summary>
        /// 考试是否存在
        /// </summary>
        /// <param name="examId"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> AnyByExamIdAndUserIdAsync(Guid examId, Guid userId, CancellationToken cancellationToken = default);

        Task<bool> UnfinishedExistsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}