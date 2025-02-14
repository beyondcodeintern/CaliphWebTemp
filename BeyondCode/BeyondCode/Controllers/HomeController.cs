﻿using BeyondCode.Core;
using BeyondCode.Helper;
using BeyondCode.Helper.Mapper;
using BeyondCode.Models.API;
using BeyondCode.Models.API.Announcement.Request;
using BeyondCode.Models.API.Announcement.Response;
using BeyondCode.Models.API.Deal;
using BeyondCode.Models.API.Event.Response;
using BeyondCode.Models.API.one2one;
using BeyondCode.Models.ViewModel;
using BeyondCode.Services;
using BeyondCode.Services.Helper;
using BeyondCode.ViewModel;
using BeyondCode.ViewModel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BeyondCode.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMasterDataService _masterService;
        private readonly ICaliphAPIHelper _caliphAPIHelper;
        private readonly IUserService _userService;
        private readonly IOne2OneApiHelper _one2oneAPIHelper;

        public ICaliphAPIHelper CaliphAPIHelper => _caliphAPIHelper;

        public HomeController(IMasterDataService masterService, ICaliphAPIHelper caliphAPIHelper, IUserService userService, IOne2OneApiHelper one2oneAPIHelper)
        {
            this._masterService = masterService;
            this._caliphAPIHelper = caliphAPIHelper;
            this._userService = userService;
            this._one2oneAPIHelper = one2oneAPIHelper;
        }
        public async Task<ActionResult> Index()
        {


            var dealfilter = new DealFilterRequest { PageNumber = 1, PageSize = 9999, CreatedBy = UserHelper.GetDefaultSearchUser() };
            var dealresponseData = await CaliphAPIHelper.PostAsync<DealFilterRequest, ResponseData<List<Deal>>>(dealfilter, "/api/v1/client-deal/get-by-filter");


            var clientfilter = new ClientFilterRequest { PageNumber = 1, PageSize = 9999, CreatedBy = UserHelper.GetDefaultSearchUser() };
            var clientresponseData = await CaliphAPIHelper.PostAsync<ClientFilterRequest, ResponseData<List<Client>>>(clientfilter, "/api/v1/client/get-by-client-filter");


            var activityFilter = new ActivityFilterRequest { PageNumber = 1, PageSize = 9999, ActivityStartDate = DateTime.Now, CreatedBy = UserHelper.GetDefaultSearchUser() };
            var activityresponseData = await CaliphAPIHelper.PostAsync<ActivityFilterRequest, ResponseData<List<DealActivity>>>(activityFilter, "/api/v1/client-deal-activity/get-by-filter");
            var vm = new IndexViewModel();


            if (dealresponseData != null && dealresponseData.Data != null)
            {
                vm.TotalDeal = dealresponseData.Data.Where(x =>
                 x.StatusId == (int)MasterDataEnum.Status.Active ||
                 x.StatusId == (int)MasterDataEnum.Status.Closed ||
                 x.StatusId == (int)MasterDataEnum.Status.Lost).Count();


                vm.ActiveDeal = dealresponseData.Data.Where(x =>
                 x.StatusId == (int)MasterDataEnum.Status.Active).Count();


                vm.ClosedDeal = dealresponseData.Data.Where(x =>
                 x.StatusId == (int)MasterDataEnum.Status.Closed).Count();
            }

            if (clientresponseData != null && clientresponseData.Data != null)
            {
                vm.TotalClient = clientresponseData.Data.Where(x =>
                 x.StatusId == (int)MasterDataEnum.Status.Confirm ||
                 x.StatusId == (int)MasterDataEnum.Status.Potential).Count();

                vm.ConfirmedClient = clientresponseData.Data.Where(x =>
           x.StatusId == (int)MasterDataEnum.Status.Confirm).Count();
            }


            if (activityresponseData != null && activityresponseData.Data != null)
            {

                vm.UpcomingActivity = activityresponseData.Data.Where(x =>
         x.StatusId == (int)MasterDataEnum.Status.Active).Count();
            }

            var startDate = DateTime.Now.AddDays(-6);
            var endDate = DateTime.Now;

            var loopDate = startDate;

            while (loopDate <= endDate)
            {
                var activeDeals = dealresponseData.Data.Where(x => (x.StatusId == (int)MasterDataEnum.Status.Active ||
                x.StatusId == (int)MasterDataEnum.Status.Closed ||
                x.StatusId == (int)MasterDataEnum.Status.Lost) && x.CreatedDate.Date == loopDate.Date).Count();
                var closedDeals = dealresponseData.Data.Where(x => x.StatusId == (int)MasterDataEnum.Status.Closed && x.CreatedDate.Date == loopDate.Date).Count();
                var lostDeals = dealresponseData.Data.Where(x => x.StatusId == (int)MasterDataEnum.Status.Lost && x.CreatedDate.Date == loopDate.Date).Count();

                vm.WeeklyActiveDeal.Add(activeDeals);
                vm.WeeklyClosedDeal.Add(closedDeals);
                vm.WeeklyLostDeal.Add(lostDeals);
                vm.WeeklyDays.Add(loopDate.Date.ToString("dddd"));
                loopDate = loopDate.AddDays(1);

            }

            var user = UserHelper.GetLoginUserViewModel();
            var filter = new AnnouncementGet
            {
                PageSize = 100,
                PageNumber = 1,
                UserId = user.UserId,
                PublishEndDate = DateTime.Now
            };

            var response = await _caliphAPIHelper.PostAsync<AnnouncementGet, ResponseData<List<Announcement>>>(filter, "/api/v1/announcement/get-by-filter");
            vm.Announcements = response.Data.Where(x => x.PublishEndDate >= DateTime.Now).ToList();


            var firstdayOfTheTodayMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var filter1 = new AgentCommissionGet
            {
                PayoutDateFrom = firstdayOfTheTodayMonth,
                PayoutDateTo = DateTime.Now.Date,
                Username = user.Username,
                PageSize = 100,
                PageNumber = 1,
            };

            var response1 = await _caliphAPIHelper.PostAsync<AgentCommissionGet, ResponseData<List<AgentCommission>>>(filter1, "/api/v1/agent-commission/get-by-filter");
            decimal total = 0;
            if (response1.Data.Count > 0)
            {
                foreach (var item in response1.Data)
                {
                    total = total + item.CommAmt;
                }
            }
            vm.TotalCommissionAmt = total;


          
            return View(vm);
        }


        [HttpPost]
        public async Task<ActionResult> GetACE(string type )
        {
            var user = UserHelper.GetLoginUser();

            var req = new AgentMapaRequest { agent_id = user, type = type, start_month = 1,
                end_month = DateTime.Now.Month,
                start_year = DateTime.Now.Year, end_year = DateTime.Now.Year

            };
            var responseData = await _one2oneAPIHelper.PostAsync<AgentMapaRequest, One2OneResponse<AgentMapaResponse> >(req, "/edfwebapi/alc/agentmapa");
            var ace = (responseData == null || responseData.data == null) ? new AgentMapaResponse() : responseData.data.OrderByDescending(x => x.month).FirstOrDefault();

            return Json(ace);
        }

        [HttpPost]
        public async Task<ActionResult> GetPersistency(string type)
        {

            var startDate = DateTime.Now;
            var endDate = DateTime.Now;
            var persistencyDate = DateTime.Now;
            if (type == "d0")
            {
                startDate = new DateTime(DateTime.Now.Year, 1, 1);
                endDate = new DateTime(DateTime.Now.Year, 12, 31);
                var lastMonth = DateTime.Now.AddMonths(-1);
                var daysInMonth = DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month);
                persistencyDate = new DateTime(lastMonth.Year, lastMonth.Month, daysInMonth);
            }
            else if (type == "d1")
            {
                startDate = new DateTime(  DateTime.Now.Year - 1,1,1);
                endDate = new DateTime( DateTime.Now.Year - 1, 12, 31);
                persistencyDate = new DateTime( DateTime.Now.Year - 1, 12, 31);
            }


            var user = UserHelper.GetLoginUser();

            var req = new AgentPolicyRequest
            {
                agent_id = user,
                date_from = startDate, date_to = endDate

            };
            var responseData = await _one2oneAPIHelper.PostAsync<AgentPolicyRequest, One2OneResponse<AgentPolicyResponse>>(req, "/edfwebapi/alc/AgentPolicyData");
            var vm = new PersistencySummaryData{ StartDate = startDate, EndDate = endDate, PersistencyDate = persistencyDate, AgentId = user};
            vm.GroupPolicies = (responseData == null || responseData.data == null) ? new List<AgentPolicyResponse>() : responseData.data;
            var personalPolicies = vm.GroupPolicies.Where(x => x.selling_agent_code == user).ToList();
            vm.PersonalPolicies = (personalPolicies == null ) ? new List<AgentPolicyResponse>() : personalPolicies;
            return Json(new PersistencySummary { GroupRatio = vm.GroupRatio, PersonalRatio=vm.PersonalRatio});
        }


        [HttpPost]
        public async Task<ActionResult> GetBonusTracker()
        {
            var user = UserHelper.GetLoginUserViewModel();
            if (user.IsAgent || user.IsLeader)
            {
                var startDate = new DateTime(DateTime.Now.Year, 1, 1);
                var endDate = new DateTime(DateTime.Now.Year, 12, 31);
                var lastMonth = DateTime.Now.AddMonths(-1);
                var daysInMonth = DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month);
                var persistencyDate = new DateTime(lastMonth.Year, lastMonth.Month, daysInMonth);

                var req = new AgentPolicyRequest
                {
                    agent_id = user.Username,
                    date_from = startDate,
                    date_to = endDate

                };
                var responseData = await _one2oneAPIHelper.PostAsync<AgentPolicyRequest, One2OneResponse<AgentPolicyByProdyctResponse>>(req, "/edfwebapi/alc/AgentPolicyProductData");
                var vm = new BonusTrackerViewModel {  PersistencyDate = persistencyDate  };
                vm.AgentProductPolicies = (responseData == null || responseData.data == null) ? new List<AgentPolicyByProdyctResponse>() : responseData.data;

                var bonusContestRules = BonusService.GetContests();
                var bonusDashboard = AutoMapHelper.Map<BonusTrackerViewModel, BonusTrackerDashboard>(vm);

                bonusDashboard.QualifiedBonus = bonusContestRules.Where(x => vm.ACE >= x.ACE &&
                vm.Cases >= x.Cases &&
                vm.PersistencyRatio >= x.PR_D0).OrderBy(x => x.BonusPercent).ToList();

                var nextBonus = bonusDashboard.QualifiedBonus.Count + 1;
                if (nextBonus > bonusContestRules.Count)
                    nextBonus = bonusContestRules.Count;

                bonusDashboard.PotentialBonus =  bonusContestRules[nextBonus-1] ;
                return Json(bonusDashboard);
            }
            return Json(new BonusTrackerDashboard ());
        }

        [HttpPost]
        public async Task<ActionResult> GetCPDPoint()
        {

           
            var user = UserHelper.GetLoginUserViewModel();

            var userEvent = await _caliphAPIHelper.PostAsync<EventFilterViewModel, ResponseData<List<EventAttendanceListResponse>>>(new EventFilterViewModel {  UserId=user.UserId , AttendanceId = (int)MasterDataEnum.Status.Attended  }, "/api/v1/event/user-get-by-filter");
            var responseData =    (userEvent == null || userEvent.Data == null) ? new List<EventAttendanceListResponse>() : userEvent.Data;
            var cpd = responseData.Sum(x => x.CPDPoint);
            return Json(cpd);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}