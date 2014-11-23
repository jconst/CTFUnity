using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DropItem : MonoBehaviour
{
    public Player owner;
    public float manaCost;
    public float time = 0;
    public float lifeTime = 3600f;

    public virtual bool TryDrop(Player player) {
        owner = player;
        return true;
    }

    public void Update() {
        UpdateTime();
        if (lifeTime <= 0) {
            Destroy (this.gameObject);
        }
    }

    public void UpdateTime() {
    	time += Time.deltaTime;
		lifeTime -= Time.deltaTime;
    }
}
