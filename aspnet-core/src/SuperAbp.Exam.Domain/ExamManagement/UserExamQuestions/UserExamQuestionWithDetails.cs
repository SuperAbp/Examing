﻿using System;
using System.Collections.Generic;
using SuperAbp.Exam.QuestionManagement.Questions;

namespace SuperAbp.Exam.ExamManagement.UserExamQuestions;

public class UserExamQuestionWithDetails
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }
    public required QuestionType QuestionType { get; set; }

    /// <summary>
    /// 题干
    /// </summary>
    public required string Question { get; set; }

    public decimal QuestionScore { get; set; }

    public string? Answers { get; set; }

    public List<QuestionAnswer> QuestionAnswers { get; set; } = [];

    public class QuestionAnswer
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        public required string Content { get; set; }
    }
}