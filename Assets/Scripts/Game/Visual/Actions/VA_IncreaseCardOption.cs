using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VA_IncreaseCardOption : VisualAction
{
    public VisualEntity Visual;
    ParticleSystem ParticleSystem;

    public VA_IncreaseCardOption(VisualEntity visual, MatchUI UI)
    {
        Frames = DefaultFrames * 1.5f;
        Visual = visual;
        ParticleSystem = GameObject.Instantiate(UI.PS_CardOptionIncrease, visual.transform.position, Quaternion.identity);
        var main = ParticleSystem.main;
        main.duration = Frames;
        ParticleSystem.Play();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnDone()
    {
        base.OnDone();
        GameObject.Destroy(ParticleSystem.gameObject);
    }
}
