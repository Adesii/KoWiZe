using UnityEngine;
using Mirror;
using System.Linq;
using Steamworks;

/*
	Documentation: https://mirror-networking.com/docs/Components/NetworkRoomManager.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkRoomManager.html

	See Also: NetworkManager
	Documentation: https://mirror-networking.com/docs/Components/NetworkManager.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

/// <summary>
/// This is a specialized NetworkManager that includes a networked room.
/// The room has slots that track the joined players, and a maximum player count that is enforced.
/// It requires that the NetworkRoomPlayer component be on the room player objects.
/// NetworkRoomManager is derived from NetworkManager, and so it implements many of the virtual functions provided by the NetworkManager class.
/// </summary>
public class AORNetworkRoomManager : NetworkRoomManager
{

    /// <summary>
    /// This is called on the server when the server is started - including when a host is started.
    /// </summary>
    /// 
    public override void OnRoomStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
        base.OnRoomStartServer();

    }

    public override void OnRoomStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");
        foreach (var item in spawnablePrefabs)
        {
            ClientScene.RegisterPrefab(item);
        }
        base.OnRoomStartClient();

    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(
            SteamLobby.LobbyId,
            numPlayers - 1);

        var playerInfoDisplay = conn.identity.GetComponent<AORNetworkRoomPlayer>();

        playerInfoDisplay.SetSteamId(steamId.m_SteamID);
    }
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        GameController.UIInstance.NewGame();

    }
    public void ChangePlayerReadyState()
    {
        foreach (var item in roomSlots)
        {
            if (item.isLocalPlayer)
            {
                item.CmdChangeReadyState(!item.readyToBegin);
            }
        }
    }
    
}
