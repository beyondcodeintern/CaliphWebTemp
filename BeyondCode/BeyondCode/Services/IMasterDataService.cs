﻿using BeyondCode.Models.API;
using BeyondCode.ViewModel;
using BeyondCode.ViewModel.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeyondCode.Services
{
    public interface IMasterDataService
    {
        Task<List<ActivityPoint>> GetActivityPointAsync();
        Task<List<MasterData>> GetClientStatusAsync();
        Task<List<MasterData>> GetDealActivityStatusAsync();
        Task<List<MasterData>> GetDealStatusAsync();
        Task<List<MasterData>> GetMasterDatasAsync(MasterDataEnum.MasterData masterid);
        Task<List<ActivityPoint>> GetSalesActivityPointAsync();
        Task<List<MasterData>> GetAnnouncementTypeAsync();
        Task<List<MasterData>> GetEventTypeAsync();
        Task<List<MasterData>> GetEventHostAsync();
        Task<List<MasterData>> GetEventChannelAsync();
        Task<List<MasterData>> GetEventAttendantTypeAsync();
        Task<List<MasterData>> GetEventQuizScoreAsync();
        Task<List<MasterData>> GetUserEventStatusAsync();
        Task<List<MasterData>> GetPaymentChannelAsync();
        Task<List<MasterData>> GetEventPaymentStatusAsync();
        Task<List<ActivityPoint>> GetAgentRecruitmentActivityPointAsync();

        Task<List<MasterData>> GetResourceStatusAsync();
    }
}