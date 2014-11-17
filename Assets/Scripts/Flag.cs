using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Flag : MonoBehaviour
{
	Player carrier;
	Vector3 initialPosit;

	const float timeLimit = 3f;
	float countdown = timeLimit;

	// Use this for initialization
	void Start () {
		initialPosit = transform.position;
	}

	// Update is called once per frame
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
		carrier = p;
		p.flag = this;
	}

	void Score()
	{
		Manager.S.DidScore(carrier);
	}

	public void Drop()
	{
		if (carrier)
			carrier.flag = null;
		carrier = null;
		countdown = timeLimit;
	}

	public void Reset()
	{
		transform.position = initialPosit;
		carrier = null;
		countdown = timeLimit;
	}

	public void OnTriggerEnter2D(Collider2D coll) {
		CheckPickup(coll);
	}

	public void OnTriggerStay2D(Collider2D coll) {
		ScoreZone zone = coll.GetComponent<ScoreZone>();
		if (zone && carrier && carrier.team != zone.team) {
			countdown -= Time.deltaTime;
		}
	}

	public void OnTriggerExit2D(Collider2D coll){
		ScoreZone zone = coll.GetComponent<ScoreZone>();
		if (zone) {
			countdown = timeLimit;
		}
	}

	void CheckPickup(Collider2D coll) {
		Player player = coll.GetComponent<Player>();
		if (carrier == null && player) {
			Pickup(player);
		}
	}
}
