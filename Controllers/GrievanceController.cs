using ArmyGrievances.Generics;
using ArmyGrievances.Models;
using ArmyGrievances.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ArmyGrievances.Controllers
{
    public class GrievanceController : Controller
    {
        private readonly ILogger<GrievanceController> _logger;
        public readonly ICommonGenericFunction _commonGeneric;
        public readonly IOperationRepository _operationRepository;
        IConfiguration _configuration;
        public GrievanceController(ILogger<GrievanceController> logger, ICommonGenericFunction commonGeneric, IConfiguration configuration, IOperationRepository operationRepository)
        {
            _logger = logger;
            _commonGeneric = commonGeneric;
            _configuration = configuration;
            _operationRepository = operationRepository;
        }
        public async Task<IActionResult> Records(GrievanceModal grievance, int excel = 0)
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return Redirect("~/Home");
            }
            grievance = await _operationRepository.SP_FetchGrievanceRecords(grievance, _configuration);
            string reportname = "Grievance_Excel.xlsx";
            if (excel == 1)
            {
                List<GrievanceExcel>? excelStatusReports = grievance?.grievances?.Select(e => new GrievanceExcel { S_No = e.S_No, Name = e.Name, Army_No = e.ArmyNo, Grievance_Recept_Date = e.Grievance_ReceptDate, Grienvance_Subject = e.Grienvance_Subject, Sent_To_Whom = e.Sent_Area, Regt_Record = e.Regt_Record, ZSB_Memo_No = e.ZSB_MemoNo, ZSB_Memo_Date = e.ZSB_MemoDate }).ToList();

                if (excelStatusReports?.Count > 0)
                {
                    var exportbytes = _commonGeneric.ExporttoExcel<GrievanceExcel>(excelStatusReports, reportname);
                    return File(exportbytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
                }
                else
                {
                    TempData["Message"] = "No Data to Export";
                }
            }
            ViewData["Message"] = TempData["Message"];
            List<SelectionList> selectionList = new List<SelectionList>();
            selectionList = await _operationRepository.SP_GetSelectionList("", _configuration);
            ViewBag.RecordsDropdown = selectionList;
            return View(grievance);
        }
        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile file, string Action, int Sheet)
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
                string JsonString = _commonGeneric.ParseExcelFile(file, Action,Sheet);
                string[] ArrJS = JsonString.Split(',');
                if (ArrJS[0] == "ErrorMessage")
                {
                    TempData["Message"] = ArrJS[1];
                }
                else
                {
                    importExcel.JsonString = JsonString;
                    importExcel.Action = Action;
                    status = await _operationRepository.SP_ExcelManagement(importExcel, _configuration);
                    importExcel.Status = status;
                    TempData["Message"] = importExcel.Status?.ErrorMsg;
                }
            }
            else { TempData["Message"] = "No file attached"; }
            return (Action == "1") ? Redirect("~/Grievance/Records") : Redirect("~/MailIn_Out/Records");
        }

        public async Task<IActionResult> NewGrievance()
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return Redirect("~/Home");
            }
            ViewBag.PageHeading = "New Grievance Record";
            GrievanceModal grievance = new GrievanceModal();
            List<SelectionList> selectionList = new List<SelectionList>();
            selectionList = await _operationRepository.SP_GetSelectionList("", _configuration);
            ViewBag.RecordsDropdown = selectionList;
            return View(grievance);
        }
        public async Task<IActionResult> MngSubmit(GrievanceModal grievanceModal)
        {
            grievanceModal.Action = grievanceModal.Id > 0 ? 2 : 1;
            StatusCode status = new StatusCode();
            status = await _operationRepository.SP_GrievanceManagement(grievanceModal, _configuration);
            TempData["Message"] = status.ErrorMsg;
            return Redirect("~/Grievance/Records");
        }
        public async Task<IActionResult> EditGrievance(string id)
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return Redirect("~/Home");
            }
            ViewBag.PageHeading = "Edit Grievance Record";
            GrievanceModal grievance = new GrievanceModal();
            grievance.Id = Convert.ToInt32(id);
            var response = await _operationRepository.SP_FetchGrievanceRecords(grievance, _configuration);
            grievance = response.grievances[0];
            List<SelectionList> selectionList = new List<SelectionList>();
            selectionList = await _operationRepository.SP_GetSelectionList("", _configuration);
            ViewBag.RecordsDropdown = selectionList;
            return View("~/Views/Grievance/NewGrievance.cshtml", grievance);
        }
        public async Task<JsonResult> DeleteRecords(string ids)
        {
            var result = "";
            string[] arrIds = ids.Split(',');
            try
            {
                StatusCode statusCode = new StatusCode();
                statusCode = await _operationRepository.SP_DeleteRecords(ids, 1, _configuration);
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
