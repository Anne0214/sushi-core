using Member.API.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Member.API.Controllers
{
    [ApiController]
    public class LogginController : Controller
    {
        [HttpPost]
        [Route("~/api/CheckAccountAndPwd")]
        public IActionResult CheckAccountAndPwd([FromBody] CheckAccountAndPwdRequest req)
        {
            return Json(new { UserID = 10003});
        }
    }
}
