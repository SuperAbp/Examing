﻿using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace SuperAbp.Exam.ExamManagement.UserExamQuestions
{
    /// <summary>
    /// 列表
    /// </summary>
    public class UserExamQuestionListDto : EntityDto<Guid>
    {
        public required string Question { get; set; }

        public Guid QuestionId { get; set; }
        public int QuestionType { get; set; }
        public decimal QuestionScore { get; set; }
        public string? Answers { get; set; }

        /// <summary>
        /// 正确
        /// </summary>
        public bool? Right { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        public decimal? Score { get; set; }

        public string? QuestionAnalysis { get; set; }

        public IReadOnlyList<string> KnowledgePoints { get; set; } = [];
        public IReadOnlyList<QuestionAnswerListDto> QuestionAnswers { get; set; } = [];

        public class QuestionAnswerListDto
        {
            public Guid Id { get; set; }

            /// <summary>
            /// 答案
            /// </summary>
            public required string Content { get; set; }

            public bool? Right { get; set; }
        }
    }
}