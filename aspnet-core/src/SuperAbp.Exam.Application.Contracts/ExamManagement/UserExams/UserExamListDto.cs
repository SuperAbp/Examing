﻿using System;
using Volo.Abp.Application.Dtos;

namespace SuperAbp.Exam.ExamManagement.UserExams
{
    /// <summary>
    /// 列表
    /// </summary>
    public class UserExamListDto : EntityDto<Guid>
    {
        public Guid ExamId { get; set; }

        /// <summary>
        /// 考试名称
        /// </summary>
        public string ExamName { get; set; }

        /// <summary>
        /// 最高分
        /// </summary>
        public decimal TotalScore { get; set; }

        public DateTime? FinishedTime { get; set; }

        public DateTime CreationTime { get; set; }

        public int Status { get; set; }
    }
}