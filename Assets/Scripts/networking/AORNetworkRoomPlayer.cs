using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using Steamworks;
using FMODUnity;

/*
	Documentation: https://mirror-networking.com/docs/Components/NetworkRoomPlayer.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkRoomPlayer.html
*/

/// <summary>
/// This component works in conjunction with the NetworkRoomManager to make up the multiplayer room system.
/// The RoomPrefab object of the NetworkRoomManager must have this component on it.
/// This component holds basic room player data required for the room to function.
/// Game specific data for room players can be put in other components on the RoomPrefab or in scripts derived from NetworkRoomPlayer.
/// </summary>
public class AORNetworkRoomPlayer : NetworkRoomPlayer
{
    [SyncVar(hook = nameof(HandleSteamIdUpdated))]
    private ulong steamId;

    [SerializeField] public RawImage profileImage = null;
    [SerializeField] public TMP_Text displayNameText = null;

    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

    #region Server

    public void SetSteamId(ulong steamId)
    {
        this.steamId = steamId;
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
        transform.SetParent(FindObjectOfType<AORRoomDisplayer>().transform);
        if (isLocalPlayer) GameController.Instance.localSettings.localRoomPlayer = this;
    }
    [Command]
    public void CmdSetLobbySettings(AORLobbySettings lobbySettings)
    {
        SyncableStorage.main.WorldSeedString = lobbySettings.seed;
    }




    [System.Serializable]
    public struct AORLobbySettings
    {
        public string seed;
    }


    private void HandleSteamIdUpdated(ulong oldSteamId, ulong newSteamId)
    {
        var cSteamId = new CSteamID(newSteamId);

        displayNameText.text = SteamFriends.GetFriendPersonaName(cSteamId);

        int imageId = SteamFriends.GetLargeFriendAvatar(cSteamId);

        if (imageId == -1) { return; }

        profileImage.texture = GetSteamImageAsTexture(imageId);
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID != steamId) { return; }

        profileImage.texture = GetSteamImageAsTexture(callback.m_iImage);
    }
    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        return texture;
    }
    #endregion
    [ClientRpc]
    public void RpcChangeView()
    {
        RuntimeManager.StudioSystem.setParameterByName("GlobalState", 0f);
        if (GameController.UIInstance.menuUI.BlackScreen != null)
        {
            GameController.UIInstance.menuUI.BlackScreen.SetActive(true);
        }
        GameController.UIInstance.NewGame(()=> { });
    }
}
