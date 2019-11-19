using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M05_Grey : Minion
{

    public M05_Grey(Match model, Player owner, Player enemy, int orderNum) : base(model, owner, enemy, orderNum)
    {
        Name = "Grey";
        Text = "Do nothing.";
        Type = MinionType.Grey;
        Color = new Color(0.8f, 0.8f, 0.8f);
    }

    public override void Action()
    {
    }
}
