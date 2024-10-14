using ArmyGrievances.Generics;
using ArmyGrievances.Models;
using ArmyGrievances.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ArmyGrievances.Controllers
{
    public class ESMController : Controller
    {
        private readonly ILogger<ESMController> _logger;
        public readonly ICommonGenericFunction _commonGeneric;
        public readonly IOperationRepository _operationRepository;
        IConfiguration _configuration;
        public ESMController(ILogger<ESMController> logger, ICommonGenericFunction commonGeneric, IConfiguration configuration, IOperationRepository operationRepository)
        {
            _logger = logger;
            _commonGeneric = commonGeneric;
            _configuration = configuration; 
            _operationRepository = operationRepository;
        }
        public async Task<IActionResult> Records(IndividualModal individual, int excel = 0)
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return Redirect("~/Home");
            }
            individual = await _operationRepository.SP_FetchIndividualRecords(individual, _configuration);
            string reportname = "ESM_Excel.xlsx";
            if (excel == 1)
            {
                List<IndividualExcel>? excelStatusReports = individual?.individuals?.Select(e => new IndividualExcel { S_No = e.S_No, Army_No = e.Army_No, Rank = e.Rank, Name = e.Name, Address = e.Address, Mobile_No = e.Mobile_No, Identity_Card_No = e.IdentityCardNo }).ToList();

                if (excelStatusReports?.Count > 0)
                {
                    var exportbytes = _commonGeneric.ExporttoExcel<IndividualExcel>(excelStatusReports, reportname);
                    return File(exportbytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
                }
                else
                {
                    TempData["Message"] = "No Data to Export";
                }
            }
            ViewData["Message"] = TempData["Message"];
            return View(individual);
        }
        public IActionResult NewIndividual()
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return Redirect("~/Home");
            }
            ViewBag.PageHeading = "New Individual Record";
            IndividualModal individual = new IndividualModal();
            return View(individual);
        }

        public async Task<IActionResult> EditIndividual(string id)
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return Redirect("~/Home");
            }
            ViewBag.PageHeading = "Edit Individual Record";
            IndividualModal individual = new IndividualModal();
            individual.Id = Convert.ToInt32(id);
            var response = await _operationRepository.SP_FetchIndividualRecords(individual, _configuration);
            individual = response.individuals[0];
            return View("~/Views/ESM/NewIndividual.cshtml", individual);
        }
        public async Task<IActionResult> MngSubmit(IndividualModal individual)
        {
            individual.Action = individual.Id > 0 ? 2 : 1;
            StatusCode status = new StatusCode();
            status = await _operationRepository.SP_IndividualManagement(individual, _configuration);
            TempData["Message"] = status.ErrorMsg;
            return Redirect("~/ESM/Records");
        }
        public async Task<JsonResult> EligibleCertificate(string id)
        {
            List<ElegibleCertificate> ilist = new List<ElegibleCertificate>();
            var response = await _operationRepository.SP_FetchEligibleCertificate(id,_configuration);
            var result = response;
            if(result.Count  == 0) { result = null; }
            return Json(result);
        }
        public async Task<JsonResult> checkExistArmyNo(string ArmyNo)
        {
            //StatusCode status = new StatusCode();
            var response = await _operationRepository.SP_CheckExistArmyNo(ArmyNo, _configuration);
            var result = response.Status;
            return Json(result);
        }
        public async Task<JsonResult> ECSubmit(ElegibleCertificate elegibleCertificate)
        {
            //StatusCode status = new StatusCode();
            var response = await _operationRepository.SP_CertificateManagement(elegibleCertificate, _configuration);
            var result = response.Status;
            return Json(result);
        }
        public async Task<IActionResult> ImportFromExcel(IFormFile file, string Action,int Sheet)
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return Redirect("~/Home");
            }
            int id = Convert.ToInt32(HttpContext.Session.GetInt32("Id"));
            StatusCode status = new StatusCode();
            ImportExcelFile importExcel = new ImportExcelFile();
            if (file != null && file.Length > 0)
            {
                string JsonString = _commonGeneric.ParseExcelFile(file, Action, Sheet);
                string[] ArrJS = JsonString.Split(',');
                if (ArrJS[0] == "ErrorMessage")
                {
                    TempData["Message"] = ArrJS[1];
                }
                else
                {
                    importExcel.JsonString = JsonString;
                    importExcel.Action = Action;
                    try
                    {
                        status = await _operationRepository.SP_ExcelManagement(importExcel, _configuration);
                        importExcel.Status = status;
                        TempData["Message"] = importExcel.Status?.ErrorMsg;
                    }
                    catch (Exception e)
                    {
                        TempData["Message"] = e.Message;
                    }

                }
            }
            else { TempData["Message"] = "No file attached"; }
            return Redirect("~/ESM/Records");
        }
        public async Task<JsonResult> DeleteRecords(string ids)
        {
            var result = "";
            string[] arrIds = ids.Split(',');
            try
            {
                StatusCode statusCode = new StatusCode();
                statusCode = await _operationRepository.SP_DeleteRecords(ids, 3, _configuration);
                result = statusCode.Status;
                if (result == "true")
                {
                    string mess = arrIds.Length == 1 ? "record is" : "records are";
                    TempData["Message"] = "Selected " + mess + " deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                TempData["Message"] = ex;
            }
            return Json(result);
        }
    }
}
