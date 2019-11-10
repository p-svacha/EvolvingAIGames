using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualAction
{
    public int CurrentFrame = 0;
    public float Frames;
    public bool Done;

    public virtual void Update()
    {
        CurrentFrame++;
        if(CurrentFrame >= Frames)
        {
            Done = true;
        }
    }
}
