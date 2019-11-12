using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VA_SummonMinion : VisualAction
{
    public VisualEntity Visual;
    public Vector3 SourcePosition;
    public Vector3 TargetPosition;
    public float TargetScale;

    public VA_SummonMinion(VisualEntity visual, Vector3 sourcePosition, Vector3 targetPosition, float targetScale)
    {
        Frames = 100;
        Visual = visual;
        Visual.transform.localScale = new Vector3(0, 0, 0);
        SourcePosition = sourcePosition;
        TargetPosition = targetPosition;
        TargetScale = targetScale;
        Visual.transform.position = SourcePosition;
    }

    public override void Update()
    {
        base.Update();
        Visual.transform.position = Vector3.Lerp(SourcePosition, TargetPosition, CurrentFrame / Frames);
        float scale = TargetScale / Frames * CurrentFrame;
        Visual.transform.localScale = new Vector3(scale, scale, scale);
    }
}
