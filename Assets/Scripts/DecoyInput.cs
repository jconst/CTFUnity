using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DecoyInput : InputControl
{
    Vector2 direction;

    public DecoyInput(Player p) {
        direction = p.lastInputVelocity.normalized;
    }

    override public Vector2 RunVelocity(int playerNum)
    {
        return direction;
    }

    override public Vector2 TackleVelocity(int playerNum)
    {
        return Vector2.zero;
    }

    override public bool ItemButtonDown(int playerNum, int itemNum)
    {
        return false;
    }
}
