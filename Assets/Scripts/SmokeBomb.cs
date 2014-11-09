using UnityEngine;
using System.Collections;

public class SmokeBomb : DropItem {
	private float speed = 0.6f;

	void Update() {
		UpdateTime();
		BlowUp();
	}

	new void WasDroppedByPlayer(Player p) {
        base.WasDroppedByPlayer(p);
    }

    void BlowUp() {
    	if (lifeTime <= 0) {
			Destroy (this.gameObject);
		} else {
			transform.localScale += Vector3.one * 0.8f * speed * Time.deltaTime;

			if (lifeTime <= 4) {
				Color color = gameObject.renderer.material.color;
				color.a -= 0.01f;
				gameObject.renderer.material.color = color;
			}
		}
    }

    /*void OnTriggerEnter2D (Collider2D coll) {
		Player player = coll.GetComponent<Player>();

		if (player && time >= safetime) {
			player.KillPlayer();
		}
	}*/
}
