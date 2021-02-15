using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using Mirror;

public class UI_City_Hover_prefab_Store : MonoBehaviour
{
    
    public TextMeshProUGUI CityName;
    public GameObject ResourceInstancing;
    public Transform ResourceParent;

    public citySystem ownCity;
    public Canvas sortingStuff;
    public RawImage OwnerPicture;

    public float UIscale;

    private void OnEnable()
    {
        LocalSettings.playerPairs.TryGetValue(GameController.Instance.localSettings.localPlayer.netIdentity, out LocalSettings.playerPair val);
        if (val.RoomPlayer != null)
            OwnerPicture.texture = val.RoomPlayer.gameObject.GetComponent<AORNetworkRoomPlayer>().profileImage.texture;
    }
    public void disableObject()
    {
        //transform.DOScale(0, 0.2f).OnComplete(() => { gameObject.SetActive(false); });
    }
    public void SetName()
    {
        CityName.text = ownCity.name;
    }

   
}
