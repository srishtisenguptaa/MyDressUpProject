using MyMVCProject.Models;

namespace MyUserProject.Services
{
    public class PythonApiService
    {
        private readonly HttpClient _http;

        public PythonApiService(HttpClient httpClient)
        {
            _http = httpClient;
            _http.BaseAddress = new Uri("http://localhost:8000/");
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _http.GetFromJsonAsync<List<Product>>("products");
        }
    }
}
