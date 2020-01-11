using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualAction
{
    public int CurrentFrame = 0;
    public float DefaultFrames = 60;
    public float Frames;
    public bool Done;

    public virtual void Update()
    {
        if (CurrentFrame == 0) OnStart();
        CurrentFrame++;
        if(CurrentFrame >= Frames)
        {
            Done = true;
            OnDone();
        }
    }

    public virtual void OnStart() { }
    public virtual void OnDone() { }
}
