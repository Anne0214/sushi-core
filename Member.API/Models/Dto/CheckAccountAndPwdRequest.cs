namespace Member.API.Models.Dto
{
    public record CheckAccountAndPwdRequest
    {
        public string Account { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
