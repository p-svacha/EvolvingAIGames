using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M02_Yellow : Minion
{

    public M02_Yellow(Match model, Player owner, Player enemy, int orderNum) : base(model, owner, enemy, orderNum)
    {
        Name = "Yellow";
        Text = "Restore 2 health.";
        Type = MinionType.Yellow;
        Color = Color.yellow;
    }

    public override void Action()
    {
        Model.Heal(this, Owner, 2);
    }
}
