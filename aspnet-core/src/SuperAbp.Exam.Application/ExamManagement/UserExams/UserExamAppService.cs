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

        public virtual async Task<UserExamDetailDto> GetAsync(Guid id)
        {
            UserExam entity = await _userExamRepository.GetAsync(id);

            return ObjectMapper.Map<UserExam, UserExamDetailDto>(entity);
        }

        public virtual async Task<PagedResultDto<UserExamListDto>> GetListAsync(GetUserExamsInput input)
        {
            await NormalizeMaxResultCountAsync(input);

            var queryable = await _userExamRepository.GetQueryableAsync();

            long totalCount = await AsyncExecuter.CountAsync(queryable);

            var entities = await AsyncExecuter.ToListAsync(queryable
                .OrderBy(input.Sorting ?? "Id DESC")
                .PageBy(input));

            var dtos = ObjectMapper.Map<List<UserExam>, List<UserExamListDto>>(entities);

            return new PagedResultDto<UserExamListDto>(totalCount, dtos);
        }

        public virtual async Task<UserExamListDto> CreateAsync(UserExamCreateDto input)
        {
            if (await _userExamRepository.AnyByExamIdAndUserIdAsync(input.ExamId, CurrentUser.GetId()))
            {
                throw new UserFriendlyException("您已经参加过此考试！");
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