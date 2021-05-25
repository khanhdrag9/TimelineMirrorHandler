using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Mirror;
using UnityEngine.Playables;
using System;
using UnityEngine.Events;

public class TimelineNetworking : MirrorCustomCycleBehaviour
{
    [SyncVar(hook =("TimeUpdate"))] float timelineTime = 0f;
    [SyncVar] bool isPlay = false;

    public bool IsPlay => isPlay;
    public double Time => timelineTime;

    public Action<double> OnReceivedTime;


    public void Play()
    {
        if (IsAvailable && IsConnected)
        {
            MoveToHead();
            SetState(true);
        }
    }

    public void Stop()
    {
        MoveToHead();
        SetState(false);
    }

    public void UpdateTime(float value)
    {
        if(IsCaster)
            CmdUpdateTime(value);
    }

    void SetState(bool play)
    {
        CmdSetState(play);
    }

    public void TimeUpdate(float oldValue, float newValue)
    {
        if (IsCaster) return;
        OnReceivedTime?.Invoke(newValue);
    }

    [Command(requiresAuthority = false)] 
    public void CmdUpdateTime(float value)
    {
        timelineTime = value;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetState(bool value)
    {
        isPlay = value;
    }

    protected override void OnLeave(string eventName)
    {
        if(this.eventName == eventName)
        {
            if (isServer)
            {
                timelineTime = 0f;
                isPlay = false;
                OnReceivedTime?.Invoke(0);
            }
            else
            {
                OnReceivedTime?.Invoke(0);
            }
        }
    }
}
