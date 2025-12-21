using Microsoft.AspNetCore.Mvc;

namespace Member.API.Models.Dto
{
    public class CheckAccountAndPwdRequest
    {
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;


    }
}
