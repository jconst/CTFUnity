using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Flag : MonoBehaviour
{
	public Player carrier;
	Vector3 initialPosit;
	ProgressBar progress;

	const float timeLimit = 3f;
	float countdown = timeLimit;

	void Start () {
		initialPosit = transform.position;
	}

	void Update () {
		if (carrier != null) {
			if (!carrier.carrying) {
				carrier = null;
			} else {
				transform.position = carrier.transform.position;
			}
		}
		if (countdown < 0) 
		{
			Score();
		}
	}

	public void Pickup(Player p)
	{
		if (p.canGrabFlag) {
			carrier = p;
			p.flag = this;
		}
	}

	void Score()
	{
		Manager.S.DidScore(carrier);
		particleSystem.Play();
		renderer.enabled = false;
		countdown = 100f; //stop repeated scoring while game reloads
	}

	public void Drop()
	{
		if (carrier)
			carrier.flag = null;
		carrier = null;
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

		if (progress) 
			progress.Reset();
	}

	public void OnTriggerEnter2D(Collider2D coll) {
		CheckPickup(coll);
		ScoreZone zone = coll.GetComponent<ScoreZone>();
		if (zone && carrier && carrier.team != zone.team) {
			StartProgressBar(coll);
		}
	}

	public void OnTriggerStay2D(Collider2D coll) {
		ScoreZone zone = coll.GetComponent<ScoreZone>();
		if (zone && carrier && carrier.team != zone.team) {
			countdown -= Time.deltaTime;
			progress.countdown -= Time.deltaTime;
		}
	}

	public void OnTriggerExit2D(Collider2D coll){
		ScoreZone zone = coll.GetComponent<ScoreZone>();
		if (zone) {
			countdown = timeLimit;
			GameObject pb = GameObject.FindWithTag("ProgressBar");
			Destroy(pb);
		}
	}

	void CheckPickup(Collider2D coll) {
		Player player = coll.GetComponent<Player>();
		if (carrier == null && player) {
			Pickup(player);
		}
	}

	void StartProgressBar(Collider2D coll) {
		GameObject go = (GameObject)Instantiate(Resources.Load("ProgressBar"));
		progress = go.GetComponent<ProgressBar>();
		
		progress.timeLimit = timeLimit;
		progress.countdown = timeLimit;
		
		Vector3 temp = coll.transform.position;
		temp.y -= 1.5f;
		progress.transform.position = temp;
	}
}
