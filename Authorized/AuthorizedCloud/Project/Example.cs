using System;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

namespace HelloWorld;

public class MyModule
{
    readonly IGameApiClient m_GameApiClient;
    readonly ILogger<MyModule> m_Logger;

    public MyModule(IGameApiClient gameApiClient, ILogger<MyModule> logger)
    {
        m_GameApiClient = gameApiClient;
        m_Logger = logger;
    }

    [CloudCodeFunction("SayHello")]
    public string Hello(string name)
    {
        return $"Hello, {name}!";
    }

    [CloudCodeFunction("HandleNewPlayerNameEntry")]
    public string HandleNewPlayerNameEntry(string newName)
    {
        if (IsPlayerNameValid(newName))
        {
            return newName; 
        }
        throw new ArgumentException("Name is not valid");
    }

    private bool IsPlayerNameValid(string Name)
    {
        if(Name.Length is < 4 or > 16)
        {
            return false;
        }

        // Check for special characters uning LINQ 
        if(!Name.All(c => char.IsLetterOrDigit(c)))
        {
            return false;
        } 
        return true;
    }

    [CloudCodeFunction("GetServerTime")]
    public string GetServerTime(IExecutionContext context)
    {
        return DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
    }
}


