using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;

namespace AuthorizedCloud;

public class PlayerDataService
{
    private readonly ILogger<PlayerDataService> _logger;
    public const string k_PlayerNameKey = "PLAYER_NAME";

    public PlayerDataService(ILogger<PlayerDataService> logger)
    {
        _logger = logger;
    }

    
    private async Task SaveData(IExecutionContext context, IGameApiClient gameApiClient, string key, object value)
    {
        try
        {
            await gameApiClient.CloudSaveData.SetItemAsync(
                context,
                context.AccessToken,
                context.ProjectId,
                context.PlayerId,
                new SetItemBody(key, value));
        }
        catch (ApiException ex)
        {
            _logger.LogError("Failed to save data. Error: {Error}", ex.Message);
            throw new Exception($"Failed to save data for playerID: {context.PlayerId}. Error: {ex.Message}");
        }
    }

    
    private async Task<object> GetData(IExecutionContext context, IGameApiClient gameApiClient, string key)
    {
        try
        {
            var result = await gameApiClient.CloudSaveData.GetItemsAsync(
                context,
                context.AccessToken,
                context.ProjectId,
                context.PlayerId,
                new List<string> { key });

            return result.Data.Results.First().Value;
        }
        catch (ApiException ex)
        {
            _logger.LogError("Failed to save data. Error: {Error}", ex.Message);
            throw new Exception($"Failed to save data for playerID: {context.PlayerId}. Error: {ex.Message}");
        }
    }


    [CloudCodeFunction("SayHello")]
    public string Hello(string name)
    {
        return $"Hello, {name}!";
    }

    [CloudCodeFunction("HandleNewPlayerNameEntry")]
    public async Task<string> HandleNewPlayerNameEntry(IExecutionContext context, IGameApiClient gameApiClient, string newName)
    {
        if (IsPlayerNameValid(newName))
        { 
            await SaveData(context, gameApiClient, k_PlayerNameKey, newName);
            return newName;
        }
        throw new ArgumentException("Name is not valid");
    }

    private bool IsPlayerNameValid(string Name)
    {
        if (Name.Length is < 4 or > 16)
        {
            return false;
        }

        // Check for special characters uning LINQ 
        if (!Name.All(c => char.IsLetterOrDigit(c)))
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


