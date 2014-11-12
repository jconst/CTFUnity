using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DropItem : MonoBehaviour
{
    public Player owner;
    public float manaCost;
    public float time = 0;
    public float lifeTime = 6f;
    public float safetime = 1f;

    public virtual void WasDroppedByPlayer(Player player) {
        owner = player;
    }

    public void UpdateTime() {
    	time += Time.deltaTime;
		lifeTime -= Time.deltaTime;
    }
}
