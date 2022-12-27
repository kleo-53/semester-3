namespace AbobusPractice;

using Newtonsoft.Json;
using System;
using System.Diagnostics;

class Program
{
    private static async Task Main(string[] args)
    {
        var httpClient = new HttpClient();

        var userID = "id275764447";
        /*var serviceToken = "81d294b081d294b081d294b04382c3a15e881d281d294b0e28a7ae7ecc451a5ed0fa5bc";
        var field = "bdate";
        var request = $"https://api.vk.com/method/users.get?user_id={userID}&access_token={serviceToken}&v=5.131&fields={field}";

        var response = await httpClient.GetAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            var data = JsonConvert.DeserializeObject<UserInfoGetResponce>(content);
            Console.WriteLine(data.response.First().Bdate);
            Console.WriteLine(data.response.First().FirstName);
        }*/
        var clientId = "51459566";
        var redirectUri = "https://oauth.vk.com/blank.html";
        var authString = $"https://oauth.vk.com/authorize?client_id={clientId}&display=page&redirect_uri={redirectUri}&response_type=token&v=5.131&state=123456";
        authString = authString.Replace("&", "^&");
        Process.Start(new ProcessStartInfo(
            "cmd",
            $"/c start {authString}")
        { CreateNoWindow = true });
    https://oauth.vk.com/blank.html#access_token=vk1.a.Cv_c_KBB2_kGglJS-W3pkxNadIayxljDmkW1mfYISsy1AklbMqAX78GKliEBBbhcyekkx7-UCLt6Bq37BmSGO8LtcrcpnggfdJ9FQO7D4y5Z3qobaxCjBqWTWpFt9T0ZW4UcmnV5BJ7fIXfnUCntqVworVJpn5n5m4j9iDybbTc906dNl6uTa7aUzqCu3zeu&expires_in=86400&user_id=275764447&state=123456
        var access_token = Console.ReadLine();
        var response = await httpClient.GetAsync($"https://api.vk.com/method/friends.getOnLine?user_id={userID}&v=5.131&fields=bdate&access_token={access_token}");
    }

}