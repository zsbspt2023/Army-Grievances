using ArmyGrievances.Generics;
using ArmyGrievances.Models;
using ArmyGrievances.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ArmyGrievances.Controllers
{
    public class MailIn_OutController : Controller
    {
        private readonly ILogger<MailIn_OutController> _logger;
        public readonly ICommonGenericFunction _commonGeneric;
        public readonly IOperationRepository _operationRepository;
        IConfiguration _configuration;
        public MailIn_OutController(ILogger<MailIn_OutController> logger, ICommonGenericFunction commonGeneric, IConfiguration configuration, IOperationRepository operationRepository)
        {
            _logger = logger;
            _commonGeneric = commonGeneric;
            _configuration = configuration;
            _operationRepository = operationRepository;
        }
        public async Task<IActionResult> Records(MainIn_OutModal mainIn_Out, int excel = 0)
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return Redirect("~/Home");
            }
            mainIn_Out = await _operationRepository.SP_FetchMailInOutRecords(mainIn_Out, _configuration);
            string reportname = "OutMails_Excel.xlsx";
            if (excel == 1)
            {
                List<MainIn_OutExcel>? excelStatusReports = mainIn_Out?.mainIn_Outs?.Select(e => new MainIn_OutExcel
                {
                    S_No = e.S_No,
                    File_Name = e.Item,
                    Mail_Out_Date = e.MailOut_Date,
                    Subject = e.Subject,
                    To_Whom = e.ToWhom,
                    Rank = e.Rank,
                    Service_No = e.Service_No,
                    Name = e.Name,
                    //Letter_Date = e.Letter_Date,
                    Letter_No = e.Letter_No
                }).ToList();

                if (excelStatusReports?.Count > 0)
                {
                    var exportbytes = _commonGeneric.ExporttoExcel<MainIn_OutExcel>(excelStatusReports, reportname);
                    return File(exportbytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
                }
                else
                {
                    TempData["Message"] = "No Data to Export";
                }
            }
            ViewData["Message"] = TempData["Message"];
            ViewBag.PageHeading = "Out Mail Records";
            return View(mainIn_Out);
        }
        public async Task<IActionResult> InRecords(MainIn_OutModal mainIn_Out, int excel = 0)
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return Redirect("~/Home");
            }
            mainIn_Out = await _operationRepository.SP_FetchMailInOutRecords(mainIn_Out, _configuration);
            string reportname = "InMails_Excel.xlsx";
            if (excel == 1)
            {
                List<MainIn_OutExcel>? excelStatusReports = mainIn_Out?.mainIn_Outs?.Select(e => new MainIn_OutExcel
                {
                    S_No = e.S_No,
                    File_Name = e.Item,
                    //Mail_Out_Date = e.MailOut_Date,
                    Subject = e.Subject,
                    To_Whom = e.ToWhom,
                    Rank = e.Rank,
                    Service_No = e.Service_No,
                    Name = e.Name,
                    Letter_Date = e.Letter_Date,
                    Letter_No = e.Letter_No
                }).ToList();

                if (excelStatusReports?.Count > 0)
                {
                    var exportbytes = _commonGeneric.ExporttoExcel<MainIn_OutExcel>(excelStatusReports, reportname);
                    return File(exportbytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
                }
                else
                {
                    TempData["Message"] = "No Data to Export";
                }
            }
            ViewData["Message"] = TempData["Message"];
            ViewBag.PageHeading = "In Mail Records";
            return View("~/Views/MailIn_Out/Records.cshtml", mainIn_Out);
        }
        public IActionResult NewMail()
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return Redirect("~/Home");
            }
            ViewBag.PageHeading = "New In/Out Mail Record";
            MainIn_OutModal mainIn_OutModal = new MainIn_OutModal();
            return View(mainIn_OutModal);
        }
        public async Task<IActionResult> Submit(MainIn_OutModal mainIn_Out)
        {
            mainIn_Out.Action = mainIn_Out.Mail_Id > 0 ? 2 : 1;
            StatusCode status = new StatusCode();
            status = await _operationRepository.Sp_MailManagement(mainIn_Out, _configuration);
            TempData["Message"] = status.ErrorMsg;
            return Redirect("~/MailIn_Out/Records");
        }
        public async Task<IActionResult> EditMail(string id)
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return Redirect("~/Home");
            }
            ViewBag.PageHeading = "Edit In/Out Mail Record";
            MainIn_OutModal main = new MainIn_OutModal();
            main.Mail_Id = Convert.ToInt32(id);
            var response = await _operationRepository.SP_FetchMailInOutRecords(main, _configuration);
            main = response.mainIn_Outs[0];
            return View("~/Views/MailIn_Out/NewMail.cshtml", main);
        }
        public async Task<JsonResult> DeleteRecords(string ids)
        {
            var result = "";
            string[] arrIds = ids.Split(',');
            try
            {
                StatusCode statusCode = new StatusCode();
                statusCode = await _operationRepository.SP_DeleteRecords(ids, 2, _configuration);
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
