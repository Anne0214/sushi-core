using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Abstractions;
using ApiGateway.Application.Abstractions.External;
using ApiGateway.Application.Auth;
using ApiGateway.Domain.Auth;
using Microsoft.IdentityModel.JsonWebTokens;
using Sushi.All.Infrastructure.Result;

namespace ApiGateway.Application.UseCase
{
    public record LoginRes
    (
        string AccessToken,
        string RefreshToken
    )
    { }
    public class LoginUseCase : ILoginUseCase
    {
        private readonly IMemberServiceClient _memberService;
        private readonly JwtUtil _jwtUtil;
        private readonly RefreshTokenUtil _refreshTokenUtil;

        public LoginUseCase(IMemberServiceClient memberService, JwtUtil jwtUtil, RefreshTokenUtil refreshTokenUtil) => (_memberService, _jwtUtil, _refreshTokenUtil) = (memberService, jwtUtil, refreshTokenUtil);

        public async Task<Result<LoginRes>> ExecuteAsync(string account, string password, string gatewayName, int lifeTimeHour)
        {
            string accessToken = string.Empty;
            string refreshToken = string.Empty;
            try
            {
                var userId = await _memberService.CheckAccountAndPwd(
                    new DTO.MemberServiceClient.CheckAccountAndPwdRequestDto
                    {
                        Account = account,
                        Password = password
                    });


                if (userId > 0)
                {
                    var claim = new JwtClaim(userId);
                    var lifeTimeSpan = new TimeSpan(lifeTimeHour, 0, 0);
                    accessToken = _jwtUtil.GenerateJwtToken(gatewayName, claim, lifeTimeSpan);
                    var getRefreshTokenResult = await _refreshTokenUtil.GenerateRefreshToken(userId);
                    refreshToken = getRefreshTokenResult.Value;
                }
                else
                {
                    return Result<LoginRes>.Fail(new Error("LoginUseCase.InvalidAccountOrPassword", "Account or password is invalid.", ErrorType.Validation));
                }
            }
            catch (Exception ex)
            {
                return Result<LoginRes>.Fail(new Error("LoginUseCase.Exception", ex.Message, ErrorType.Unexpected));
            }


            return Result<LoginRes>.Ok(new LoginRes(accessToken, refreshToken));

        }
    }
}
