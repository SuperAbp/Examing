﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using System;
using SuperAbp.Exam.Admin.PaperManagement.Papers;

namespace SuperAbp.Exam.Admin.Controllers
{
    /// <summary>
    /// 考试
    /// </summary>
    [Route("api/paper")]
    public class PaperController : ExamController, IPaperAppService
    {
        private readonly IPaperAppService _examingAppService;

        public PaperController(IPaperAppService examingAppService)
        {
            _examingAppService = examingAppService;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        [HttpGet]
        public virtual async Task<PagedResultDto<PaperListDto>> GetListAsync(GetPapersInput input)
        {
            return await _examingAppService.GetListAsync(input);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("{id}/editor")]
        public virtual async Task<GetPaperForEditorOutput> GetEditorAsync(Guid id)
        {
            return await _examingAppService.GetEditorAsync(id);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<PaperListDto> CreateAsync(PaperCreateDto input)
        {
            return await _examingAppService.CreateAsync(input);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public virtual async Task<PaperListDto> UpdateAsync(Guid id, PaperUpdateDto input)
        {
            return await _examingAppService.UpdateAsync(id, input);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _examingAppService.DeleteAsync(id);
        }
    }
}