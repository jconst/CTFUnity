using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Decoy : DropItem
{
    Player fakePlayer;

    public override bool TryDrop(Player p) {
        base.TryDrop(p);
        
        lifeTime = 6f;
        CreateFakePlayer(p);
        return true;
    }

    public void CreateFakePlayer(Player p) {
        GameObject go = Instantiate(owner.gameObject) as GameObject;
        go.transform.position += (Vector3)p.lastInputVelocity.normalized;
        fakePlayer = go.GetComponent<Player>();

        fakePlayer.canGrabFlag = false;
        fakePlayer.inputCtrl = new DecoyInput(p);
    }

    public new void Update() {
        base.Update();
        transform.position = fakePlayer.transform.position;
    }

    void OnDestroy() {
        Destroy(fakePlayer.gameObject);
    }
}
