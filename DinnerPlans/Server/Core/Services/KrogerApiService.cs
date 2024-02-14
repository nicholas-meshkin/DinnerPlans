using DinnerPlans.Server.Core.IServices;
using DinnerPlans.Server.Core.Util;
using DinnerPlans.Server.Persistence.Entities;
using DinnerPlans.Shared.DTOs;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace DinnerPlans.Server.Core.Services
{
    public class KrogerApiService : IKrogerApiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IAppService _appService;

        public KrogerApiService(IUnitOfWork unitOfWork, IConfiguration config, IAppService appService)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _appService = appService;
        }

        public async Task<KrogerProductSearchResponseDto> ProductSearch(KrogerProductSearchRequestDto searchDto)
        {
            try
            {
                //TODO move validation outside of this method
                //TODO probably will want to return something besides just the deserialized json

                //need either search term or product Id
                if (string.IsNullOrEmpty(searchDto.ProductId) && string.IsNullOrEmpty(searchDto.Query)) return null;

                var url = _config.GetSection(AppConstants.KROGER_BASE_URL).Value + "/products?";
                if (!string.IsNullOrEmpty(searchDto.ProductId)) { url += "filter.productId=" + searchDto.ProductId; }
                else
                {
                    searchDto.Query = searchDto.Query.Trim();
                    //query needs to be between 3 and 128(?) chars
                    if(searchDto.Query.Length < 3 || searchDto.Query.Length > 128) return null;

                    var termArray =  searchDto.Query.Split(' ');
                    url += "filter.term=" + termArray[0];
                    if (termArray.Length > 1)
                    {
                        for (int i = 1; i < termArray.Length; i++)
                        {
                            url += "%" + termArray[i];
                        }
                    }

                }
                if (!string.IsNullOrEmpty(searchDto.LocationId)) { url += "&filter.locationId=" + searchDto.LocationId; }
                if (!string.IsNullOrEmpty(searchDto.Brand)) { url += "&filter.brand=" + searchDto.Brand; }
                //TODO validate types of fulfillment if ever use this 
                if (!string.IsNullOrEmpty(searchDto.Fulfillment)) { url += "&filter.fulfillment=" + searchDto.Fulfillment; }
                url += "&filter.limit=50";
                var response = await MakeRequest(url);
                if(!string.IsNullOrEmpty(response))
                {
                    var dto = JsonConvert.DeserializeObject<KrogerProductSearchResponseDto>(response);
                    return dto;
                }
               
                //todo finish

            }
            catch (Exception ex)
            {
                await CreateErrorLog("none", ex, "ProductSearch");
            }

            return null;
        }

        public async Task<LocationSearchResponseDto> LocationSearch(string zip, int radius)
        {
            try
            {
                if (string.IsNullOrEmpty(zip) || radius < 0) return null;
                var url = _config.GetSection(AppConstants.KROGER_BASE_URL).Value 
                    + "/locations?filter.zipCode.near="+ zip 
                    + "&filter.limit=10";
                if (radius != 0) url += "&filter.radiusInMiles=" + radius;

                var response = await MakeRequest(url);
                if (!string.IsNullOrEmpty(response))
                {
                    var dto = JsonConvert.DeserializeObject<LocationSearchResponseDto>(response);
                    return dto;
                }
            }
            catch (Exception ex)
            {
                await CreateErrorLog("none", ex, "LocationSearchResponseDto");
            }
            return null;
        }

        private async Task<string> MakeRequest(string url)
        {
            try
            {
                var client = GetClient();
                var token = await GetTokenForClient(client);
                HttpRequestHeaders headers = client.DefaultRequestHeaders;
                headers.Add("Authorization", "Bearer " + token.AccessToken);
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
                await _appService.CreateErrorLog("Web API call failed for query: " + url + " Reason: " + response.ReasonPhrase, null, "MakeRequest");
            }
            catch (Exception ex)
            {
                await CreateErrorLog("url: " + url, ex, "MakeRequest");
            }
            return null;
        }

        private HttpClient GetClient()
        {
            var url = _config.GetSection(AppConstants.KROGER_BASE_URL).Value;
            var client = new HttpClient
            {
                BaseAddress = new Uri(url),
                Timeout = new TimeSpan(0, 2, 0)
            };
            return client;
        }

        private async Task<TokenDto> GetTokenForClient(HttpClient client)
        {
            try
            {
               //TODO maybe store client and token somewhere else
               //TODO definitely do something about only grabbing new token if current one has expired

                var tokenUrl = _config.GetSection(AppConstants.KROGER_TOKEN_URL).Value;
                var scope = "product.compact";
                string[] scopes = { scope };
                   
                string baseAddress = @tokenUrl;

                string grant_type = "client_credentials";
                string client_id = _config.GetSection(AppConstants.KROGER_CLIENT_ID).Value;
                string client_secret = _config.GetSection(AppConstants.KROGER_CLIENT_SECRET).Value;

                var form = new Dictionary<string, string>
                {
                    {"grant_type", grant_type},
                    {"client_id", client_id},
                    {"client_secret", client_secret},
                    {"scope", scope}
                };

                HttpResponseMessage tokenResponse = await client.PostAsync(baseAddress, new FormUrlEncodedContent(form));
                var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
                TokenDto tok = JsonConvert.DeserializeObject<TokenDto>(jsonContent);
                return tok;

            }
            catch (Exception ex)
            {
                await CreateErrorLog("none", ex, "GetToken");
            }
            return null;
        }

        public async Task<bool> Save<T>(T entity, int id) where T : class
        {
            if (id == 0) await _unitOfWork.WriteRepo.AddEntityAsync(entity);
            else _unitOfWork.WriteRepo.UpdateEntity(entity);

            return await _unitOfWork.Commit();
        }

        public async Task CreateErrorLog(string identifier, Exception? e, string method)
        {
            var existingError = await _unitOfWork.ReadRepo.GetEntityWithPredicate<KrogerErrorLog>(
                a => a.Identifier.Equals(identifier) && a.Method.Equals(method)
                && ((e == null && a.Message == null) || a.Message.Equals(e.Message))
                && ((e == null && a.Stack == null) || a.Stack.Equals(e.StackTrace)));
            if (existingError != null)
            {
                existingError.Count++;
                existingError.UpdatedDate = DateTime.UtcNow;
                await Save(existingError, existingError.Id);
            }
            else
            {
                var error = new KrogerErrorLog
                {
                    CreatedDate = DateTime.UtcNow,
                    Identifier = identifier,
                    Message = e?.Message,
                    Method = method,
                    Stack = e?.StackTrace
                };
                await Save(error, 0);
            }
        }

        public class TokenDto
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
        }

    }
}
