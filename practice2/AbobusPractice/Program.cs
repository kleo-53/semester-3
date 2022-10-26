namespace AbobusPractice;

using Newtonsoft.Json;
using System;

class Program
{
    private static async Task Main(string[] args)
    {
        var httpClient = new HttpClient();

        var userID = "";
        var serviceToken = "";
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
        }
    }

}