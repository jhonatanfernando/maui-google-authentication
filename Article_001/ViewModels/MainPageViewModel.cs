using Article_001.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Article_001.ViewModels
{
    public class MainPageViewModel
    {
        public Command LoginCommand
                => new Command(async () =>
                {
                    var authUrl = $"{Constants.Google.auth_uri}?response_type=code" +
                    $"&redirect_uri=com.maui.login://" +
                    $"&client_id={Constants.Google.client_id}" +
                    $"&scope=https://www.googleapis.com/auth/userinfo.email" +
                    $"&include_granted_scopes=true" +
                    $"&state=state_parameter_passthrough_value";


                    var callbackUrl = "com.maui.login://";

                    try
                    {
                        var response = await WebAuthenticator.AuthenticateAsync(new WebAuthenticatorOptions()
                        {
                            Url = new Uri(authUrl),
                            CallbackUrl = new Uri(callbackUrl)
                        });

                        var codeToken = response.Properties["code"];

                        var parameters = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string,string>("grant_type","authorization_code"),
                        new KeyValuePair<string,string>("client_id",Constants.Google.client_id),
                        new KeyValuePair<string,string>("redirect_uri",callbackUrl),
                        new KeyValuePair<string,string>("code",codeToken),
                    });


                        HttpClient client = new HttpClient();
                        var accessTokenResponse = await client.PostAsync(Constants.Google.token_ur, parameters);

                        LoginResponse loginResponse;

                        if (accessTokenResponse.IsSuccessStatusCode)
                        {
                            var data = await accessTokenResponse.Content.ReadAsStringAsync();

                            loginResponse = JsonConvert.DeserializeObject<LoginResponse>(data);
                        }
                    }
                    catch (TaskCanceledException e)
                    {
                        // Use stopped auth
                    }


                });

    }
}
