﻿using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace SuperAbp.Exam.Admin.QuestionManagement.Questions;

/// <summary>
/// 列表
/// </summary>
public class QuestionListDto : EntityDto<Guid>
{
    /// <summary>
    /// 题库
    /// </summary>
    public required string QuestionBank { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    public IReadOnlyList<string>? KnowledgePoints { get; set; }

    public int QuestionType { get; set; }

    /// <summary>
    /// 题干
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// 解析
    /// </summary>
    public string? Analysis { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }
}