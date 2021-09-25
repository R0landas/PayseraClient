using System;
using System.Threading.Tasks;
using Paysera.RestClient.Common;
using RestSharp;

namespace Paysera.RestClient
{
    public class PayseraClient
    {
        private readonly IRestClient _restClient;

        public PayseraClient(IRestClient restClient)
        {
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        }
        
        public async Task<TResult> GetAsync<TResult>(string resource)
        {
            if (string.IsNullOrWhiteSpace(resource))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(resource));
            
            var request = new RestRequest(resource);
            var response = await _restClient.ExecuteGetAsync<TResult>(request);

            ValidateResponse(response);
            
            return response.Data;
        }

        public async Task<TResult> PostAsync<TResult, TPayload>(string resource, TPayload payload)
        {
            if (string.IsNullOrWhiteSpace(resource))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(resource));
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            
            var request = new RestRequest(resource);
            request.AddJsonBody(payload);
            var response = await _restClient.ExecutePostAsync<TResult>(request);

            ValidateResponse(response);

            return response.Data;
        }

        public async Task<TResult> PutAsync<TResult, TPayload>(string resource, TPayload payload)
        {
            if (string.IsNullOrWhiteSpace(resource))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(resource));
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            
            var request = new RestRequest(resource);
            request.AddJsonBody(payload);
            
            return await _restClient.PutAsync<TResult>(request);
        }

        private static void ValidateResponse(IRestResponse response)
        {
            if (response.IsSuccessful) return;
            throw new PayseraClientException($"Error executing API call error code: {response.StatusCode}");
        }
    }
}