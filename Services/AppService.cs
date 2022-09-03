using LoginFlowInMauiBlazorApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginFlowInMauiBlazorApp.Services
{
    public class AppService : IAppService
    {
        private string _baseUrl = "http://192.168.0.185:8188";
        public async Task<MainResponse> AuthenticateUser(LoginModel loginModel)
        {
            var returnResponse = new MainResponse();
            using (var client = new HttpClient())
            {
                var url = $"{_baseUrl}{APIs.AuthenticateUser}";

                var serializedStr = JsonConvert.SerializeObject(loginModel);

                var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string contentStr = await response.Content.ReadAsStringAsync();
                    returnResponse = JsonConvert.DeserializeObject<MainResponse>(contentStr);
                }
            }
            return returnResponse;
        }

        public async Task<List<StudentModel>> GetAllStudents()
        {
            var returnResponse = new List<StudentModel>();
            using (var client = new HttpClient())
            {
                var url = $"{_baseUrl}{APIs.GetAllStudents}";

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Setting.UserBasicDetail?.AccessToken}");
                var response = await client.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    bool isTokenRefreshed = await RefreshToken();
                    if (isTokenRefreshed) return await GetAllStudents();
                }
                else
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string contentStr = await response.Content.ReadAsStringAsync();
                        var mainResponse = JsonConvert.DeserializeObject<MainResponse>(contentStr);
                        if (mainResponse.IsSuccess)
                        {
                            returnResponse = JsonConvert.DeserializeObject<List<StudentModel>>(mainResponse.Content.ToString());
                        }
                    }
                }

            }
            return returnResponse;
        }

        public async Task<bool> RefreshToken()
        {
            bool isTokenRefreshed = false;
            using (var client = new HttpClient())
            {
                var url = $"{_baseUrl}{APIs.RefreshToken}";

                var serializedStr = JsonConvert.SerializeObject(new AuthenticateRequestAndResponse
                {
                    RefreshToken = Setting.UserBasicDetail.RefreshToken,
                    AccessToken = Setting.UserBasicDetail.AccessToken
                });

                var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string contentStr = await response.Content.ReadAsStringAsync();
                    var mainResponse = JsonConvert.DeserializeObject<MainResponse>(contentStr);
                    if (mainResponse.IsSuccess)
                    {
                        var tokenDetails = JsonConvert.DeserializeObject<AuthenticateRequestAndResponse>(mainResponse.Content.ToString());
                        Setting.UserBasicDetail.AccessToken = tokenDetails.AccessToken;
                        Setting.UserBasicDetail.RefreshToken = tokenDetails.RefreshToken;

                        string userDetailsStr = JsonConvert.SerializeObject(Setting.UserBasicDetail);
                        await SecureStorage.SetAsync(nameof(Setting.UserBasicDetail), userDetailsStr);
                        isTokenRefreshed = true;
                    }
                }
            }
            return isTokenRefreshed;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> RegisterUser(RegistrationModel registerUser)
        {
            string errorMessage = string.Empty;
            bool isSuccess = false;
            using (var client = new HttpClient())
            {
                var url = $"{_baseUrl}{APIs.RegisterUser}";

                var serializedStr = JsonConvert.SerializeObject(registerUser);
                var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    isSuccess = true;
                }
                else
                {
                    errorMessage = await response.Content.ReadAsStringAsync();
                }
            }
            return (isSuccess, errorMessage);
        }
    }
}
