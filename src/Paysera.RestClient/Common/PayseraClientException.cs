using System;

namespace Paysera.RestClient.Common
{
    public class PayseraClientException : Exception
    {
        public PayseraClientException(string message) : base(message)
        {
        }
    }
}