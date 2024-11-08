using UnityEditor;
using UnityEngine;
//using System.Threading.Tasks;

/// <summary>
/// 설정 및 관리
/// </summary>

public static class Config
{
    public static string userNickName;


    
}

    //async static void TrySetGameData()
    //{
    //    await Task.Run(() => {
    //        PlayerData.SetGameData();
    //    });
    //}

public enum SceneList
{
    Init,
    World
}



public enum PlayerState
{
    Idle,
    Walk,
    Run   
}

public enum Weather
{
    Sunny,
    Rainy,
    RainySnowy,
    Snowy,
    Raindrop = 5,
    RaindropSnowyblow,
    Snowblow
}





