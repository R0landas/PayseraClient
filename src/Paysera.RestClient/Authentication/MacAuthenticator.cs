using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using RestSharp;
using RestSharp.Authenticators;

namespace Paysera.RestClient.Authentication
{
    public class MacAuthenticator : IAuthenticator
    {
        private readonly string _macId;
        private readonly string _key;

        public MacAuthenticator(string macId, string key)
        {
            if (string.IsNullOrWhiteSpace(macId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(macId));
            
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            
            _macId = macId;
            _key = key;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            var headerValue = BuildAuthorizationHeader(new Uri($"{client.BaseUrl}{request.Resource}"), request);
            request.AddHeader("Authorization", headerValue);
        }

        private string BuildAuthorizationHeader(Uri uri, IRestRequest request)
        {
            var method = request.Method.ToString();

            var ts = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            var nonce = Guid.NewGuid().ToString();

            var ext = "";
            if (request.Method is Method.POST or Method.PUT)
            {
                var bodyHash = GetBodyHash(request.Body);
                ext = $"body_hash={HttpUtility.UrlEncode(bodyHash)}";
            }
            
            
            var mac = BuildMac(ts, nonce, method, uri, ext);

            return $@"MAC id=""{_macId}"",ts=""{ts}"",nonce=""{nonce}"",mac=""{mac}"",ext=""{ext}""";
        }
        
        private string BuildMac(string ts, string nonce, string method, Uri uri, string ext)
        {
            var normalizedString = $"{ts}\n{nonce}\n{method}\n{uri.PathAndQuery}\n{uri.Host}\n{uri.Port}\n{ext}\n";
            HashAlgorithm hashGenerator = new HMACSHA256(Encoding.ASCII.GetBytes(_key));
            return Convert.ToBase64String(hashGenerator.ComputeHash(Encoding.ASCII.GetBytes(normalizedString)));
        }

        private static string GetBodyHash(RequestBody body)
        {
            if (body is null)
            {
                return "";
            }

            using var sha256Hash = SHA256.Create();
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(body.Value.ToString() ?? throw new InvalidOperationException()));
            return Convert.ToBase64String(bytes);
        }
    }
}