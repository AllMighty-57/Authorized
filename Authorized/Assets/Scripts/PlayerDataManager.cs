using UnityEngine;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using System;
using System.Threading.Tasks;

public class PlayerDataManager : MonoBehaviour
{
    public MyModuleBindings MyModuleBindings;
    public LoginManager LoginManager; 
    public string PlayerName;

    void Start()
    { 
        LoginManager.PlayerSignedIn += InitializePlayer; 
        MyModuleBindings = new MyModuleBindings(CloudCodeService.Instance);

    }

    private async void InitializePlayer()
    {
        try
        {
            var resultFromCloud = await MyModuleBindings.SayHello(PlayerName);
            Debug.Log($"{resultFromCloud}");
        } 

        catch(CloudCodeException ex)
        {
            Debug.LogException(ex);
        }
    } 

    public async void SaveNewPlayerName()
    {
        try
        {
            // PlayerName = await MyModuleBindings 
            Debug.Log($"Saved new player name in cloud: {PlayerName}"); 
        } 

        catch(Exception ex) 
        {
            Debug.LogException(ex);
        }
    }

    private void OnDisable()
    {
        LoginManager.PlayerSignedIn -= InitializePlayer;
    }
}

