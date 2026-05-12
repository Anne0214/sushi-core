using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Application.Abstractions.External;
using ApiGateway.Application.DTO.MemberServiceClient;

namespace ApiGateway.Infrastructure.Client
{
    public class MemberHttpClient:IMemberServiceClient
    {
        private readonly HttpClient _httpClient;

        public MemberHttpClient(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<long> CheckAccountAndPwd(CheckAccountAndPwdRequestDto request)
        {
            Console.WriteLine("BaseAddress: " + _httpClient.BaseAddress);

            var response = await _httpClient.PostAsJsonAsync("/api/checkAccountAndPwd", request);

            if (!response.IsSuccessStatusCode)
            {
                return 0;
            }
            var result = await response.Content.ReadFromJsonAsync<CheckAccountAndPwdResponseDto>();

            // todo retry, logging
            return result?.UserId??0;
        }
    }
}
