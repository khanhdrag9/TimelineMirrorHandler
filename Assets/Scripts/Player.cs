using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField] GameObject canvasTest;

    public override void OnStartClient()
    {
        base.OnStartClient();

        //NetworkServer.Spawn(canvasTest);
    }


}
