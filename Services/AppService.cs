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
        private string _baseUrl = "http://192.168.0.185";
        public async Task<string> AuthenticateUser(LoginModel loginModel)
        {
            string returnStr = string.Empty;
            using (var client = new HttpClient())
            {
                var url = $"{_baseUrl}{APIs.AuthenticateUser}";

                var serializedStr = JsonConvert.SerializeObject(loginModel);

                var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    returnStr = await response.Content.ReadAsStringAsync();
                }
            }
            return returnStr;
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
