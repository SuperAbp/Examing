﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SuperAbp.Exam.ExamManagement.Exams;
using SuperAbp.Exam.PaperManagement.PaperRepos;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;

namespace SuperAbp.Exam.ExamManagement.UserExams
{
    [Authorize]
    public class UserExamAppService : ExamAppService, IUserExamAppService
    {
        private readonly IUserExamRepository _userExamRepository;
        private readonly UserExamManager _userExamManager;
        private readonly IExamRepository _examRepository;
        private readonly IPaperRepoRepository _paperRepoRepository;

        public UserExamAppService(
            IUserExamRepository userExamRepository, UserExamManager userExamManager, IExamRepository examRepository, IPaperRepoRepository paperRepoRepository)
        {
            _userExamRepository = userExamRepository;
            _userExamManager = userExamManager;
            _examRepository = examRepository;
            _paperRepoRepository = paperRepoRepository;
        }

        public async Task<Guid?> GetUnfinishedAsync()
        {
            var userExam = await _userExamRepository.FindAsync(u => u.UserId == CurrentUser.GetId() && !u.Finished);
            return userExam?.Id;
            
        }
        public virtual async Task<UserExamDetailDto> GetAsync(Guid id)
        {
            UserExam entity = await _userExamRepository.GetAsync(id);

            return ObjectMapper.Map<UserExam, UserExamDetailDto>(entity);
        }

        public virtual async Task<PagedResultDto<UserExamListDto>> GetListAsync(GetUserExamsInput input)
        {
            await NormalizeMaxResultCountAsync(input);

            var queryable = await _userExamRepository.GetQueryableAsync();
            var examQueryable = await _examRepository.GetQueryableAsync();
            // TODO:性能较低，需要优化
            var result = from ue in queryable
                         join e in examQueryable on ue.ExamId equals e.Id
                         group new { ue, e } by ue.ExamId into g
                         select new UserExamWithExam
                         {
                             ExamId = g.Key,
                             ExamName = g.Max(m => m.e.Name),
                             Count = g.Count(),
                             LastTime = g.Max(m => m.ue.CreationTime),
                             MaxScore = g.Max(m => m.ue.TotalScore)
                         };
            var totalCount = await AsyncExecuter.CountAsync(result);
            var entities = await AsyncExecuter.ToListAsync(result
                .OrderBy(input.Sorting ?? UserExamConsts.DefaultSorting)
                .PageBy(input));

            var dtos = ObjectMapper.Map<List<UserExamWithExam>, List<UserExamListDto>>(entities);
            return new PagedResultDto<UserExamListDto>(totalCount, dtos);
        }

        public virtual async Task<UserExamListDto> CreateAsync(UserExamCreateDto input)
        {
            if(await _userExamRepository.AnyAsync(ue => ue.UserId == CurrentUser.GetId() && !ue.Finished))
            {
                throw new BusinessException(ExamDomainErrorCodes.ExistsUnfinishedExams);
            }
            var exam = await _examRepository.GetAsync(input.ExamId);
            if (exam.StartTime > DateTime.Now || exam.EndTime < DateTime.Now)
            {
                throw new BusinessException(ExamDomainErrorCodes.OutOfExamTime);
            }
            var userExam = new UserExam(GuidGenerator.Create(), input.ExamId, CurrentUser.GetId());
            await _userExamManager.CreateQuestionsAsync(userExam.Id, input.ExamId);
            await _userExamRepository.InsertAsync(userExam);
            return ObjectMapper.Map<UserExam, UserExamListDto>(userExam);
        }

        private async Task NormalizeMaxResultCountAsync(PagedAndSortedResultRequestDto input)
        {
            var maxPageSize = (await SettingProvider.GetOrNullAsync(UserExamSettings.MaxPageSize))?.To<int>();
            if (maxPageSize.HasValue && input.MaxResultCount > maxPageSize.Value)
            {
                input.MaxResultCount = maxPageSize.Value;
            }
        }

    }
}