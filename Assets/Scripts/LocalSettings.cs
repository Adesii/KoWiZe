using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LocalSettings
{
    public Camera LocalCamera;
    public PlayerScript localPlayer;
    public AORNetworkRoomPlayer localRoomPlayer;

    public List<IUnit> LocalPlayerUnlockedUnits = new List<IUnit>();

    public static connPlayerDictionary playerPairs = new connPlayerDictionary();

    public List<GainPair> GainAmount= new List<GainPair>();
    [System.Serializable]
    public struct GainPair
    {
        public ResourceClass.ResourceTypes Resource;
        public float amount;
    }

    public class connPlayerDictionary : SyncDictionary<NetworkIdentity, playerPair> { }
    [System.Serializable]
    public struct playerPair
    {
        public NetworkIdentity RoomPlayer;
        public NetworkIdentity GamePlayer;
    }

}