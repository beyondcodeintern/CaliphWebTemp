﻿using BeyondCode.Core;
using BeyondCode.Helper;
using BeyondCode.Helper.Mapper;
using BeyondCode.Models.API.AgentRecruit;
using BeyondCode.Models.API.one2one;
using BeyondCode.Models.API.Agent;
using BeyondCode.Models.ViewModel;
using BeyondCode.Services;
using BeyondCode.Services.Helper;
using BeyondCode.ViewModel;
using BeyondCode.ViewModel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Caliph.Library.Helper;
using BeyondCode.Models.API.AgentStimulator;

namespace BeyondCode.Controllers
{
    public class AgentController : Controller
    {
        private readonly ICaliphAPIHelper _caliphAPIHelper;
        private readonly IMasterDataService _masterDataService;
        private readonly IUserService _userService;
        private readonly IOne2OneApiHelper _one2oneAPIHelper;

        public AgentController(ICaliphAPIHelper caliphAPIHelper, IMasterDataService masterDataService, IUserService userService, IOne2OneApiHelper one2OneApiHelper )
        {
            this._caliphAPIHelper = caliphAPIHelper;
            this._masterDataService = masterDataService;
            this._userService = userService;
            this._one2oneAPIHelper = one2OneApiHelper;
        }
        // GET: Agent
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Add()
        {
            var addAgentRecruitVM = new AddAgentRecruitViewModel();
            addAgentRecruitVM.EducationalBackgrounds = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.EducationBackground);
            addAgentRecruitVM.Age = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.AgeRange);
           
            addAgentRecruitVM.AnnualIncome = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.AnnualIncome);
            addAgentRecruitVM.Occupation = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.Occupation);

            addAgentRecruitVM.MaritalStatus = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.MaritalStatus);
            addAgentRecruitVM.Type = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.AgentRecruitType);

            return View(addAgentRecruitVM);
        }

        public async Task<ActionResult> AddAgent()
        {
            return View();
        }

        public async Task<ActionResult> List()
        {
            var vm = new AgentRecruitListViewModel();
            vm.Status = await _masterDataService.GetDealStatusAsync();


            var filter = new FiltertAgentRecruitRequest { PageNumber = 1, PageSize = 10, CreatedBy = UserHelper.GetLoginUser() };

            var responseData = await _caliphAPIHelper.PostAsync<FiltertAgentRecruitRequest, ResponseData<List<AgentRecruiment>>>(filter, "/api/v1/agent-recruit/get-by-filter");
            vm.AgentRecruimentData.AgentRecruiments = responseData.Data;
            vm.AgentRecruimentData.Paging.ItemCount = responseData.ItemCount;
            vm.AgentRecruimentData.Paging.PageSize = filter.PageSize;
            vm.AgentRecruimentData.Paging.CurrentPage = filter.PageNumber;

            foreach (var agentRecruits in vm.AgentRecruimentData.AgentRecruiments)
            {
                var activityFilter = new FilterAgentRecruitActivityRequest { PageNumber = 1, PageSize = 10, CreatedBy = UserHelper.GetDefaultSearchUser(), AgentId = agentRecruits.AgentRecruitId };
                var activityResponse = await _caliphAPIHelper.PostAsync<FilterAgentRecruitActivityRequest, ResponseData<List<AgentRecruimentActivity>>>(activityFilter, "/api/v1/agent-activity/get-by-filter");
                agentRecruits.Activities = activityResponse.Data.Where(x => x.StatusId != (int)MasterDataEnum.Status.Inactive).ToList();
                if (agentRecruits.Activities.Count() > 0)
                {
                    var nextActvity = agentRecruits.Activities.OrderByDescending(p => p.ActivityStartDate).First();
                    agentRecruits.NextActivityDate = nextActvity.ActivityStartDate;
                    agentRecruits.NextActivityDesc = nextActvity.ActivityPointsDesc;
                }
            }
            var addActivityVM = new AddAgentRecruitmentActivityViewModel();

            var filterAgentRecruitRequest = new FilterAgentRecruitActivityRequest { PageNumber = 1, PageSize = 999, CreatedBy = UserHelper.GetDefaultSearchUser() };
            var agentData = await _caliphAPIHelper.PostAsync<FilterAgentRecruitActivityRequest, ResponseData<List<AgentRecruiment>>>(filterAgentRecruitRequest, "/api/v1/agent-recruit/get-by-filter");
            addActivityVM.AgentRecruiments = agentData.Data.Where(x => x.StatusId != (int)MasterDataEnum.Status.Inactive).ToList();


            addActivityVM.Activities = await _masterDataService.GetAgentRecruitmentActivityPointAsync();
            vm.AddAgentRecruitmentActivityViewModel = addActivityVM;
            return View(vm);
        }

        public ActionResult AgentProfile()
        {
            return View();
        }
        public async Task<ActionResult> ReferralList()
        {
            var filter = new FilterAgentRecruitmentLeadRequest { PageNumber = 1, PageSize = 10, CreatedBy = UserHelper.GetDefaultSearchUser() };
            var responseData = await _caliphAPIHelper.PostAsync<FilterAgentRecruitmentLeadRequest, ResponseData<List<AgentRecruitmentLead>>>(filter, "/api/v1/agent-lead/get-by-filter");
            var referralViewModel = new AgentRecruitmentLeadListViewModel();
            referralViewModel.ReferralData.Referral = responseData.Data;
            referralViewModel.ReferralData.Paging.ItemCount = responseData.ItemCount;
            referralViewModel.ReferralData.Paging.PageSize = filter.PageSize;
            referralViewModel.ReferralData.Paging.CurrentPage = filter.PageNumber;
            referralViewModel.Users = await _userService.GetAgentUserAsync();
            return View(referralViewModel);
        }


        [HttpPost]
        public async Task<ActionResult> Referral(FilterAgentRecruitmentLeadRequest filter)
        {
            var responseData = await _caliphAPIHelper.PostAsync<FilterAgentRecruitmentLeadRequest, ResponseData<List<AgentRecruitmentLead>>>(filter, "/api/v1/agent-lead/get-by-filter");
            var referralViewModel = new AgentRecruitmentLeadListViewModel();
            referralViewModel.ReferralData.Referral = responseData.Data;
            referralViewModel.ReferralData.Paging.ItemCount = responseData.ItemCount;
            referralViewModel.ReferralData.Paging.PageSize = filter.PageSize;
            referralViewModel.ReferralData.Paging.CurrentPage = filter.PageNumber;
            referralViewModel.Users = await _userService.GetAgentUserAsync();
            return PartialView("_ReferralListTable", referralViewModel.ReferralData);
        }


        [HttpPost]
        public async Task<ActionResult> GetRecruimentActivity(int id)
        {
            var recruitActivitiesMasterData = await _masterDataService.GetAgentRecruitmentActivityPointAsync();
            var activityFilter = new FilterAgentRecruitActivityRequest { PageNumber = 1, PageSize = 10, CreatedBy = UserHelper.GetDefaultSearchUser(), AgentId = id };
            var activityResponse = await _caliphAPIHelper.PostAsync<FilterAgentRecruitActivityRequest, ResponseData<List<AgentRecruimentActivity>>>(activityFilter, "/api/v1/agent-activity/get-by-filter");

            var hasAgentContracted = false;
            if (activityResponse != null && activityResponse.Data != null)
            {
                hasAgentContracted = activityResponse.Data.Where(x => x.ActivityPointId == (int)MasterDataEnum.ActivityPoint.AgentContracted).Count()>0;
            }

            if(!hasAgentContracted)
            {
                recruitActivitiesMasterData.RemoveAll(x => x.ActivityPointId == (int)MasterDataEnum.ActivityPoint.Coaching_OneToOne);
                recruitActivitiesMasterData.RemoveAll(x => x.ActivityPointId == (int)MasterDataEnum.ActivityPoint.Training);
                recruitActivitiesMasterData.RemoveAll(x => x.ActivityPointId == (int)MasterDataEnum.ActivityPoint.JointFieldWork);
            }
          
            return Json(recruitActivitiesMasterData);
        }
        public async Task<ActionResult> AgentRecruitLeadAsync()
        {
            var addAgentRecruitVM = new AddAgentRecruitViewModel();
            addAgentRecruitVM.EducationalBackgrounds = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.EducationBackground);
            addAgentRecruitVM.Age = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.AgeRange);

            return View(addAgentRecruitVM);
        }
        [HttpPost]
        public async Task<JsonResult> Add(FormCollection formCollection)
        {
            var addClient = FormCollectionMapper.FormToModel<AddAgentRecruitRequest>(formCollection);
            addClient.CreatedBy = UserHelper.GetLoginUser();

            var response = await _caliphAPIHelper.PostAsync<AddAgentRecruitRequest, ResponseData<string>>(addClient, "/api/v1/agent-recruit/add");

            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> AddAgent(FormCollection formCollection)
        {
            var addAgent = FormCollectionMapper.FormToModel<AddStaffRequest>(formCollection);
            addAgent.CreatedBy = UserHelper.GetLoginUser();
            addAgent.JoinDate = DateTime.Now;
            addAgent.DisplayName = addAgent.Fullname;
            addAgent.RoleId = (int)MasterDataEnum.RoleId.PontentialAgent;
            var key = "H6&a##5";
            addAgent.PW = HashHelper.GetHashMd5(key + addAgent.PW);

            var response = await _caliphAPIHelper.PostAsync<AddStaffRequest, ResponseData<string>>(addAgent, "/api/v1/system-user/add");

            return Json(response);
        }

        [HttpPost]
        public async Task<ActionResult> List(FiltertAgentRecruitRequest filter)
        {
            var recruitmentData = new AgentRecruimentData();
            filter.CreatedBy = UserHelper.GetDefaultSearchUser();
            var responseData = await _caliphAPIHelper.PostAsync<FiltertAgentRecruitRequest, ResponseData<List<AgentRecruiment>>>(filter, "/api/v1/agent-recruit/get-by-filter");

            recruitmentData.AgentRecruiments = responseData.Data.Where(x => x.StatusId != (int)MasterDataEnum.Status.Inactive).ToList(); ;

            recruitmentData.Paging.ItemCount = responseData.ItemCount;
            recruitmentData.Paging.PageSize = filter.PageSize;
            recruitmentData.Paging.CurrentPage = filter.PageNumber;
            foreach (var agentRecruits in recruitmentData.AgentRecruiments)
            {
                var activityFilter = new FilterAgentRecruitActivityRequest { PageNumber = 1, PageSize = 10, CreatedBy = UserHelper.GetDefaultSearchUser(), AgentId = agentRecruits.AgentRecruitId };
                var activityResponse = await _caliphAPIHelper.PostAsync<FilterAgentRecruitActivityRequest, ResponseData<List<AgentRecruimentActivity>>>(activityFilter, "/api/v1/agent-activity/get-by-filter");
                agentRecruits.Activities = activityResponse.Data.Where(x => x.StatusId != (int)MasterDataEnum.Status.Inactive).ToList();
                if (agentRecruits.Activities.Count() > 0)
                {
                    var nextActvity = agentRecruits.Activities.OrderByDescending(p => p.ActivityStartDate.Value).First();
                    agentRecruits.NextActivityDate = nextActvity.ActivityStartDate;
                    agentRecruits.NextActivityDesc = nextActvity.ActivityPointsDesc;
                }
            }
            return PartialView("_AgentRecruitmentListTable", recruitmentData);
        }
        [HttpPost]
        public async Task<JsonResult> AddActivity(FormCollection formCollection)
        {
            var addActivity = FormCollectionMapper.FormToModel<AddAgentRecruitActivityRequest>(formCollection);
            if (addActivity.ActivityPointId == (int)MasterDataEnum.ActivityPoint.ReferralsLead)
            {
                var leadResponse = AddLead(formCollection);
                return Json(leadResponse);
            }
            addActivity.CreatedBy = UserHelper.GetLoginUser();
            var response = await _caliphAPIHelper.PostAsync<AddAgentRecruitActivityRequest, ResponseData<int?>>(addActivity, "/api/v1/agent-activity/add");


            return Json(response);
        }
        [HttpPost]
        public async Task<JsonResult> AddLead(FormCollection formCollection)
        {
            var addActivity = FormCollectionMapper.FormToModel<AddAgentRecruitActivityRequest>(formCollection);
            addActivity.CreatedBy = UserHelper.GetLoginUser();
            addActivity.ActivityPointId = 7;//ref leads
            addActivity.ActivityStartDate = DateTime.Now;

            var response = await _caliphAPIHelper.PostAsync<AddAgentRecruitActivityRequest, ResponseData<int?>>(addActivity, "/api/v1/agent-activity/add");


            var addLead = FormCollectionMapper.FormToModel<AddAgentRecruitmentLeadRequest>(formCollection);
            addLead.CreatedBy = UserHelper.GetLoginUser();
            addLead.AgentActivityId = response.Data ?? 0;
            var leadResponse = await _caliphAPIHelper.PostAsync<AddAgentRecruitmentLeadRequest, ResponseData<string>>(addLead, "/api/v1/agent-lead/add");
            return Json(leadResponse);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateStatus(int status, int id)
        {



            var updateReq = new UpdateAgentRecruitStatusRequest { AgentRecruitId = id, StatusId = status };
            updateReq.UpdatedBy = UserHelper.GetLoginUser();
            var response = await _caliphAPIHelper.PostAsync<UpdateAgentRecruitStatusRequest, ResponseData<string>>(updateReq, "/api/v1/agent-recruit/update-status");
            return Json(response);
        }



        public async Task<ActionResult> MapaCalculator(int? id)
        {
            int checkYear = DateTime.Now.Year;
            if (id.HasValue)
                checkYear = id.Value;

            var vm = new MapaCalculatorViewModel { Year= checkYear };
            var user = UserHelper.GetLoginUserViewModel();
            var mapaFilter = new AgentStimulatorFilterRequest { AgentSimulatorYear = checkYear, UserId= user.UserId, PageNumber=1, PageSize=10};

            // get percentage for target mapa 
            var response = await _caliphAPIHelper.PostAsync<AgentStimulatorFilterRequest, ResponseData<List<AgentStimulator>>>(mapaFilter,"/api/v1/agent-simulator/get-by-filter");
            var agent = new AgentStimulator();



            if (response != null && response.Data != null)
                agent = response.Data.FirstOrDefault() ?? new AgentStimulator(); ;

            
            // start = previous year 1-12 
            // end = current year update to date 


            var startDate = new DateTime(checkYear, 1, 1);
            var endDate = DateTime.Now;

            var actualMapaAchieved = await AchieveMapaAsync("u", startDate, endDate);


            var lastYearstartDate = new DateTime(checkYear - 1, 1, 1);
            var lastYearendDate = lastYearstartDate.AddYears(1).AddDays(-1);

            var previousMapaAchieved = await AchieveMapaAsync("u", lastYearstartDate, lastYearendDate);


            for (int month = 1; month <= 12; month++)
            {
               var personalMapaBudgets = new MapaPlanning { Month = month };
                personalMapaBudgets.Manpower_YTDRecruit = ConvertHelper.ConvertInt(agent.GetType().GetProperty(nameof(personalMapaBudgets.Manpower_YTDRecruit) + month.ToString()).GetValue(agent, null).ToString());
                personalMapaBudgets.ActiveAgent_TotalCases = ConvertHelper.ConvertInt(agent.GetType().GetProperty(nameof(personalMapaBudgets.ActiveAgent_TotalCases) + month.ToString()).GetValue(agent, null).ToString());
                personalMapaBudgets.ActiveAgent_YTDRecruit = ConvertHelper.ConvertInt(agent.GetType().GetProperty(nameof(personalMapaBudgets.ActiveAgent_YTDRecruit) + month.ToString()).GetValue(agent, null).ToString());
                personalMapaBudgets.ACE_TotalCases = ConvertHelper.ConvertInt(agent.GetType().GetProperty(nameof(personalMapaBudgets.ACE_TotalCases) + month.ToString()).GetValue(agent, null).ToString());

                var currentMonthAndYearAchieveMonth = actualMapaAchieved.Where(x => x.Month == month).FirstOrDefault()?? new MapaPlanning { Month=month};
                var currentMonthAndPreviousYearAchieveMonth = previousMapaAchieved.Where(x => x.Month == month).FirstOrDefault() ?? new MapaPlanning { Month = month };


               vm.PlannedMapa.Add(personalMapaBudgets);

                vm.CurrentYearActualMapa.Add(currentMonthAndYearAchieveMonth);
                vm.PreviousYearActualMapa.Add(currentMonthAndPreviousYearAchieveMonth);
            }
            vm.ID = agent.AgentSimulatorId;

            return View(vm);
        }

        public async Task<List<MapaPlanning>> AchieveMapaAsync( string type, DateTime startDate , DateTime endDate)
        {



            var request = new AgentMapaRequest
            {
                start_month = startDate.Month,
                start_year = startDate.Year,
                agent_id = UserHelper.GetLoginUserViewModel().Username,
                type = type,
                end_month = endDate.Month,
                end_year = endDate.Year
            };


            var response =await  _one2oneAPIHelper.PostAsync<AgentMapaRequest, One2OneResponse<AgentMapaResponse>>(request, "/edfwebapi/alc/agentmapa");
            var result = new List<MapaPlanning>();


            var agentMapa = response.data;
            if (agentMapa == null)
                return result;


            // map from api to mapa planning modal 
            foreach (var mapa in agentMapa)
            {
                var actualMapa = new MapaPlanning
                {

                    ACE = ConvertHelper.ConvertDecimal(mapa.ace_mtd),
                    ActiveAgent = mapa.active_agent_mtd,
                    NewRecruit = mapa.recruit_mtd,
                    YtdRecruit = mapa.recruit_ytd,
                    TotalCases = mapa.case_mtd,
                    Month = mapa.month,

                };

                actualMapa.Manpower_YTDRecruit = mapa.manpower;
                actualMapa.ActiveAgent_TotalCases = actualMapa.ActiveAgent == 0 ? 0 : ((decimal)actualMapa.TotalCases / (decimal)actualMapa.ActiveAgent);
                actualMapa.ActiveAgent_YTDRecruit = actualMapa.YtdRecruit == 0 ? 0 : (int)(((decimal)actualMapa.ActiveAgent / (decimal)actualMapa.YtdRecruit) * 100); ;
                actualMapa.ACE_TotalCases = actualMapa.TotalCases == 0 ? 0 : actualMapa.ACE / actualMapa.TotalCases; ;
                actualMapa.Month = actualMapa.Month;
                result.Add(actualMapa);
            }
              
            return result;
        }


        [HttpPost]
        public async Task<ActionResult> UpdateGrowth(AddAgentStimulatorRequest req)
        {

            var user = UserHelper.GetLoginUserViewModel();
            req.UserId = user.UserId;
            req.CreatedBy = user.Username;
            
            if (req.AgentSimulatorId > 0)
            {
                var updateReq =  AutoMapHelper.Map<AddAgentStimulatorRequest, UpdateAgentStimulatorRequest>(req);
                updateReq.UpdatedBy = user.Username;

                var response = await _caliphAPIHelper.PostAsync<UpdateAgentStimulatorRequest, ResponseData<string>>(updateReq, "/api/v1/agent-simulator/update");
                return Json(response);
            }
            else
            {
              //  var addReq = new AddAgentStimulatorRequest { UserId = user.UserId, GrowthPercentage = growth, CreatedBy = user.Username };
                var response = await _caliphAPIHelper.PostAsync<AddAgentStimulatorRequest, ResponseData<string>>(req, "/api/v1/agent-simulator/add");
                return Json(response);
            }

        }

        public MapaPlanning DummyCurrentYearAchieveMapa(int month, MapaPlanning actualMapa)
        {
            switch (month)
            {
                case 1:
                    actualMapa.ACE = 3912001;
                    actualMapa.ActiveAgent = 608;
                    actualMapa.NewRecruit = 64;
                    actualMapa.YtdRecruit = 1849;
                    actualMapa.TotalCases = 1797;
                    break;
                case 2:
                    actualMapa.ACE = 6143893;
                    actualMapa.ActiveAgent = 649;
                    actualMapa.NewRecruit = 90;
                    actualMapa.YtdRecruit = 2020;
                    actualMapa.TotalCases = 1902;
                    break;
                case 3:
                    actualMapa.ACE = 5116817;
                    actualMapa.ActiveAgent = 713;
                    actualMapa.NewRecruit = 106;
                    actualMapa.YtdRecruit = 2222;
                    actualMapa.TotalCases = 1850;
                    break;
                case 4:
                    actualMapa.ACE = 6610000;
                    actualMapa.ActiveAgent = 1;
                    actualMapa.NewRecruit = 0;
                    actualMapa.YtdRecruit = 1;
                    actualMapa.TotalCases = 15;
                    break;
                case 5:
                    actualMapa.ACE = 126000;
                    actualMapa.ActiveAgent = 1;
                    actualMapa.NewRecruit = 0;
                    actualMapa.YtdRecruit = 1;
                    actualMapa.TotalCases = 15;
                    break;
                case 6:
                    actualMapa.ACE = 126000;
                    actualMapa.ActiveAgent = 1;
                    actualMapa.NewRecruit = 0;
                    actualMapa.YtdRecruit = 1;
                    actualMapa.TotalCases = 15;
                    break;
                case 7:
                    actualMapa.ACE = 6480000;
                    actualMapa.ActiveAgent = 1;
                    actualMapa.NewRecruit = 0;
                    actualMapa.YtdRecruit = 1;
                    actualMapa.TotalCases = 15;
                    break;
                case 8:
                    actualMapa.ACE = 6804000;
                    break;
                case 9:
                    actualMapa.ACE = 1276000;
                    break;
                case 10:
                    actualMapa.ACE = 8200000;
                    break;
                case 11:
                    actualMapa.ACE = 116000;
                    break;
                case 12:
                    actualMapa.ACE = 134000;
                    break;


            }

            return actualMapa;
        }

        public MapaPlanning DummyPreviousYearActualMapa(int month)
        {

            var actualMapaAchieved = new MapaPlanning { Month=month};
            switch (month)
            {
                case 1:
                    actualMapaAchieved.ACE = (decimal)3912001.00;
                    actualMapaAchieved.ActiveAgent = 608;
                    actualMapaAchieved.Manpower_YTDRecruit = 1849;
                    actualMapaAchieved.NewRecruit = 64;
                    actualMapaAchieved.TotalCases = 1797;
                    break;
                case 2:
                    actualMapaAchieved.ACE = 6143893;
                    actualMapaAchieved.ActiveAgent = 649;
                    actualMapaAchieved.Manpower_YTDRecruit = 2033;
                    actualMapaAchieved.NewRecruit = 180;
                    actualMapaAchieved.TotalCases = 1902;
                    break;
                case 3:
                    actualMapaAchieved.ACE = 5116817;
                    actualMapaAchieved.ActiveAgent = 713;
                    actualMapaAchieved.Manpower_YTDRecruit = 2246;
                    actualMapaAchieved.NewRecruit = 226;
                    actualMapaAchieved.TotalCases = 1850;
                    break;
                case 4:
                    actualMapaAchieved.ACE = 4747699;
                    actualMapaAchieved.ActiveAgent = 648;
                    actualMapaAchieved.Manpower_YTDRecruit = 2397;
                    actualMapaAchieved.NewRecruit = 83;
                    actualMapaAchieved.TotalCases = 1888;
                    break;
                case 5:
                    actualMapaAchieved.ACE = 3247649;
                    actualMapaAchieved.ActiveAgent = 601;
                    actualMapaAchieved.Manpower_YTDRecruit = 2543;
                    actualMapaAchieved.NewRecruit = 87;
                    actualMapaAchieved.TotalCases = 1726;
                    break;
                case 6:
                    actualMapaAchieved.ACE = 4910097;
                    actualMapaAchieved.ActiveAgent = 694;
                    actualMapaAchieved.Manpower_YTDRecruit = 2692;
                    actualMapaAchieved.NewRecruit = 160;
                    actualMapaAchieved.TotalCases = 2054;
                    break;
                case 7:
                    actualMapaAchieved.ACE = 5916056;
                    actualMapaAchieved.ActiveAgent = 747;
                    actualMapaAchieved.Manpower_YTDRecruit = 2783;
                    actualMapaAchieved.NewRecruit = 186;
                    actualMapaAchieved.TotalCases = 2305;
                    break;
                case 8:
                    actualMapaAchieved.ACE = 5522147;
                    actualMapaAchieved.ActiveAgent = 745;
                    actualMapaAchieved.Manpower_YTDRecruit = 2980;
                    actualMapaAchieved.NewRecruit = 189;
                    actualMapaAchieved.TotalCases = 2144;
                    break;
                case 9:
                    actualMapaAchieved.ACE = 6013631;
                    actualMapaAchieved.ActiveAgent = 699;
                    actualMapaAchieved.Manpower_YTDRecruit = 3043;
                    actualMapaAchieved.NewRecruit = 200;
                    actualMapaAchieved.TotalCases = 1804;
                    break;
                case 10:
                    actualMapaAchieved.ACE = 5599778;
                    actualMapaAchieved.ActiveAgent = 662;
                    actualMapaAchieved.Manpower_YTDRecruit = 3070;
                    actualMapaAchieved.NewRecruit = 182;
                    actualMapaAchieved.TotalCases = 1660;
                    break;
                case 11:
                    actualMapaAchieved.ACE = 4812580;
                    actualMapaAchieved.ActiveAgent = 577;
                    actualMapaAchieved.Manpower_YTDRecruit = 3009;
                    actualMapaAchieved.NewRecruit = 105;
                    actualMapaAchieved.TotalCases = 1430;
                    break;
                case 12:
                    actualMapaAchieved.ACE = 7280561;
                    actualMapaAchieved.ActiveAgent = 681;
                    actualMapaAchieved.Manpower_YTDRecruit = 2280;
                    actualMapaAchieved.NewRecruit = 145;
                    actualMapaAchieved.TotalCases = 2103;
                    break;


            }

            return actualMapaAchieved;
        }


        public DirectGroupMapaBuget DummyDirectGroupBudget(int month)
        {

            var actualMapaAchieved = new DirectGroupMapaBuget { Month = month };
            switch (month)
            {
                case 1:
                    actualMapaAchieved.ACE = (decimal)3000;
                    actualMapaAchieved.ActiveAgent_TotalCases = 608;
                    actualMapaAchieved.MPower = 1849;
                    actualMapaAchieved.Rec = 141;
                    actualMapaAchieved.TotalCases = 1797;
                    break;
                case 2:
                    actualMapaAchieved.ACE = 6143893;
                    actualMapaAchieved.ActiveAgent = 649;
                    actualMapaAchieved.MPower = 2033;
                    actualMapaAchieved.Rec = 180;
                    actualMapaAchieved.TotalCases = 19;
                    break;
                case 3:
                    actualMapaAchieved.ACE = 5116817;
                    actualMapaAchieved.ActiveAgent = 713;
                    actualMapaAchieved.MPower = 2246;
                    actualMapaAchieved.Rec = 226;
                    actualMapaAchieved.TotalCases = 1850;
                    break;
                case 4:
                    actualMapaAchieved.ACE = 5116817;
                    actualMapaAchieved.ActiveAgent = 3;
                    actualMapaAchieved.MPower = 1;
                    actualMapaAchieved.Rec = 6;
                    actualMapaAchieved.TotalCases = 15;
                    break;
                case 5:
                    actualMapaAchieved.ACE = 126000;
                    actualMapaAchieved.ActiveAgent = 6;
                    actualMapaAchieved.MPower = 1;
                    actualMapaAchieved.Rec = 10;
                    actualMapaAchieved.TotalCases = 15;
                    break;
                case 6:
                    actualMapaAchieved.ACE = 126000;
                    actualMapaAchieved.ActiveAgent = 7;
                    actualMapaAchieved.MPower = 0;
                    actualMapaAchieved.Rec = 21;
                    actualMapaAchieved.TotalCases = 15;
                    break;
                case 7:
                    actualMapaAchieved.ACE = 6480000;
                    actualMapaAchieved.ActiveAgent = 1;
                    actualMapaAchieved.MPower = 0;
                    actualMapaAchieved.Rec = 1;
                    actualMapaAchieved.TotalCases = 15;
                    break;
                case 8:
                    actualMapaAchieved.ACE = 6804000;
                    break;
                case 9:
                    actualMapaAchieved.ACE = 1270000;
                    break;
                case 10:
                    actualMapaAchieved.ACE = 820000;
                    break;
                case 11:
                    actualMapaAchieved.ACE = 116000;
                    break;
                case 12:
                    actualMapaAchieved.ACE = 134000;
                    break;


            }

            return actualMapaAchieved;
        }


        [HttpPost]
        public async Task<ActionResult> GetAgentByGeneration(int generation)
        {
            var login = UserHelper.GetLoginUserViewModel();

            var req = new AgentHierarchyRequest { agent_id = UserHelper.GetDefaultOne2OneSearchUser(), generation=generation.ToString() };
             

            var responseData = await _one2oneAPIHelper.PostAsync<AgentHierarchyRequest, One2OneResponse<AgentHierarchyResponse>>(req, "/edfwebapi/alc/agenthierarchy");

            if (responseData == null || responseData.data == null)
            {
                if (generation == 0)
                {
                    var agent = new AgentHierarchyResponse { agent_id = login.Username, agent_name = login.Fullname };
                    var result = new List<AgentHierarchyResponse>();
                    result.Add(agent);
                    return Json(new List<AgentHierarchyResponse>());
                }
                else
                    return Json(new List<AgentHierarchyResponse>());
            }
            else
                return Json(responseData.data);
        }

        [HttpPost]
        public async Task<ActionResult> GetAgentByGenerationAndID(AgentHierarchyRequest req)
        {

            var responseData = await _one2oneAPIHelper.PostAsync<AgentHierarchyRequest, One2OneResponse<AgentHierarchyResponse>>(req, "/edfwebapi/alc/agenthierarchy");

            if (responseData == null || responseData.data == null || responseData.data.Count == 0)
            {
                if (ConvertHelper.ConvertInt(req.generation) == 0)
                {
                    var login = UserHelper.GetLoginUserViewModel();
                    var agent = new AgentHierarchyResponse { agent_id = login.Username, agent_name = login.Fullname };
                    var result = new List<AgentHierarchyResponse>();
                    result.Add(agent);
                    return Json(new List<AgentHierarchyResponse>());
                }
                else
                    return Json(new List<AgentHierarchyResponse>());
            }
            else
            {
                TempData["Agents"] = responseData.data;
                return Json(responseData.data);
            }
        }



        public async Task<ActionResult> Edit(int id)
        {
            var vm = new UpdateAgentRecruitViewModel();
            var filter = new FiltertAgentRecruitRequest { PageNumber = 1, PageSize = 999, CreatedBy = UserHelper.GetDefaultSearchUser(), AgentRecruitId=id };
            var responseData = await _caliphAPIHelper.PostAsync<FiltertAgentRecruitRequest, ResponseData<List<AgentRecruiment>>>(filter, "/api/v1/agent-recruit/get-by-filter");

            vm.AgentRecruiment = (responseData == null || responseData.Data == null) ? new AgentRecruiment() : responseData.Data.FirstOrDefault();
            vm.EducationalBackgrounds = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.EducationBackground);
            vm.Age = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.AgeRange);

            vm.AnnualIncome = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.AnnualIncome);
            vm.Occupation = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.Occupation);

            vm.MaritalStatus = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.MaritalStatus);
            vm.Type = await _masterDataService.GetMasterDatasAsync(MasterDataEnum.MasterData.AgentRecruitType);

            return View(vm);
        }


        [HttpPost]
        public async Task<JsonResult> Edit(FormCollection formCollection)
        {
            var updateDealReq = FormCollectionMapper.FormToModel<UpdateAgentRecruitRequest>(formCollection);
            updateDealReq.UpdatedBy = UserHelper.GetLoginUser();
            var response = await _caliphAPIHelper.PostAsync<UpdateAgentRecruitRequest, ResponseData<string>>(updateDealReq, "/api/v1/agent-recruit/update");
            return Json(response);
        }

    }




}