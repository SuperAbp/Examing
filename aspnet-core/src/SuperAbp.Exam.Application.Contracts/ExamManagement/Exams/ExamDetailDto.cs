﻿using System;
using Volo.Abp.Application.Dtos;

namespace SuperAbp.Exam.ExamManagement.Exams
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ExamDetailDto : EntityDto<System.Guid>
    {
        public required string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        public decimal Score { get; set; }
        public decimal PassingScore { get; set; }
        public int TotalTime { get; set; }

        /// <summary>
        /// 试卷Id
        /// </summary>
        public Guid PaperId { get; set; }

        public System.DateTime? StartTime { get; set; }
        public System.DateTime? EndTime { get; set; }
    }
}