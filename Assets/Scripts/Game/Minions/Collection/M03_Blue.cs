using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M03_Blue : Minion
{

    public M03_Blue(Match model, Player owner, Player enemy, int orderNum) : base(model, owner, enemy, orderNum)
    {
        Name = "Blue";
        Text = "Destroy a random enemy minion.";
        Type = MinionType.Blue;
        Color = new Color(0.2f, 0.2f, 1);
    }

    public override void Action()
    {
        Model.DestabilizeMinion(this, Model.RandomMinionFromPlayer(Enemy, withoutDestabilized: true));
    }
}
