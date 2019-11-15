using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VA_DealDamage : VisualProjectileAction
{
    public VA_DealDamage(VisualEntity source, VisualEntity target, int amount, Color color) : base(source, target, amount, color)
    {
        Frames = 80;
        ProjectTileBaseSize = 0.05f;
        BobDurationRel = 0.5f;
        BobExtraScale = 1.5f;
        Init();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnDone()
    {
        base.OnDone();
    }
}
