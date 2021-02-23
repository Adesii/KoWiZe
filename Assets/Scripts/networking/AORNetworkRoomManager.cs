using UnityEngine;
using Mirror;
using System.Linq;
using Steamworks;
using System.Threading.Tasks;

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
    public static AORNetworkRoomManager instance;

    public override void Awake()
    {
        base.Awake();
        instance = this;
    }
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

    public override void ServerChangeScene(string newSceneName)
    {
        if (GameController.UIInstance.menuUI.BlackScreen != null)
        {
            GameController.UIInstance.menuUI.BlackScreen.SetActive(true);
        }
        GameController.UIInstance.NewGame(()=>base.ServerChangeScene(newSceneName));
    }
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
    }
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer)
    {

        LocalSettings.playerPairs.Add(gamePlayer.GetComponent<NetworkIdentity>(), new LocalSettings.playerPair
        {
            GamePlayer = gamePlayer.GetComponent<NetworkIdentity>(),
            RoomPlayer = roomPlayer.GetComponent<NetworkIdentity>()
        });
        var playerCPIndexX = Mathf.Lerp(2,World.main.typeOfWorld.sizeX-2, (conn.connectionId+1 % numPlayers));
        var playerCPIndexZ = Mathf.Lerp(2, World.main.typeOfWorld.sizeZ- 2,( conn.connectionId+1 / numPlayers));
        var pos = new Vector3((playerCPIndexX * World.chunkSize) - ((World.main.typeOfWorld.sizeX * World.chunkSize) / 2f), 0, (playerCPIndexZ * World.chunkSize) - ((World.main.typeOfWorld.sizeZ * World.chunkSize) / 2f));
        gamePlayer.transform.position = pos;
        return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
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
