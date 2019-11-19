using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VA_MoveMinions : VisualAction
{
    public Match Model;
    public bool ToAction;

    public VA_MoveMinions(Match model, bool toAction)
    {
        Frames = DefaultFrames;
        Model = model;
        ToAction = toAction;

        foreach (Minion m in model.Minions) m.Visual.SourcePosition = m.Visual.transform.position;
    }

    public override void Update()
    {
        base.Update();
        foreach(Minion m in Model.Minions)
        {
            Vector3 targetPosition = ToAction ? Model.GetActionPosition(m) : Model.GetPlanPosition(m);
            m.Visual.transform.position = Vector3.Lerp(m.Visual.SourcePosition, targetPosition, CurrentFrame / Frames);
        }
    }

    public override void OnDone()
    {
        foreach(Minion m in Model.Minions)
        {
            Vector3 targetPosition = ToAction ? Model.GetActionPosition(m) : Model.GetPlanPosition(m);
            m.Visual.transform.position = targetPosition;
        }
    }
}
