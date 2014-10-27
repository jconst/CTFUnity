using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {
	private Vector3 startPos;
	public Vector2 heading = Vector2.up;
	private float turnChance = 0.1f;
	private float speed = 4f;
	private float radius = 1f;
	float safetime = 1f;
	float time=0f;
	private float lifeTime = 5f;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
		lifeTime += safetime;
	}
	
	// Update is called once per frame
	void Update () {
		lifeTime -= Time.deltaTime;
		time += Time.deltaTime;
		if (lifeTime <= 0) {
			Destroy (this.gameObject);
		} else {
			Move();		
		}
		
	}

	void FixedUpdate () {

	}

	void Move() {
		if (Random.value < turnChance) {
			heading = RandomDir();		
		}
		
		Vector3 newDir = (Vector3)((heading) * speed * Time.deltaTime);		
		if (!OutsideBounds(transform.position + newDir)) {
			transform.position += newDir;
		} else {
			transform.position -= newDir;
		}
			
	}

	Vector3 RandomDir() {
		System.Random rand = new System.Random();
		Vector3 heading = new Vector3(rand.Next(-1, 2), rand.Next(-1, 2), 0f);
		heading.Normalize();
		return heading;
	}

	bool OutsideBounds(Vector3 newPos) {
		if (Mathf.Abs(newPos.x - startPos.x) > radius) {
			return true;
		} else if (Mathf.Abs(newPos.y - startPos.y) > radius) {
			return true;
		} else return false;
	}

	void OnTriggerEnter (Collider coll) {
		GameObject collObject = coll.gameObject;
		Player player = collObject.GetComponent<Player>();

		print(coll);

		if (player&&time>=safetime) {
			player.KillPlayer();
		}
	}
}
