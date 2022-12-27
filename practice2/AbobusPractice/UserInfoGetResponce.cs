namespace AbobusPractice;

using Newtonsoft.Json;

public class UserInfoGetResponce
{
    public List<UserInfo> response { get; set; } = new List<UserInfo>();

    public class UserInfo
    {
        public string id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("bdate")]
        public string Bdate { get; set; }

    }
}
