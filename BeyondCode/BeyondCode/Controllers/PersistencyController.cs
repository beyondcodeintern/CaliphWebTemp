﻿using AutoMapper;
using BeyondCode.Core;
using BeyondCode.Helper;
using BeyondCode.Helper.Mapper;
using BeyondCode.Models.API;
using BeyondCode.Models.API.AgentRecruit;
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
    [Authorize]
    public class PersistencyController : Controller
    {

        private readonly IMasterDataService _masterService;
        private readonly ICaliphAPIHelper _caliphAPIHelper;
        private readonly IUserService _userService;
        private readonly IOne2OneApiHelper _one2oneAPIHelper;
        public PersistencyController(IMasterDataService masterService, ICaliphAPIHelper caliphAPIHelper, IUserService userService, IOne2OneApiHelper one2oneAPIHelper)
        {
            this._masterService = masterService;
            this._caliphAPIHelper = caliphAPIHelper;
            this._userService = userService;
            this._one2oneAPIHelper = one2oneAPIHelper;
        }


        public List<AgentHierarchyResponse> GetDummyHierarchies()
        {
            var result = new List<AgentHierarchyResponse>();

            result.Add(new AgentHierarchyResponse
            {
                agent_id = "A000001",
                agent_name = "Nurul Azira Sahroni binti Yussof",
                upline_agent_id = "A00800",
                upline_agent_name = "",
                role = "leader",
                generation = 0
            }); ; ;

            result.Add(new AgentHierarchyResponse
            {
                agent_id = "A000002",
                agent_name = "Zahirah Nazirin",
                upline_agent_id = "A00800",
                upline_agent_name = "",
                role = "leader",
                generation = 1
            }); ;
            result.Add(new AgentHierarchyResponse
            {
                agent_id = "A000003",
                agent_name = "Muhammed Hj Humadu bin Muhaimen",
                upline_agent_id = "A000002",
                upline_agent_name = "",
                role = "agent",
                generation = 3
            }); ;

            result.Add(new AgentHierarchyResponse
            {
                agent_id = "A000004",
                agent_name = "Teo Teo Miu",
                upline_agent_id = "A000002",
                upline_agent_name = "",
                role = "agent",
                generation = 3
            }); ;



            result.Add(new AgentHierarchyResponse
            {
                agent_id = "A000005",
                agent_name = "Nursarwindah Hishhram binti Remi",
                upline_agent_id = "A00800",
                upline_agent_name = "",
                role = "leader",
                generation = 2
            }); ;

            result.Add(new AgentHierarchyResponse
            {
                agent_id = "A000006",
                agent_name = "Jahari Sani bin Wan Afif Arias",
                upline_agent_id = "A000005",
                upline_agent_name = "",
                role = "agent",
                generation = 3
            }); ;

            return result;
        }

        public List<AgentPolicyResponse> GetDummyPolicies()
        {
            var result = new List<AgentPolicyResponse>();
            var dummyacounts = GetDummyHierarchies();

            Random rnd = new Random();
            var dummyNames = new List<string> {
            "NABIL FIKRI BIN MOHD AZMAN",
             "AHMAD FAZIRUL MUBIN BIN ISA",
               "SHUKRIYAH BINTI MOHD FADZIL",
             "WAN IMRUL NAZIMI BIN NORDIN",
               "SHARUL NIZAM BIN SENIN",
             "AHMAD FAZIRUL MUBIN BIN ISA",
               "LOKMAN HAKIM BIN KAMALUDIN",
             "NOR ZAHARYL FAIRUZ BIN ZAINAL",
               "EZUL FARHAN BIN ELIAS",
             "NURAISYAH BINTI OTHMAN",
               "JOSHUA A/L FRANCIS",
             "YETTI MURNI BINTI ZAMRIS",
            };

            foreach (var account in dummyacounts)
            {
                result.Add(new AgentPolicyResponse
                {
                    leader_code = account.upline_agent_id,
                    leader_name = account.upline_agent_name,
                    certificate_no = rnd.Next().ToString(),
                    selling_agent_code = account.agent_id,
                    selling_agent_name = account.agent_name,
                    life_assured = dummyNames[rnd.Next(10)],
                    certificate_status = "In Force",
                    pay_mode = "Monthly",
                    product_name = FWDProduct.FutureFirst,
                    term = rnd.Next(50).ToString(),
                    issue_date = new DateTime(2022, 01, 28),
                    due_date = new DateTime(2022, 08, 28),
                    payment_method_code = "Direct Billing (Cash)",
                    contribution_amount = 46,
                });
                result.Add(new AgentPolicyResponse
                {
                    leader_code = account.upline_agent_id,
                    leader_name = account.upline_agent_name,
                    certificate_no = rnd.Next().ToString(),
                    selling_agent_code = account.agent_id,
                    selling_agent_name = account.agent_name,
                    life_assured = dummyNames[rnd.Next(10)],
                    certificate_status = "In Force",
                    pay_mode = "Monthly",
                    product_name = FWDProduct.FutureFirst,
                    term = rnd.Next(50).ToString(),
                    issue_date = new DateTime(2022, 02, 09),
                    due_date = new DateTime(2022, 08, 09),
                    payment_method_code = "Biro Angkasa",
                    contribution_amount = 300,
                });
                result.Add(new AgentPolicyResponse
                {
                    leader_code = account.upline_agent_id,
                    leader_name = account.upline_agent_name,
                    certificate_no = rnd.Next().ToString(),
                    selling_agent_code = account.agent_id,
                    selling_agent_name = account.agent_name,
                    life_assured = dummyNames[rnd.Next(10)],
                    certificate_status = "In Force",
                    pay_mode = "Monthly",
                    product_name = FWDProduct.FutureFirst,
                    term = rnd.Next(50).ToString(),
                    issue_date = new DateTime(2022, 02, 27),
                    due_date = new DateTime(2022, 07, 27),
                    payment_method_code = "Credit Card",
                    contribution_amount = 46,
                });
                result.Add(new AgentPolicyResponse
                {
                    leader_code = account.upline_agent_id,
                    leader_name = account.upline_agent_name,
                    certificate_no = rnd.Next().ToString(),
                    selling_agent_code = account.agent_id,
                    selling_agent_name = account.agent_name,
                    life_assured = dummyNames[rnd.Next(10)],
                    certificate_status = "In Force",
                    pay_mode = "Monthly",
                    product_name = FWDProduct.FutureFirst,
                    term = rnd.Next(50).ToString(),
                    issue_date = new DateTime(2022, 03, 25),
                    due_date = new DateTime(2022, 08, 18),
                    payment_method_code = "Credit Card",
                    contribution_amount = 178.97,
                });
                result.Add(new AgentPolicyResponse
                {
                    leader_code = account.upline_agent_id,
                    leader_name = account.upline_agent_name,
                    certificate_no = rnd.Next().ToString(),
                    selling_agent_code = account.agent_id,
                    selling_agent_name = account.agent_name,
                    life_assured = dummyNames[rnd.Next(10)],
                    certificate_status = "In Force",
                    pay_mode = "Monthly",
                    product_name = FWDProduct.FutureFirst,
                    term = rnd.Next(50).ToString(),
                    issue_date = new DateTime(2022, 01, 28),
                    due_date = new DateTime(2022, 08, 28),
                    payment_method_code = "Direct Billing (Cash)",
                    contribution_amount = 46,
                });
                result.Add(new AgentPolicyResponse
                {
                    leader_code = account.upline_agent_id,
                    leader_name = account.upline_agent_name,
                    certificate_no = rnd.Next().ToString(),
                    selling_agent_code = account.agent_id,
                    selling_agent_name = account.agent_name,
                    life_assured = dummyNames[rnd.Next(10)],
                    certificate_status = "In Force",
                    pay_mode = "Monthly",
                    product_name = FWDProduct.FutureFirst,
                    term = rnd.Next(50).ToString(),
                    issue_date = new DateTime(2022, 01, 28),
                    due_date = new DateTime(2022, 08, 28),
                    payment_method_code = "Direct Billing (Cash)",
                    contribution_amount = 46,
                });
                result.Add(new AgentPolicyResponse
                {
                    leader_code = account.upline_agent_id,
                    leader_name = account.upline_agent_name,
                    certificate_no = rnd.Next().ToString(),
                    selling_agent_code = account.agent_id,
                    selling_agent_name = account.agent_name,
                    life_assured = dummyNames[rnd.Next(10)],
                    certificate_status = "In Force",
                    pay_mode = "Monthly",
                    product_name = FWDProduct.FutureFirst,
                    term = rnd.Next(50).ToString(),
                    issue_date = new DateTime(2022, 01, 28),
                    due_date = new DateTime(2022, 08, 28),
                    payment_method_code = "Direct Billing (Cash)",
                    contribution_amount = 46,
                });
                result.Add(new AgentPolicyResponse
                {
                    leader_code = account.upline_agent_id,
                    leader_name = account.upline_agent_name,
                    certificate_no = rnd.Next().ToString(),
                    selling_agent_code = account.agent_id,
                    selling_agent_name = account.agent_name,
                    life_assured = dummyNames[rnd.Next(10)],
                    certificate_status = "In Force",
                    pay_mode = "Monthly",
                    product_name = FWDProduct.FutureFirst,
                    term = rnd.Next(50).ToString(),
                    issue_date = new DateTime(2022, 01, 28),
                    due_date = new DateTime(2022, 08, 28),
                    payment_method_code = "Direct Billing (Cash)",
                    contribution_amount = 46,
                });
                result.Add(new AgentPolicyResponse
                {
                    leader_code = account.upline_agent_id,
                    leader_name = account.upline_agent_name,
                    certificate_no = rnd.Next().ToString(),
                    selling_agent_code = account.agent_id,
                    selling_agent_name = account.agent_name,
                    life_assured = dummyNames[rnd.Next(10)],
                    certificate_status = "In Force",
                    pay_mode = "Monthly",
                    product_name = FWDProduct.FutureFirst,
                    term = rnd.Next(50).ToString(),
                    issue_date = new DateTime(2022, 01, 28),
                    due_date = new DateTime(2022, 08, 28),
                    payment_method_code = "Direct Billing (Cash)",
                    contribution_amount = 46,
                });
                result.Add(new AgentPolicyResponse
                {
                    leader_code = account.upline_agent_id,
                    leader_name = account.upline_agent_name,
                    certificate_no = rnd.Next().ToString(),
                    selling_agent_code = account.agent_id,
                    selling_agent_name = account.agent_name,
                    life_assured = dummyNames[rnd.Next(10)],
                    certificate_status = "In Force",
                    pay_mode = "Monthly",
                    product_name = FWDProduct.FutureFirst,
                    term = rnd.Next(50).ToString(),
                    issue_date = new DateTime(2022, 01, 28),
                    due_date = new DateTime(2022, 08, 28),
                    payment_method_code = "Direct Billing (Cash)",
                    contribution_amount = 46,
                });
                result.Add(new AgentPolicyResponse
                {
                    leader_code = account.upline_agent_id,
                    leader_name = account.upline_agent_name,
                    certificate_no = rnd.Next().ToString(),
                    selling_agent_code = account.agent_id,
                    selling_agent_name = account.agent_name,
                    life_assured = dummyNames[rnd.Next(10)],
                    certificate_status = "In Force",
                    pay_mode = "Monthly",
                    product_name = FWDProduct.FutureFirst,
                    term = rnd.Next(50).ToString(),
                    issue_date = new DateTime(2022, 01, 28),
                    due_date = new DateTime(2022, 08, 28),
                    payment_method_code = "Direct Billing (Cash)",
                    contribution_amount = 46,
                });
            }

            return result;
        }
        // GET: Persistency
        public async Task<ActionResult> Index()
        {

            var agentid = UserHelper.GetLoginUser();
            var list = new List<AgentHierarchyResponse>();
            var name = "";
            if (TempData["Agents"] != null && !string.IsNullOrEmpty(agentid))
            {
                list = (List<AgentHierarchyResponse>)TempData["Agents"];
                var agent = list.Where(x => x.agent_id == agentid).FirstOrDefault();
                name = agent == null ? "" : agent.agent_name;
            }
            // var name = TempData["PersistencyName"];

            var user = UserHelper.GetLoginUserViewModel();
            var username = string.IsNullOrEmpty(agentid) ? user.Username : agentid;
            var fullname = string.IsNullOrEmpty(agentid) ? user.Fullname : name;

            var startDate = new DateTime(DateTime.Now.Year, 1, 1);
            var endDate = new DateTime(DateTime.Now.Year, 12, 31);
            var date = DateTime.Now.AddMonths(-2);
            var persistencyDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            if (TempData["SearchPersistency"] != null)
            {
                var searchVM = (PersistencyCalculatorViewModel)TempData["SearchPersistency"];
                startDate = searchVM.StartDate;
                endDate = searchVM.EndDate;
                username = searchVM.AgentId;
                fullname = searchVM.AgentName;
                persistencyDate = searchVM.PersistencyDate;
            }

            // if dummy , end generation = 2 
            var dummyAccounts = GetDummyHierarchies();
            var dummyList = dummyAccounts.Select(x => x.agent_id).ToList();
            var currentLogin = UserHelper.GetLoginUser();
            var isDummy = dummyList.Contains(currentLogin);
            List<AgentPolicyResponse> policies = new List<AgentPolicyResponse>();

            if (isDummy)
                policies = GetDummyPolicies();
            else
                policies = await GetAgentPolicies(username, startDate, endDate);

            Session["policies"] = policies;

            var persistencyCalculator = new PersistencyCalculatorViewModel
            {
                AgentId = username,
                AgentName = fullname,
                StartDate = startDate,
                EndDate = endDate,
                AgentPolicies = policies,
                PersistencyDate = persistencyDate

            };

          
            var hierarchyPolicies = new List<AgentHierarchyPolicy>();


            if (isDummy)
            {
                for (int i = 0; i <= 2; i++)
                {
                    hierarchyPolicies = await GetHierarchyPolicies(persistencyCalculator, dummyAccounts.Where(x=>x.generation==i).ToList());
                    persistencyCalculator.HierarchyPolicies.Add(new GenerationGroupPolicy
                    {
                        AgentHierarchyPolicies = hierarchyPolicies,
                        Generation = i,

                    });
                }
 
            }

            int startGeneration = 0;
            var generationHierarchyReq= new AgentHierarchyRequest { agent_id = agentid, generation = startGeneration.ToString() };
            var generationHierarchyRes = await _one2oneAPIHelper.PostAsync<AgentHierarchyRequest, One2OneResponse<AgentHierarchyResponse>>(generationHierarchyReq, "/edfwebapi/alc/agenthierarchy");
            var generationHierarchies = (generationHierarchyRes == null || generationHierarchyRes.data == null) ? new List<AgentHierarchyResponse>() : generationHierarchyRes.data.Where(x=>x.role=="leader"|| x.agent_id== username).ToList();
          

            while (generationHierarchies.Count > 0)
            {
                hierarchyPolicies = await GetHierarchyPolicies(persistencyCalculator, generationHierarchies);
                persistencyCalculator.HierarchyPolicies.Add(new GenerationGroupPolicy
                {
                    AgentHierarchyPolicies = hierarchyPolicies,
                    Generation = startGeneration,

                });
                startGeneration++;

                generationHierarchyReq = new AgentHierarchyRequest { agent_id = agentid, generation = startGeneration.ToString() };
                generationHierarchyRes = await _one2oneAPIHelper.PostAsync<AgentHierarchyRequest, One2OneResponse<AgentHierarchyResponse>>(generationHierarchyReq, "/edfwebapi/alc/agenthierarchy");
                generationHierarchies = (generationHierarchyRes == null || generationHierarchyRes.data == null) ? new List<AgentHierarchyResponse>() : generationHierarchyRes.data.Where(x => x.role == "leader").ToList();
            }

            // persistencyCalculator.DirectGroups = directGroups;
            return View(persistencyCalculator);
        }

        private async Task<List<AgentHierarchyPolicy>> GetHierarchyPolicies(PersistencyCalculatorViewModel persistencyCalculator, List<AgentHierarchyResponse> agentHierachies,bool isPersonal=false)
        {
            var hierarchyPolicies = new List<AgentHierarchyPolicy>();
            foreach (var agent in agentHierachies)
            {
                var agentpolicies = new List<AgentPolicyResponse>();
                if (isPersonal)
                {
                    agentpolicies = persistencyCalculator.AgentPolicies.Where(x=>  x.selling_agent_code == agent.agent_id).ToList(); //await GetAgentPolicies(agent.agent_id, persistencyCalculator., endDate);
                }
                else
                {
                    agentpolicies = persistencyCalculator.AgentPolicies.Where(x => x.leader_code == agent.agent_id || x.selling_agent_code == agent.agent_id).ToList(); //await GetAgentPolicies(agent.agent_id, persistencyCalculator., endDate);
                }
                hierarchyPolicies.Add(new AgentHierarchyPolicy
                {
                    Agent = agent,
                    Policies = agentpolicies,
                    EndDate = persistencyCalculator.PersistencyDate
                });
            }
            return hierarchyPolicies;
        }

        private async Task<List<AgentPolicyResponse>> GetAgentPolicies(string username, DateTime startDate, DateTime endDate)
        {
            var dummyList = GetDummyHierarchies().Select(x => x.agent_id).ToList();
            var currentLogin = UserHelper.GetLoginUser();

            var isDummy = dummyList.Contains(currentLogin);

            if (isDummy)
            {
                return GetDummyPolicies();
            }
            else
            {
                var policyReq = new AgentPolicyRequest { date_from = startDate, date_to = endDate, agent_id = username };
                var policyRes = await _one2oneAPIHelper.PostAsync<AgentPolicyRequest, One2OneResponse<AgentPolicyResponse>>(policyReq, "/edfwebapi/alc/AgentPolicyData");

                var policies = (policyRes == null || policyRes.data == null) ? new List<AgentPolicyResponse>() : policyRes.data;
                return policies;
            }
        }

        public ActionResult test()
        {
            return View();
        }
        public async Task<ActionResult> Calculator(string agentid)
        {

            var list = new List<AgentHierarchyResponse>();
            var name = "";
            if (TempData["Agents"] != null && !string.IsNullOrEmpty(agentid))
            {
                list = (List<AgentHierarchyResponse>)TempData["Agents"];
                var agent = list.Where(x => x.agent_id == agentid).FirstOrDefault();
                name = agent == null ? "" : agent.agent_name;
            }
            // var name = TempData["PersistencyName"];

            var user = UserHelper.GetLoginUserViewModel();
            var username = string.IsNullOrEmpty(agentid) ? user.Username : agentid;
            var fullname = string.IsNullOrEmpty(agentid) ? user.Fullname : name;

            var startDate = new DateTime(DateTime.Now.Year, 1, 1);
            var endDate = DateTime.Now;
            var date = DateTime.Now.AddMonths(-2);
            var persistencyDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            var type = "";
            double targetRatio = 0;
            if (TempData["SearchPersistency"] != null)
            {
                var searchVM = (PersistencyCalculatorViewModel)TempData["SearchPersistency"];
                startDate = searchVM.StartDate;
                endDate = searchVM.EndDate;
                username = searchVM.AgentId;
                fullname = searchVM.AgentName;
                persistencyDate = searchVM.PersistencyDate;
                type = searchVM.Type;
                targetRatio = searchVM.TargetRatio;
            }


         

         
            var policies =      new List<AgentPolicyResponse>();
            // if dummy , end generation = 2 
            var dummyAccounts = GetDummyHierarchies();
            var dummyList = dummyAccounts.Select(x => x.agent_id).ToList();
            var hierarchyPolicies = new List<AgentHierarchyPolicy>();

            var isDummy = dummyList.Contains(username);
            if (isDummy)
            {
                policies = GetDummyPolicies();
            }
            else
            {
                var policyReq = new AgentPolicyRequest { date_from = startDate, date_to = endDate, agent_id = user.Username };
                var policyRes = await _one2oneAPIHelper.PostAsync<AgentPolicyRequest, One2OneResponse<AgentPolicyResponse>>(policyReq, "/edfwebapi/alc/AgentPolicyData");
                policies = (policyRes == null || policyRes.data == null) ? new List<AgentPolicyResponse>() : policyRes.data;
            }

            if (type == "g")
                policies = policies.Where(x => x.leader_code == username || x.selling_agent_code == username).ToList();//.Where(x => x.type.ToLower() == "u" || x.type.ToLower() == "p").ToList();
            else if (type == "p")
                    policies = policies.Where(x=>  x.selling_agent_code==username).ToList();
                  
            
            var persistencyCalculator = new PersistencyCalculatorViewModel
            {
                AgentId = username,
                AgentName = fullname,
                StartDate = startDate,
                EndDate = endDate,
                AgentPolicies = policies,
                PersistencyDate =persistencyDate,
                TargetRatio=targetRatio
            };

         
            TempData["SearchPersistency"] = persistencyCalculator;


            return View(persistencyCalculator);
        }

        [HttpPost]
        public async Task<ActionResult> Search(PersistencyCalculatorViewModel req)
        {
            TempData["SearchPersistency"] = req;
            return Json(req);
        }

        [HttpPost]
        public async Task<ActionResult> SearchAgentPolicy(PersistencyCalculatorViewModel req)
        {
            if (Session["policies"] != null)
                req.AgentPolicies = (List<AgentPolicyResponse>)Session["policies"];
            else
                req.AgentPolicies = await GetAgentPolicies(req.AgentId, req.StartDate, req.EndDate);




            // if dummy , end generation = 2 
            var dummyAccounts = GetDummyHierarchies();
            var dummyList = dummyAccounts.Select(x => x.agent_id).ToList();
            var hierarchyPolicies = new List<AgentHierarchyPolicy>();

            var isDummy = dummyList.Contains(req.AgentId);
            if (isDummy)
            {
                var generationHierarchies = GetDummyHierarchies().Where(x => x.agent_id == req.AgentId || x.upline_agent_id == req.AgentId).ToList();
                hierarchyPolicies = await GetHierarchyPolicies(req, generationHierarchies, true);

            }
            else
            {
                var generationHierarchyReq = new AgentHierarchyRequest { agent_id = req.AgentId, generation = "0" };
                var generationHierarchyRes = await _one2oneAPIHelper.PostAsync<AgentHierarchyRequest, One2OneResponse<AgentHierarchyResponse>>(generationHierarchyReq, "/edfwebapi/alc/agenthierarchy");
                var generationHierarchies = (generationHierarchyRes == null || generationHierarchyRes.data == null) ? new List<AgentHierarchyResponse>() : generationHierarchyRes.data.Where(x => x.role == "agent" || x.agent_id == req.AgentId).ToList();
                hierarchyPolicies = await GetHierarchyPolicies(req, generationHierarchies, true);
            }
            var generation = new GenerationGroupPolicy
            {
                LeaderId = req.AgentId,
                AgentHierarchyPolicies = hierarchyPolicies,
                Generation= req.Generation
            };
            return PartialView("~/Views/Persistency/_AgentPolicySummary.cshtml", generation);
        }
    }
}