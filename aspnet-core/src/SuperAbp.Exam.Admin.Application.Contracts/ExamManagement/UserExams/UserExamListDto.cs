﻿using System;

namespace SuperAbp.Exam.Admin.ExamManagement.UserExams;

public class UserExamListDto
{
    public Guid Id { get; set; }

    /// <summary>
    /// 总分
    /// </summary>
    public decimal TotalScore { get; set; }

    /// <summary>
    /// 交卷时间
    /// </summary>
    public DateTime? FinishedTime { get; set; }

    public DateTime CreationTime { get; protected set; }

    public int Status { get; set; }
}