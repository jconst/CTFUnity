using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Decoy : DropItem
{
    public PhysicsMaterial2D physMat;
    Player fakePlayer;
    DecoyInput input;

    public override bool TryDrop(Player p) {
        base.TryDrop(p);
        
        lifeTime = 6f;
        CreateFakePlayer(p);
        return true;
    }

    public void CreateFakePlayer(Player p) {
        GameObject go = Instantiate(owner.gameObject) as GameObject;
        Vector2 startDirection = p.lastInputVelocity.normalized;
        go.transform.position += (Vector3)startDirection * 0.5f;
        fakePlayer = go.GetComponent<Player>();

        fakePlayer.collider2D.sharedMaterial = physMat;
        fakePlayer.canGrabFlag = false;
        fakePlayer.inputCtrl = input = new DecoyInput(fakePlayer, startDirection);
    }

    public new void Update() {
        base.Update();
        transform.localScale = fakePlayer.transform.localScale * 2f;
        transform.rotation = fakePlayer.transform.rotation;
        transform.position = fakePlayer.transform.position;
    }

    public void OnTriggerStay2D(Collider2D coll) {
        if (coll.gameObject != owner.gameObject &&
            coll.gameObject != fakePlayer.gameObject &&
            coll.gameObject.tag != "Bomb") {
            Debug.Log(coll.gameObject.name);
            input.Collision();
        }
    }

    public override void Deactivate() {
        Destroy(fakePlayer.gameObject);
        base.Deactivate();
    }
}
