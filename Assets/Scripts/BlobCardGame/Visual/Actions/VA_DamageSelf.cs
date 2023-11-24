using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VA_DamageSelf : VisualAction
{
    public VisualEntity Visual;
    public Color SourceColor;

    public float BobExtraScale = 1.5f;
    public Vector3 BobSourceScale;
    public Vector3 BobTargetScale;

    public VA_DamageSelf(VisualEntity visual, int amount)
    {
        Frames = DefaultFrames * 1.5f;
        Visual = visual;
        SourceColor = Visual.GetComponent<Renderer>().material.color;

        BobExtraScale = 1 + ((float)amount / 10);
        BobSourceScale = Visual.transform.localScale;
        BobTargetScale = new Vector3(BobSourceScale.x * BobExtraScale, BobSourceScale.y * BobExtraScale, BobSourceScale.z * BobExtraScale);
    }

    public override void OnStart()
    {
        base.OnStart();
        Visual.GetComponent<Renderer>().material.color = Color.red;
    }

    public override void Update()
    {
        base.Update();
        if (CurrentFrame <= Frames / 2)
        {
            float sourceScale = BobSourceScale.x + ((BobTargetScale.x - BobSourceScale.x) * (CurrentFrame / (Frames / 2)));
            Visual.transform.localScale = new Vector3(sourceScale, sourceScale, sourceScale);
        }
        else
        {
            float sourceScale = BobTargetScale.x - ((BobTargetScale.x - BobSourceScale.x) * ((CurrentFrame - Frames / 2) / (Frames / 2)));
            Visual.transform.localScale = new Vector3(sourceScale, sourceScale, sourceScale);
        }
    }

    public override void OnDone()
    {
        base.OnDone();
        Visual.transform.localScale = BobSourceScale;
        Visual.GetComponent<Renderer>().material.color = SourceColor;
    }
}
