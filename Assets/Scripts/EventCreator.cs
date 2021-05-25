using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class EventCreator : NetworkBehaviour
{
    public static EventCreator Instance { get; protected set; }
    public Action<string> OnLeaveAll;
    public string debug = "";

    protected SyncDictionary<string, string> list = new SyncDictionary<string, string>();
    const char SPLIT = ',';

    private void Awake()
    {
        Instance = this;

    }

    public override void OnStartServer()
    {
    }

    public bool Any(string eventName)
    {
        return list.ContainsKey(eventName) && !string.IsNullOrEmpty(list[eventName]);
    }

    public bool IsHostOfEvent(string eventName, string id)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(eventName))
            return false;

        if (list.ContainsKey(eventName))
        {
            var listIds = list[eventName].Split(SPLIT);
            return listIds.Length > 0 && listIds[0] == id;
        }

        return false;
    }

    public bool JoinedEvent(string eventName, string id)
    {
        return !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(eventName) && list.ContainsKey(eventName) && list[eventName].Contains(id);
    }

    public void SubscribeEvent(string eventName, string id)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(eventName))
            return;

        CmdSubscribeEvent(eventName, id);
    }

    public void UnSubscribeEvent(string eventName, string id)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(eventName))
            return;

        CmdUnSubscribeEvent(eventName, id);
    }

    public void UnSubscribeAll(string id)
    {
        if (string.IsNullOrEmpty(id))
            return;

        Debug.Log("UnSubscribeAll " + id);
        CmdUnSubscribeAll(id);
    }

    public void RegisterCastEvent(string eventName, string id)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(eventName))
            return;

        CmdRegisterCastEvent(eventName, id);
    }

    [Command(requiresAuthority = false)]
    void CmdSubscribeEvent(string eventName, string id)
    {
        if (list.ContainsKey(eventName))
            if (list[eventName].Contains(id))
                Debug.LogWarning($"Id {id} was added");
            else
                list[eventName] += SPLIT + id;
        else
            list.Add(eventName, id);

        Log();
    }


    [Command(requiresAuthority = false)]
    void CmdUnSubscribeEvent(string eventName, string id)
    {
        if (list.ContainsKey(eventName))
        {
            list[eventName] = Remove(list[eventName], id);
        }
        else
            Debug.LogWarning($"Event {eventName} has not registered yet");

        CheckEvent(eventName);
        Log();
    } 
    
    //[Command(requiresAuthority = false)]
    void CmdUnSubscribeAll(string id)
    {
        string[] keys = list.Keys.ToArray();
        for (int i = 0; i < keys.Length; ++i)
        {
            string key = keys[i];
            string value = list[key];
            list[key] = Remove(value, id);
            CheckEvent(key);
        }

        Log();
    }

    [Command(requiresAuthority = false)]
    void CmdRegisterCastEvent(string eventName, string id)
    {
        if (list.ContainsKey(eventName))
        {
            list[eventName] = Remove(list[eventName], id);
            if (string.IsNullOrEmpty(list[eventName]))
                list[eventName] = id;
            else
                list[eventName] = id + SPLIT + list[eventName];
        }

        Log();
    }

    void CheckEvent(string eventName)
    {
        if(string.IsNullOrEmpty(list[eventName]))
        {
            if (isServer)
                OnLeaveAll?.Invoke(eventName);
            else
                RpcLeaveEvent(eventName);
        }
    }

    string Remove(string source, string element)
    {
        return string.Join(SPLIT.ToString(), source.Split(SPLIT).Where((s, i) => s != element));
    }

    void Log()
    {
        if (isServer)
        {
            debug = "";
            foreach (var e in list)
                debug += (e.Key + " - " + e.Value);
        }
        else
            RpcLog();
    }

    [ClientRpc]
    public void RpcLeaveEvent(string eventName)
    {
        OnLeaveAll?.Invoke(eventName);
    }

    [ClientRpc]
    public void RpcLog()
    {
        debug = "";
        foreach (var e in list)
            debug += (e.Key + " - " + e.Value);
    }
}
