using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VA_SummonMinions : VisualAction
{
    public List<VisualEntity> Visuals;
    public VisualEntity Source;
    public List<Vector3> TargetPositions;
    public float TargetScale;

    public float BobDurationRel = 0.5f;
    public float BobExtraScale = 1.5f;
    public float BobDuration;
    public Vector3 BobSourceScale;
    public Vector3 BobTargetScale;

    public VA_SummonMinions(List<VisualEntity> visuals, VisualEntity source, List<Vector3> targetPositions, float targetScale)
    {
        //Frames = 40;
        Visuals = visuals;
        Source = source;
        TargetPositions = targetPositions;
        TargetScale = targetScale;

        foreach(VisualEntity vis in visuals)
        {
            vis.transform.localScale = new Vector3(0, 0, 0);
            vis.transform.position = Source.transform.position;
        }

        BobDuration = Frames * BobDurationRel;
        BobSourceScale = Source.transform.localScale;
        BobTargetScale = new Vector3(BobSourceScale.x * BobExtraScale, BobSourceScale.y * BobExtraScale, BobSourceScale.z * BobExtraScale);
    }

    public override void Update()
    {
        base.Update();
        if (CurrentFrame <= BobDuration / 2)
        {
            float sourceScale = BobSourceScale.x + ((BobTargetScale.x - BobSourceScale.x) * (CurrentFrame / (BobDuration / 2)));
            Source.transform.localScale = new Vector3(sourceScale, sourceScale, sourceScale);
        }
        else if (CurrentFrame <= BobDuration)
        {
            float sourceScale = BobTargetScale.x - ((BobTargetScale.x - BobSourceScale.x) * ((CurrentFrame - BobDuration / 2) / (BobDuration / 2)));
            Source.transform.localScale = new Vector3(sourceScale, sourceScale, sourceScale);

            float minionScale = TargetScale / Frames * CurrentFrame;
            for (int i = 0; i < Visuals.Count; i++)
            {
                Visuals[i].transform.position = Vector3.Lerp(Source.transform.position, TargetPositions[i], (CurrentFrame - (BobDuration / 2)) / (Frames - (BobDuration / 2)));
                Visuals[i].transform.localScale = new Vector3(minionScale, minionScale, minionScale);
            }
        }
        else
        {
            float minionScale = TargetScale / Frames * CurrentFrame;
            for (int i = 0; i < Visuals.Count; i++)
            {
                Visuals[i].transform.position = Vector3.Lerp(Source.transform.position, TargetPositions[i], (CurrentFrame - (BobDuration / 2)) / (Frames - (BobDuration / 2)));
                Visuals[i].transform.localScale = new Vector3(minionScale, minionScale, minionScale);
            }
        }
    }
}
