using UnityEngine;
using System.Collections;

public class SpawnPad : DropItem
{
    public override bool TryDrop(Player p) {
        base.TryDrop(p);
        if (p.spawnpoint) 
            Destroy(p.spawnpoint);
        p.spawnpoint = this;
        owner = p;
        return true;
    }
}
