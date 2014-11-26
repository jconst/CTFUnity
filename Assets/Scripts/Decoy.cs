using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Decoy : DropItem
{
    public PhysicsMaterial2D physMat;
    Player fakePlayer;
    DecoyInput input;

    public Vector2 target = Vector2.zero;

    public override bool TryDrop(Player p) {
        base.TryDrop(p);
        
        lifeTime = 6f;
        CreateFakePlayer(p);
        return true;
    }

    public void CreateFakePlayer(Player p) {
        GameObject go = Instantiate(owner.gameObject) as GameObject;
        fakePlayer = go.GetComponent<Player>();

        fakePlayer.collider2D.sharedMaterial = physMat;
        // fakePlayer.canGrabFlag = false;
        fakePlayer.inputCtrl = input = new DecoyInput(this);
    }

    public new void Update() {
        base.Update();
        LockToFakePlayer();
        FindTarget();
    }

    void LockToFakePlayer() {
        transform.localScale = fakePlayer.transform.localScale * 2f;
        transform.rotation = fakePlayer.transform.rotation;
        transform.position = fakePlayer.transform.position;
    }

    void FindTarget() {
        Flag flag = Manager.S.flag;
        Vector2 newTarget = (flag.carrier == null || flag.carrier.team != fakePlayer.team)
                          ? flag.transform.position
                          : Manager.S.teamBases[fakePlayer.otherTeam].transform.position;
        if (target != newTarget) {
            CalculatePath(newTarget);
        }
        target = newTarget;
    }

    void CalculatePath(Vector3 t) {
        NavMesh.CalculatePath(transform.position, convertXYToXZ(t), -1, input.path);
        input.UpdateWaypoints();
    }

    public override void Deactivate() {
        Destroy(fakePlayer.gameObject);
        base.Deactivate();
    }

    Vector3 convertXYToXZ(Vector3 v) {
        return new Vector3(v.x, 0, v.y);
    }
}
