using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Decoy : DropItem
{
    bool drawPath = false;
    public PhysicsMaterial2D physMat;
    Player fakePlayer;
    DecoyInput input;
    LineRenderer line;
    Vector2 target = new Vector2 (-1000, -1000);

    public override bool TryDrop(Player p) {
        base.TryDrop(p);
        
        line = GetComponent<LineRenderer>();
        // lifeTime = 10f;
        CreateFakePlayer(p);
        return true;
    }

    public void CreateFakePlayer(Player p) {
        GameObject go = Instantiate(owner.gameObject) as GameObject;
        fakePlayer = go.GetComponent<Player>();

        fakePlayer.collider2D.sharedMaterial = physMat;
        // fakePlayer.canGrabFlag = false;
        fakePlayer.inputCtrl = input = new DecoyInput(this);
        Manager.S.allPlayers.Add(fakePlayer);
    }

    public new void Update() {
        base.Update();
        LockToFakePlayer();
        FindTarget();
    }

    void LockToFakePlayer() {
        transform.position = fakePlayer.transform.position;
    }

    void FindTarget() {
        Flag flag = Manager.S.flag;
        Vector2 newTarget = (flag.carrier == null || flag.carrier.team != fakePlayer.team)
                          ? flag.transform.position
                          : Manager.S.teamBases[fakePlayer.otherTeam].transform.position;
        if ((target - newTarget).magnitude > 0.5f) {
            CalculateNewPath(newTarget);
        }
        target = newTarget;
    }

    void CalculateNewPath(Vector3 t) {
        NavMesh.CalculatePath(transform.position.convertXYToXZ(), t.convertXYToXZ(), -1, input.path);
        if (drawPath) {
            line.SetVertexCount(input.path.corners.Length+1);
            line.SetPosition(0, transform.position);
            input.path.corners.Each((pt, i) => {
                line.SetPosition(i+1, pt.convertXZToXY());
            });
        }
        input.UpdateWaypoints();
    }

    public override void Deactivate() {
        Manager.S.allPlayers.Remove(fakePlayer);
        fakePlayer.Die();
        Destroy(fakePlayer.gameObject, 1);
        base.Deactivate();
    }

    void OnTriggerStay2D(Collider2D coll){
        Player p = coll.GetComponent<Player>();
        if (p != null && p.team != fakePlayer.team) {
            if (p.dead) {
                input.tackleForce = Vector2.zero;
            } else {
                //tackle him
                input.tackleForce = (p.transform.position - fakePlayer.transform.position).normalized;
            }
        }
    }

    void OnTriggerExit2D(Collider2D coll){
        Player p = coll.GetComponent<Player>();
        if (p != null && p.team != fakePlayer.team) {
            input.tackleForce = Vector2.zero;
        }
    }
}
