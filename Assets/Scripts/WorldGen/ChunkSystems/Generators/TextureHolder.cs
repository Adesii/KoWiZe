using SubjectNerd.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Textures For Terrain",menuName ="KoWiZe Custom Assets/textureHolder")]
public class TextureHolder : ScriptableObject
{
    
    [Reorderable]
    public List<pbrMaterialGroup> textureList;

    [Serializable]
    public class pbrMaterialGroup
    {
        public Texture2D mainTexture;
        public Texture2D normalTexture;
        public Texture2D roughTexture;

        public float height;
    }
}
