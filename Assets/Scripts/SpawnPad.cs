using UnityEngine;
using System.Collections;

public class SpawnPad : DropItem
{
    // void OnTriggerEnter2D(Collider2D coll) {
    //     Player player = coll.GetComponent<Player>();
    //     if (player && player.team != owner.team) {
    //         owner.InvalidateSpawn();
    //         Destroy(gameObject);
    //     }
    // }

    public override void WasDroppedByPlayer(Player p) {
        base.WasDroppedByPlayer(p);
        if (p.spawnpoint) 
            Destroy(p.spawnpoint);
        p.spawnpoint = this;
        owner = p;
    }
}
