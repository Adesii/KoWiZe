using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildableObject : MonoBehaviour
{
    public bool isBuilding = true;
    public abstract void HasBeenBuild();
    public abstract void wantsTobeBuild();
}
