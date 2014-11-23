using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DecoyInput : InputControl
{
    Vector2 direction;
    Vector2 wobble;
    float timeLastDirectionChange = 0f;

    public DecoyInput(Player decoyPlayer, Vector2 startDirection) {
        direction = wobble = startDirection.normalized;
    }

    public override Vector2 RunVelocity(int playerNum)
    {
        wobble = ConstrainedRandomDirection(0.2f, 30f, wobble);
        return wobble;
    }

    public override Vector2 TackleVelocity(int playerNum)
    {
        return Vector2.zero;
    }

    public override bool ItemButtonDown(int playerNum, int itemNum)
    {
        return false;
    }

    public void Collision() {
        direction = wobble = ConstrainedRandomDirection(0.01f, 90f, direction);
    }

    public Vector2 ConstrainedRandomDirection(float timeBetweenChanges, float maxAngleChange, Vector2 basis)
    {
        if (Time.time - timeLastDirectionChange > timeBetweenChanges) {
            timeLastDirectionChange = Time.time;
            return Quaternion.AngleAxis((Random.value-0.5f)*maxAngleChange, Vector3.forward) * basis;
        }
        return basis;
    }
}
