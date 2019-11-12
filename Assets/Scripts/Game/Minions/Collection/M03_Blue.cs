using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M03_Blue : Minion
{

    public M03_Blue(MatchModel model, Player owner, Player enemy, int orderNum) : base(model, owner, enemy, orderNum)
    {
        Name = "Blue";
        Text = "Destroy a random enemy minion.";
        Type = MinionType.Blue;
        Color = Color.blue;
    }

    public override void Action()
    {
        Model.DestroyRandomMinion(this, Enemy);
    }
}
