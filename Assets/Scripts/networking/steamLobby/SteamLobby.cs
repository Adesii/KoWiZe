using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    [SerializeField] private Button newGameButton = null;
    [SerializeField] private GameObject lobbyMenu = null;
    [SerializeField] private GameObject mainMenuFader = null;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string HostAddressKey = "HostAddress";

    private NetworkManager networkManager;
    public static CSteamID LobbyId { get; private set; }

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();

        if (!SteamManager.Initialized) { return; }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        newGameButton.enabled = false;

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }
    public void CloseLobby()
    {
        newGameButton.enabled = true;
        if (networkManager.isNetworkActive)
            networkManager.StopServer();
        else
            networkManager.StopClient();

    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            newGameButton.enabled = true;
            lobbyMenu.SetActive(false);
            return;
        }

        lobbyMenu.SetActive(true);
        mainMenuFader.GetComponent<simpleUIFader>().disableObject();
        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(
            LobbyId,
            HostAddressKey,
            SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey);
        lobbyMenu.SetActive(true);
        mainMenuFader.GetComponent<simpleUIFader>().disableObject();
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();

        newGameButton.enabled = false;
    }
}
