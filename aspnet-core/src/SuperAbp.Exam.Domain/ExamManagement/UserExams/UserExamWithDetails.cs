﻿using SuperAbp.Exam.QuestionManagement.Questions;
using SuperAbp.Exam.QuestionManagement.Questions.QuestionAnswers;
using System;
using System.Collections.Generic;

namespace SuperAbp.Exam.ExamManagement.UserExams
{
    public class UserExamWithDetails
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }

        public Guid UserId { get; set; }
        public string ExamName { get; set; }

        // public Guid QuestionId { get; set; }
        // public required QuestionType QuestionType { get; set; }
        //
        // /// <summary>
        // /// 题干
        // /// </summary>
        // public required string Question { get; set; }
        //
        // /// <summary>
        // /// 解析
        // /// </summary>
        // public string? QuestionAnalysis { get; set; }
        //
        // public decimal QuestionScore { get; set; }

        public string? Answers { get; set; }

        /// <summary>
        /// 总分
        /// </summary>
        public decimal TotalScore { get; set; }

        public DateTime CreationTime { get; set; }

        public UserExamStatus Status { get; set; }

        /// <summary>
        /// 正确
        /// </summary>
        public bool? Right { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        public decimal? Score { get; set; }

        public DateTime? FinishedTime { get; set; }

        public Question Question { get; set; }

        public List<QuestionAnswer> QuestionAnswers { get; set; } = [];
    }
}