using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public partial class GameController
{
    [Serializable]
    public class CitySetting
    {
        public List<perPlayerCitySettings> perPlayerSettings = new List<perPlayerCitySettings>();
        public List<Sprite> icons = new List<Sprite>();
        public GameObject cityPrefab;
        public GameObject ResourcePrefab;

        [Serializable]
        public class perPlayerCitySettings
        {
            public int playerID;
            public RawImage playerProfilePic;
            public float exponentialFoodProduction = 2f;
            public float exponentialScienceProduction = 2f;

            
            public PlayerScript playerScript;

            public float science;
            public float gold;

            public List<citySystem> playerCities;


        }


    }
}

