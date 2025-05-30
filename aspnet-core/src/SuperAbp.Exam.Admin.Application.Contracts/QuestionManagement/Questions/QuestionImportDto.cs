﻿using SuperAbp.Exam.QuestionManagement.Questions;
using System;

namespace SuperAbp.Exam.Admin.QuestionManagement.Questions;

public class QuestionImportDto
{
    /// <summary>
    /// 所属题库
    /// </summary>
    public Guid QuestionBankId { get; set; }

    public int QuestionType { get; set; }

    public string Content { get; set; }
}