using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VA_SummonPlayer : VisualAction
{
    public VisualEntity Visual;

    public VA_SummonPlayer(VisualEntity visual, Color color)
    {
        //Frames = 40;
        Visual = visual;
        Visual.GetComponent<Renderer>().material.color = color;
        Visual.transform.localScale = new Vector3(0, 0, 0);
    }

    public override void Update()
    {
        base.Update();
        float scale = 1 / Frames * CurrentFrame;
        Visual.transform.localScale = new Vector3(scale, scale, scale);
    }
}
