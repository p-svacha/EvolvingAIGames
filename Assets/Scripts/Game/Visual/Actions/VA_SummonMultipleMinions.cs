using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VA_SummonMultipleMinions : VisualAction
{
    public List<VisualEntity> Visuals;
    public Vector3 SourcePosition;
    public List<Vector3> TargetPositions;
    public float TargetScale;

    public VA_SummonMultipleMinions(List<VisualEntity> visuals, Vector3 sourcePosition, List<Vector3> targetPositions, float targetScale)
    {
        Frames = 100;
        Visuals = visuals;
        SourcePosition = sourcePosition;
        TargetPositions = targetPositions;
        TargetScale = targetScale;

        foreach(VisualEntity vis in visuals)
        {
            vis.transform.localScale = new Vector3(0, 0, 0);
            vis.transform.position = SourcePosition;
        }
    }

    public override void Update()
    {
        base.Update();
        float scale = TargetScale / Frames * CurrentFrame;
        for (int i = 0; i < Visuals.Count; i++)
        {
            Visuals[i].transform.position = Vector3.Lerp(SourcePosition, TargetPositions[i], CurrentFrame / Frames);
            Visuals[i].transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
