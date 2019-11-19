using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VA_MinionDeaths : VisualAction
{
    public List<VisualEntity> Visuals;
    public float SourceScale;

    public VA_MinionDeaths(List<VisualEntity> visuals, float sourceScale)
    {
        Frames = DefaultFrames;
        Visuals = visuals;
        SourceScale = sourceScale;
    }

    public override void Update()
    {
        base.Update();
        foreach(VisualEntity vis in Visuals)
        {
            float minionScale = SourceScale - (SourceScale / Frames * CurrentFrame);
            vis.transform.localScale = new Vector3(minionScale, minionScale, minionScale);
        }
    }

    public override void OnDone()
    {
        base.OnDone();
        foreach(VisualEntity vis in Visuals)
        {
            GameObject.Destroy(vis.gameObject);
        }
    }
}
