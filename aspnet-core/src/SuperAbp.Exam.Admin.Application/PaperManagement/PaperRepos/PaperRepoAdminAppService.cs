﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SuperAbp.Exam.PaperManagement.PaperRepos;
using SuperAbp.Exam.PaperManagement.Papers;
using SuperAbp.Exam.Permissions;
using SuperAbp.Exam.QuestionManagement.QuestionRepos;
using Volo.Abp.Application.Dtos;

namespace SuperAbp.Exam.Admin.PaperManagement.PaperRepos
{
    [Authorize(ExamPermissions.PaperRepos.Default)]
    public class PaperRepoAdminAppService(
        IPaperRepository paperRepository,
        IPaperRepoRepository paperRepoRepository,
        IQuestionRepoRepository questionRepoRepository)
        : ExamAppService, IPaperRepoAdminAppService
    {
        public virtual async Task<PagedResultDto<PaperRepoListDto>> GetListAsync(GetPaperReposInput input)
        {
            await NormalizeMaxResultCountAsync(input);

            var examRepoQueryable = await paperRepoRepository.GetQueryableAsync();

            examRepoQueryable = examRepoQueryable.Where(e => e.PaperId == input.PaperId);

            var queryable =
                from er in examRepoQueryable
                join qr in (await questionRepoRepository.GetQueryableAsync()) on er.QuestionRepositoryId equals qr.Id
                select new PaperRepositoryWithDetails
                {
                    Id = er.Id,
                    QuestionRepository = qr.Title,
                    QuestionRepositoryId = er.QuestionRepositoryId,
                    SingleCount = er.SingleCount,
                    SingleScore = er.SingleScore,
                    MultiCount = er.MultiCount,
                    MultiScore = er.MultiScore,
                    JudgeCount = er.JudgeCount,
                    JudgeScore = er.JudgeScore,
                    BlankCount = er.BlankCount,
                    BlankScore = er.BlankScore,
                    CreationTime = er.CreationTime
                };

            long totalCount = await AsyncExecuter.CountAsync(queryable);

            var entities = await AsyncExecuter.ToListAsync(queryable
                .OrderBy(input.Sorting ?? PaperRepoConsts.DefaultSorting)
                .PageBy(input));

            var dtos = ObjectMapper.Map<List<PaperRepositoryWithDetails>, List<PaperRepoListDto>>(entities);

            return new PagedResultDto<PaperRepoListDto>(totalCount, dtos);
        }

        public virtual async Task<GetPaperRepoForEditorOutput> GetEditorAsync(Guid id)
        {
            PaperRepo entity = await paperRepoRepository.GetAsync(id);

            return ObjectMapper.Map<PaperRepo, GetPaperRepoForEditorOutput>(entity);
        }

        [Authorize(ExamPermissions.PaperRepos.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            PaperRepo paperRepo = await paperRepoRepository.GetAsync(id);
            await paperRepoRepository.DeleteAsync(paperRepo);

            Paper paper = await paperRepository.GetAsync(paperRepo.PaperId);
            paper.Score = paper.Score - (paperRepo.SingleScore ?? 0) * (paperRepo.SingleCount ?? 0)
                          + (paperRepo.MultiScore ?? 0) * (paperRepo.MultiCount ?? 0)
                          + (paperRepo.JudgeScore ?? 0) * (paperRepo.JudgeCount ?? 0)
                          + (paperRepo.BlankScore ?? 0) * (paperRepo.BlankCount ?? 0);
            paper.TotalQuestionCount = paper.TotalQuestionCount - (paperRepo.SingleCount ?? 0) + (paperRepo.MultiCount ?? 0) + (paperRepo.JudgeCount ?? 0) + (paperRepo.BlankCount ?? 0);
            await paperRepository.UpdateAsync(paper);
        }

        /// <summary>
        /// 规范最大记录数
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        private async Task NormalizeMaxResultCountAsync(PagedAndSortedResultRequestDto input)
        {
            var maxPageSize = (await SettingProvider.GetOrNullAsync(PaperRepoSettings.MaxPageSize))?.To<int>();
            if (maxPageSize.HasValue && input.MaxResultCount > maxPageSize.Value)
            {
                input.MaxResultCount = maxPageSize.Value;
            }
        }
    }
}