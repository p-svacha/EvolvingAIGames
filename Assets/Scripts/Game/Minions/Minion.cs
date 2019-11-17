using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Minion : Creature
{
    public string Text;
    public int OrderNum;
    public Player Owner;
    public MinionType Type;
    public bool Destabilized;
    public bool HasSummonProtection;

    public Minion(Match model, Player owner, Player enemy, int orderNum) : base(model)
    {
        Owner = owner;
        Enemy = enemy;
        OrderNum = orderNum;
    }

    public abstract void Action();
}
