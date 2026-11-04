using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    private PlayerDataServiceBindings m_Bindings;
    public LoginManager LoginManager; 
    public string PlayerName;

    void Start()
    { 
        LoginManager.PlayerSignedIn += InitializePlayer; 
        m_Bindings = new PlayerDataServiceBindings(CloudCodeService.Instance);

    }

    private async void InitializePlayer()
    { 
        if (!IsPlayerNameValid(PlayerName))
        {
            Debug.LogWarning("Name must be 4-16 characters and contain only letters and numbers");
            return;
        }
        try
        {
            var resultFromCloud = await m_Bindings.SayHello(PlayerName);
            Debug.Log($"{resultFromCloud}");
        } 

        catch(CloudCodeException ex)
        {
            Debug.LogWarning("Player name invalid");
            Debug.LogException(ex);
        }
    } 

    public async void SaveNewPlayerName()
    {
        try
        {
            PlayerName = await m_Bindings.HandleNewPlayerNameEntry(PlayerName);
            Debug.Log($"Saved new player name in cloud: {PlayerName}"); 
        } 

        catch(Exception ex) 
        {
            Debug.LogException(ex);
        }
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

    private void OnDisable()
    {
        LoginManager.PlayerSignedIn -= InitializePlayer;
    }
}

