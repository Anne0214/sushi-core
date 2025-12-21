using ApiGateway.Application.Abstractions;
using ApiGateway.Application.UseCase;
using APP.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace APP.API.Controllers
{
    public class UserController : Controller
    {
        private readonly ILoginUseCase _loginUseCase;
        private readonly IConfiguration _config;
        public UserController(ILoginUseCase loginUseCase, IConfiguration config)
        {
            _loginUseCase = loginUseCase;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginReq req)
        {
            // 參數驗證
            if (req.Account is null || req.Account.Length < 6 || req.Password is null || req.Password.Length < 8)
                return BadRequest("Invalid account or password format.");

            var gatewayName = _config["GatewayName"];
            var tokenLifeHour = int.TryParse(_config["TokenLifeHour"], out var hour) ? hour : 1;

            var result = await _loginUseCase.ExecuteAsync(req.Account, req.Password, gatewayName, tokenLifeHour);

            if (!result.IsSuccess)
                return BadRequest(result.Error?.ToString() ?? "Login failed.");

            return Ok(new LoginRes(result.Value.AccessToken, result.Value.RefreshToken));
        }

        public IActionResult Logout()
        {
            return View();
        }
    }
}
