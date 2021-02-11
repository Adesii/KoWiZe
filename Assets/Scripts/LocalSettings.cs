using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalSettings
{
    public Camera LocalCamera;
    public PlayerScript localPlayer;
    public List<IUnit> LocalPlayerUnlockedUnits = new List<IUnit>();

}