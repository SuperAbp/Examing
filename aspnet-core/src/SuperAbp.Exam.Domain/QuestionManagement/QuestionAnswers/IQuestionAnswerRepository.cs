﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SuperAbp.Exam.QuestionManagement.QuestionAnswers
{
    /// <summary>
    /// 答案
    /// </summary>
    public interface IQuestionAnswerRepository : IRepository<QuestionAnswer, Guid>
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="questionId">问题Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<QuestionAnswer>> GetListAsync(Guid questionId, CancellationToken cancellationToken = default);

        Task<bool> ContentExistsAsync(Guid questionId, string content, CancellationToken cancellationToken = default);

        Task DeleteByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default);
    }
}