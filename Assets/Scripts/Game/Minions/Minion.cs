using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Minion : Creature
{
    public string Text;
    public int OrderNum;
    public Player Owner { get; private set; }
    public MinionType Type;
    public bool Destabilized;
    public bool HasSummonProtection { get; private set; }

    public Minion(Match model, Player owner, Player enemy, int orderNum) : base(model)
    {
        Owner = owner;
        Enemy = enemy;
        OrderNum = orderNum;
    }

    public void SetSummonProtection(Match match, bool protection)
    {
        HasSummonProtection = protection;
        if (match.Visual) ((VisualMinion)Visual).SetHaloEnabled(protection);
    }

    public void SetOwner(Player player)
    {
        Owner = player;
        Enemy = player.Enemy;
    }

    public abstract void Action();
}
