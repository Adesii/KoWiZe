using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    SimplexNoiseGenerator noiseGen = new SimplexNoiseGenerator();
    public string seed = "lorem";

    bool isWorldGenComplete = false;
    // Start is called before the first frame update
    void Start()
    {
        noiseGen = new SimplexNoiseGenerator(seed.ToString());
    }

    
    void Update()
    {
        if (!isWorldGenComplete)
        {
            generateWorld();
        }
    }

    void generateWorld()
    {

    }
}
