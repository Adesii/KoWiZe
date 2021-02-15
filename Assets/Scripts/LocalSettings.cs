using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LocalSettings
{
    public Camera LocalCamera;
    public PlayerScript localPlayer;
    public List<IUnit> LocalPlayerUnlockedUnits = new List<IUnit>();

    public static connPlayerDictionary playerPairs = new connPlayerDictionary();

    public class connPlayerDictionary : SyncDictionary<NetworkIdentity, playerPair> { }
    [System.Serializable]
    public struct playerPair
    {
        public NetworkIdentity RoomPlayer;
        public NetworkIdentity GamePlayer;
    }
}