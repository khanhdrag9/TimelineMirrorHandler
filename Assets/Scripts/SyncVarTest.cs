using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.Playables;

public class SyncVarTest : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] TimelineNetworking timeline;
    [SerializeField] PlayableDirector directorControl;


    private void Start()
    {
        timeline.OnReceivedTime += (value) =>
        {
            directorControl.time = value;
        };
    }

    private void Update()
    {
        if (timeline.isServer)
        {
            text.text = $"IsPlay: {timeline.IsPlay}\nTime: {timeline.Time}\nServer: {EventCreator.Instance.debug}";
        }
        else
            text.text = $"clientID: {timeline.Id}\nJoined: {timeline.IsConnected}\nIsPlay: {timeline.IsPlay}\nTime: {timeline.Time}";

        UpdateTimeline();
    }

    void UpdateTimeline()
    {
        timeline.UpdateTime((float)directorControl.time);

        if (timeline.IsPlay)
        {
            if (directorControl.state == PlayState.Paused)
            {
                directorControl.time = timeline.Time;
                directorControl.Play();
            }
        }
        else
        {
            if (directorControl.state == PlayState.Playing)
                directorControl.Stop();
        }
    }

    public void Stop()
    {
        timeline.Stop();
    }

    public void Play()
    {
        timeline.Play();
    }

    public void Subscribe()
    {
        if (!timeline.IsConnected && timeline.IsAvailable)
            timeline.Subscribe();
    }
}
