﻿using System;
using System.Diagnostics.CodeAnalysis;
using SuperAbp.Exam.QuestionManagement.Questions;
using Volo.Abp.Domain.Entities.Auditing;

namespace SuperAbp.Exam.QuestionManagement.QuestionAnswers;

/// <summary>
/// 答案
/// </summary>
public class QuestionAnswer : FullAuditedEntity<Guid>
{
    protected QuestionAnswer()
    { Content = String.Empty; }

    [SetsRequiredMembers]
    protected internal QuestionAnswer(Guid id, Guid questionId, string content, bool right) : base(id)
    {
        Right = right;
        Content = content;
        QuestionId = questionId;
    }

    /// <summary>
    /// 是否正确
    /// </summary>
    public bool Right { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Content { get; internal set; }

    /// <summary>
    /// 解析
    /// </summary>
    public string? Analysis { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    public Guid QuestionId { get; set; }
    public Question Question { get; set; }
}