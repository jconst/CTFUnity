using UnityEngine;
using System.Collections;

public class SmokeBomb : DropItem {
	private float speed = 1f;
    private float fadeoutStartTime = 2f;

    void Start() {
        lifeTime = 6f;
    }

	new void Update() {
        base.Update();
		BlowUp();
	}
    
    void BlowUp() {
		transform.localScale += Vector3.one * speed * Time.deltaTime;

		if (lifeTime <= fadeoutStartTime) {
			Color color = gameObject.renderer.material.color;
			color.a -= (1f/fadeoutStartTime) * Time.deltaTime;
            Debug.Log(color.a);
			gameObject.renderer.material.color = color;
		}
    }

    void OnTriggerEnter2D(Collider2D coll){
        Bullet b = coll.GetComponent<Bullet>();
        if (b) {
            Destroy(b);
        }
    }
}
