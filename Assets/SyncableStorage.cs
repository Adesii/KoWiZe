using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Random = UnityEngine.Random;
using System.Security.Cryptography;
using System.Text;

public class SyncableStorage : NetworkBehaviour
{
    private static SyncableStorage _main;
    public static SyncableStorage main
    {
        get { return _main; }
    }

    private void Awake()
    {
        if(_main == null)
        _main = this;
    }
    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        base.OnStartClient();
        GameController.SyncableStorageInstance = this;
    }
    public override void OnStartServer()
    {
        DontDestroyOnLoad(gameObject);

        base.OnStartServer();
        GameController.SyncableStorageInstance = this;

    }

    #region SyncableLobbySettings

    [SyncVar(hook = nameof(WorldSeedChanged))]
    public string WorldSeedString = "Nothing";
    [SyncVar]
    public int WorldSeedInt = 0;

    private void WorldSeedChanged(string oldSeed, string newSeed)
    {
        using var algo = SHA1.Create();
        if (string.IsNullOrEmpty(newSeed))
        {
            
            WorldSeedInt = BitConverter.ToInt32(algo.ComputeHash(Encoding.UTF8.GetBytes("BaseSeed")),0);
            Random.InitState(WorldSeedInt);
        }
        else
        {
            WorldSeedInt = BitConverter.ToInt32(algo.ComputeHash(Encoding.UTF8.GetBytes(newSeed)), 0);

            Random.InitState(WorldSeedInt);
        }
        Debug.Log($"New WorldSeed {WorldSeedString}; \n New int Seed = {WorldSeedInt}");

    }



    #endregion

}
