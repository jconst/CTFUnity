using UnityEngine;
using System.Collections;

public class SpawnPad : MonoBehaviour {

	public Player owner;

    void OnTriggerEnter2D(Collider2D coll) {
        Player player = coll.GetComponent<Player>();
        if (player && player.team != owner.team) {
            owner.InvalidateSpawn();
            Destroy(gameObject);
        }
    }
}
