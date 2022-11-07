using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

//This class has been added just for testing purposes and shall be deleted later on

namespace CommunAxiom.Commons.ClientUI.Shared.ViewModels
{
    public class TestViewModel : ITestViewModel
    {
        private readonly HttpClient _httpClient;

        public TestViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string ID { get; set; }
        public string DataSourceID { get; set; }
        public string CronExpression { get; set; }
        public string CronTimeZone { get; set; }
        public string NextExecutionTime { get; set; }
        public string ScheduleType { get; set; }
        public DateTime StartDate { get; set; }
        public Schedulers scheduler { get; set; }
        public List<Schedulers> Schedulers { get; set; }

        public async Task CreateScheduler(Schedulers scheduler)
        {
            await _httpClient.PostAsJsonAsync("/api/Scheduler/Create", scheduler);
        }

        public async Task<List<Schedulers>?> GetSchedulers()
        {
            HttpResponseMessage? httpResponseMessage;
            httpResponseMessage = await _httpClient.GetAsync("/api/Scheduler/GetAll");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var errorMessage = httpResponseMessage.ReasonPhrase;
                Console.WriteLine($"There was an error! {errorMessage}");
                return null;
            }
            else
            {
                if ((int)httpResponseMessage.StatusCode == 204)
                {
                    Console.WriteLine("Schedulers List is defined but has no content");
                    return null;
                }
                return httpResponseMessage.Content.ReadFromJsonAsync<List<Schedulers>>().Result;
            }
        }
    }
}
