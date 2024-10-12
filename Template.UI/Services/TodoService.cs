using Blazor_Template_Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace Template.UI.Services
{
    public class TodoService
    {
        private readonly HttpClient client;
        private readonly IConfiguration configuration;
        private readonly GlobalVariableService globalVariableService;

        public TodoService(IConfiguration configuration, GlobalVariableService globalVariableService)
        {
            this.configuration = configuration;
            this.globalVariableService = globalVariableService;
            var url = configuration["BackendUrl"];

            this.client = new HttpClient { BaseAddress = new Uri(url) };
            this.client.DefaultRequestHeaders.Accept.Add(
             new MediaTypeWithQualityHeaderValue("application/json"));

            
            //this.client.DefaultRequestHeaders.Add("x-api-key", "api-key");

        }

        public async Task<bool> Login(string username, string password)
        {

            HttpResponseMessage response = await client.GetAsync($"/login?username={username}&password={password}");
            //HttpResponseMessage response = await client.GetAsync($"/login?username=admin&password=admin");

            if (response.IsSuccessStatusCode)
            {

                string token = await response.Content.ReadAsStringAsync();
                globalVariableService.AccessToken = token.Trim('\"');
                //dynamic result = JsonConvert.DeserializeObject(json);                
                //var ret = result.title;
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<TodoDTO>> GetTodosAsync()
        {
            try
            {

                AddToken();

                var results = await this.client.GetFromJsonAsync<List<TodoDTO>>("/todos");
                if (results != null)
                {
                    return results;
                }
                else
                {
                    throw new Exception("Error loading todos");
                }
            }
            catch(Exception ex)
            {
                throw new Exception("could not make todos call");
            }
        }

        private void AddToken()
        {

            var token = globalVariableService.AccessToken;
            //this.client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", (string)"Bearer " + token);
            //this.client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            this.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            this.client.DefaultRequestHeaders.TryAddWithoutValidation("Grpc-Metadata-Authorization", (string)"Bearer " + token);
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<TodoDTO> GetTodoAsync(string id)
        {
            AddToken();

            var results = await client.GetFromJsonAsync<TodoDTO>($"/todo/{id}");
            if (results != null)
            {
                return results;
            }
            else
            {
                throw new Exception("Error loading todo");
            }
        }

        public async Task AddTodoAsync(TodoDTO todo)
        {
            AddToken();
            var response = await client.PostAsJsonAsync("/todo", todo);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error saving todo");
            }
        }

        public async Task UpdateTodoAsync(TodoDTO todo)
        {
            AddToken();
            var response = await client.PutAsJsonAsync($"/todo/{todo.Id}", todo); //puts todo in body
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error updating todo");
            }
        }

        public async Task DeleteTodoAsync(string id)
        {
            AddToken();
            var response = await client.DeleteAsync($"/todo/{id}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error deleting todo");
            }

        }
    }
}
