using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// Client Only
public class MirrorCustomCycleBehaviour : NetworkBehaviour
{
    [SerializeField] protected string eventName = "Event1";


    EventCreator Nk => EventCreator.Instance;
    public bool IsConnected => Nk.JoinedEvent(eventName, Id);
    public bool IsCaster => Nk.IsHostOfEvent(eventName, Id);
    public bool IsAvailable => NetworkClient.isConnected && !string.IsNullOrEmpty(Id);
    public string Id => NetworkClient.localPlayer == null ? "" : NetworkClient.localPlayer.netId.ToString(); // Only client
    public bool Any => Nk.Any(eventName);

    bool sentSub = false;

    #region Private 
    public override void OnStartClient()
    {
        
    }

    public override void OnStopClient()
    {
        ResetData();
    }

    private void OnDestroy()
    {
    }

    public virtual void ResetData()
    {
        sentSub = false;
    }
    #endregion

    public void MoveToHead()
    {
        Nk.RegisterCastEvent(eventName, Id);
    }

    public void Subscribe()
    {
        if (sentSub) return;

        sentSub = true;
        Nk.SubscribeEvent(eventName, Id);
        Nk.OnLeaveAll += OnLeave;
    }

    public void UnSubscribe()
    {
        Nk.UnSubscribeEvent(eventName, Id);
        Nk.OnLeaveAll -= OnLeave;

        ResetData();
    }

    protected virtual void OnLeave(string eventName)
    {
        ResetData();
    }
}
