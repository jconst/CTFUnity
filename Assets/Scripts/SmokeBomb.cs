using UnityEngine;
using System.Collections;

public class SmokeBomb : DropItem {
	private float speed = 1f;
    private float fadeoutStartTime = 2f;

	void Update() {
		UpdateTime();
		BlowUp();
	}

	new void WasDroppedByPlayer(Player p) {
        base.WasDroppedByPlayer(p);
    }

    void BlowUp() {
    	if (lifeTime <= 0) {
			Destroy(gameObject);
		}
		transform.localScale += Vector3.one * speed * Time.deltaTime;

		if (lifeTime <= fadeoutStartTime) {
			Color color = gameObject.renderer.material.color;
			color.a -= (1f/fadeoutStartTime) * Time.deltaTime;
            Debug.Log(color.a);
			gameObject.renderer.material.color = color;
		}
    }
}
