using UnityEngine;
using TMPro;
using UnityEngine.UI;


[System.Serializable]
public class ResourceIcon : MonoBehaviour
{
    public Image Resource;
    public TextMeshProUGUI ResourceCount;
    public ResourceClass.ResourceTypes ResourceType;
    public ResourceClass Resourceclass;
}
