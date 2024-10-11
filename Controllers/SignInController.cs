using ArmyGrievances.Generics;
using ArmyGrievances.Models;
using ArmyGrievances.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DevStatusManager.Controllers
{
    public class SignInController : Controller
    {
        private readonly ILogger<SignInController> _logger;
        public readonly ICommonGenericFunction _commonGeneric;
        public readonly IOperationRepository _operationRepository;
        IConfiguration _configuration;
        public SignInController(ILogger<SignInController> logger, ICommonGenericFunction commonGeneric, IConfiguration configuration, IOperationRepository operationRepository)
        {
            _logger = logger;
            _commonGeneric = commonGeneric;
            _configuration = configuration;
            _operationRepository = operationRepository;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult SignIn(int isForce = 0)
        {
            UserModel userModel = new UserModel();
            HttpContext.Session.SetString("User", "");
            HttpContext.Session.SetInt32("IsForce", isForce);
            ViewData["Message"] = TempData.Peek("Message");
            ViewData["Emptylocal"] = TempData["Message"];
            return View(userModel);

        }
        [HttpPost]
        public async Task<IActionResult> LoginSubmit(UserModel model)
        {
            model = await _operationRepository.SP_ValidateUser(model?.UserName,_configuration);
            if (model?.StatusCode?.Status == "not ok")
            {
                TempData["Message"] = model.StatusCode.ErrorMsg;
                return Redirect("~/SignIn");
            }
          
            HttpContext.Session.SetInt32("Id", model.id);
            HttpContext.Session.SetString("User", model.UserName);
            HttpContext.Session.SetInt32("IsAdmin", model.IsAdmin == true ? 1 : 0);
            //HttpContext.Session.SetInt32("IsGlobalAdmin", model.GlobalAdmin == true ? 1 : 0);
            //HttpContext.Session.SetInt32("IsSuperAdmin", model.SuperAdmin == true ? 1 : 0);
            return Redirect("~/Home/Dashboard");
        }
        public JsonResult checkExistMobileNo(string mobileNo)
        {
            var response = _commonGeneric.ExecuteGetAPI<StatusCode>("ChkExtMobile", mobileNo);
            response.Wait();
            var result = response.Result?.Status;
            return Json(result);
        }
       
    }
}
