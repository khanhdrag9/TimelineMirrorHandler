using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class MyNetworkManager : NetworkManager
{
    [Header("Myself")]
    [SerializeField] GameObject[] spawnOnScene;

    public override void OnStartServer()
    {
        foreach (var e in spawnOnScene)
            NetworkServer.Spawn(e);
    }

    public override void OnStartClient()
    {

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        EventCreator.Instance.UnSubscribeAll(conn.identity.netId.ToString());

        base.OnServerDisconnect(conn);
    }
}
