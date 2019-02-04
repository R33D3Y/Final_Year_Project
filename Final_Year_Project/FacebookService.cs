using Final_Year_Project;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public interface IFacebookService
    {
        Task<Account> GetAccountAsync(string accessToken);
        Task<List<Event>> GetEventAsync(string accessToken);
        Task PostOnWallAsync(string accessToken, string message);
    }

    public class FacebookService : IFacebookService
    {
        private readonly IFacebookClient _facebookClient;

        public FacebookService(IFacebookClient facebookClient)
        {
            _facebookClient = facebookClient;
        }

        public async Task<Account> GetAccountAsync(string accessToken)
        {
            var result = await _facebookClient.GetAsync<dynamic>(accessToken, "me", "fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale");

            if (result == null)
            {
                return new Account();
            }

            var account = new Account
            {
                Id = result.id,
                Email = result.email,
                Name = result.name,
                UserName = result.username,
                FirstName = result.first_name,
                LastName = result.last_name,
                Locale = result.locale
            };

            return account;
        }

        public async Task<List<Event>> GetEventAsync(string accessToken)
        {
            var result = await _facebookClient.GetAsync<dynamic>(accessToken, "me", "fields=events{id, name, place, description, start_time}");

            if (result == null)
            {
                return new List<Event>();
            }

            List<Event> list = new List<Event>();

            int i = 0;

            while (true)
            {
                try
                {
                    Event events = new Event
                    {
                        Id = result.events["data"][i]["id"],
                        Name = result.events["data"][i]["name"],
                        Place = result.events["data"][i]["place"]["name"],
                        Description = result.events["data"][i]["description"],
                        Date = result.events["data"][i]["start_time"]
                    };

                    list.Add(events);
                    i++;
                }

                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            }

            return list;
        }

        public async Task PostOnWallAsync(string accessToken, string message) => await _facebookClient.PostAsync(accessToken, "me/feed", new {message});
    }  
}