# Paysera Rest Client for .NET 5
This is a common rest client lib for paysera APIs (tested with the [Transfer API](https://developers.paysera.com/en/transfer/))

This library is based on the [original](https://github.com/paysera/dotnet-lib-rest-client-common) library from Paysera, but built for .NET 5

## The Ugly Part:
I have not implemented any response types, that is why the dynamic type is used in the example, by default the response will be mapped to a Dictionary<string,object>.

## Authentication:
Only MAC authentication is implemented as I have no idea how the client certificate authentication works and was not required for the project I was working on. If that is something you need, please have a look at the original library and feel free to create a pull request with your implementation.

## Example usage:
First of all, you will have to install the package from github

### Creating a Transfer
```c#
// Creating the rest client with MacAuthenticator
var restClient = new RestClient("https://wallet.paysera.com/transfer/rest/v1")
{
    Authenticator =
        new Paysera.RestClient.Authentication.MacAuthenticator("MAC_ID", "MAC_KEY")
};

// Creating Paysera API client
var client = new PayseraClient(restClient);

// Creating a transfer
var transfer = new
{
    amount = new
    {
        amount = "1",
        currency = "EUR"
    },
    beneficiary = new
    {
        type = "bank",
        name = "Firstname Lastname",
        bank_account = new
        {
            iban = "Bank Account"
        }
    },
    payer = new
    {
        account_number = "Bank Account"
    },
    purpose = new
    {
        details = "Test transfer"
    }
};

// Calling Paysera API to create a transfer
var result = await client.PostAsync<dynamic, object>("/transfers", transfer);

// Logging/accesing response data
// Using the dynamic type, response data is mapped onto Dictionary<string, object>
// That makes data access a bit ugly, but it works. 
// I might add proper response types in the near future
Console.WriteLine($"Transfer Id: {result["id"]} Status: {result["status"]}");
```