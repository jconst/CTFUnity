using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Flag : MonoBehaviour
{
	public Player carrier;
	Vector3 initialPosit;
	ProgressBar progress;
	ScoreZone currentScoreZone;
	private List<GameObject> pollens = 
	    new List<GameObject>();

	static public float timeLimit = 3f;
	public float countdown = timeLimit;
	float soundtimah;
	bool pollenating;

	void Start () {
		initialPosit = transform.position;
		soundtimah = 0f;
	}

	void Update () {
		if (carrier != null) {
			if (!carrier.carrying) {
				carrier = null;
			} else {
				Vector3 newPos = (Vector2)carrier.transform.position + (carrier.heading * 0.7f);
				newPos.z = initialPosit.z;
				transform.position = newPos;
			}
		}
		if (countdown <= 0) {
			Score();
		} else if (pollenating) {
			countdown -= Time.deltaTime;
		} else if (countdown <= timeLimit) {
			countdown += Time.deltaTime;
		}
	}

	void FixedUpdate() {
		UpdatePollenateEffect();
	}

	public void Pickup(Player p)
	{
		if (p.canGrabFlag) {
			carrier = p;
			p.flag = this;
			AudioManager.Main.PlayNewSound("Mana");
		}
	}

	void Score()
	{
		Manager.S.DidScore(carrier);
		PlayExplodeEffect();
		countdown = timeLimit; //stop repeated scoring while game reloads
	}

	public void Drop()
	{
		if (carrier)
			carrier.flag = null;
		carrier = null;
		pollenating = false;
		countdown = timeLimit;
		GameObject pb = GameObject.FindWithTag("ProgressBar");
		Destroy(pb);
	}

	public void Reset()
	{
		transform.position = initialPosit;
		carrier = null;
		countdown = timeLimit;
		renderer.enabled = true;
	}

	public void OnTriggerEnter2D(Collider2D coll) {
		CheckPickup(coll);
		ScoreZone zone = coll.GetComponent<ScoreZone>();
		if (zone && carrier && carrier.team == zone.team) {
			currentScoreZone = zone;
			StartProgressBar(coll);
			pollenating = true;
		}
	}

	public void OnTriggerExit2D(Collider2D coll){
		ScoreZone zone = coll.GetComponent<ScoreZone>();
		if (zone) {
			pollenating = false;
		}
	}

	void CheckPickup(Collider2D coll) {
		Player player = coll.GetComponent<Player>();
		if (carrier == null && player) {
			Pickup(player);
		}
	}

	void StartProgressBar(Collider2D coll) {
		if (!progress) {
			GameObject go = (GameObject)Instantiate(Resources.Load("ProgressBar"));
			progress = go.GetComponent<ProgressBar>();
		}
		
		Vector3 temp = coll.transform.position;
		temp.y += 2f;
		progress.transform.position = temp;
	}

	void PlayExplodeEffect() {
		particleSystem.Play();
	}

	void UpdatePollenateEffect() {
		if (pollenating) {
			EmitPollenParticle ();
			if (soundtimah > 0.28) {
				AudioManager.Main.PlayNewSound ("thrust");
				soundtimah = 0;
			}
			soundtimah += Time.fixedDeltaTime;
		} else
			soundtimah = 0;

		const float speed = 2;
		pollens.RemoveAll(p => {
			Vector2 toTarget = (Vector2)currentScoreZone.transform.position - (Vector2)p.transform.position;
			p.rigidbody2D.velocity = Vector2.Lerp(p.rigidbody2D.velocity.normalized, toTarget.normalized, 0.2f) * speed;
			p.transform.localScale *= 0.98f;
			if (toTarget.magnitude <= 0.05) {
				Destroy(p);
				return true;
			}
			return false;
		});
	}

	void EmitPollenParticle() {
		float size = Extensions.Rand;
		GameObject particle = Instantiate(Resources.Load("PollenParticle")) as GameObject;
		particle.transform.position = transform.position;
		particle.transform.localScale = new Vector3(size, size, size);
		particle.rigidbody2D.velocity = Quaternion.Euler(0, 0, Extensions.Rand*360f) * Vector2.up; //rand direction
		particle.transform.parent = currentScoreZone.transform;
		pollens.Add(particle);
	}
}
