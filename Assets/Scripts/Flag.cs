using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Flag : MonoBehaviour
{
	Player carrier;
	Vector3 initialPosit;

	const float timeLimit=5f;
	float countdown = timeLimit;

	// Use this for initialization
	void Start () {
		initialPosit = transform.position;
	}

	// Update is called once per frame
	void Update () {
		if (carrier) {
			transform.position = carrier.transform.position;
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
		p.carrying=true;
	}

	void Score()
	{
		Manager.S.DidScore(carrier, this);
	}

	public void Drop()
	{
		carrier = null;
		countdown = timeLimit;
	}

	public void Reset()
	{
		transform.position = initialPosit;
		carrier = null;
		countdown = timeLimit;
	}

	public void OnTriggerEnter2D(Collider2D coll)
	{
		Player player = coll.GetComponent<Player>();
		if (player) {
			Pickup(player);
		}
	}

	public void OnTriggerStay2D(Collider2D coll){
		ScoreZone zone = coll.GetComponent<ScoreZone>();
		if (zone) {
			countdown -= Time.deltaTime;
		}
	}

	public void OnTriggerExit2D(Collider2D coll){
		ScoreZone zone = coll.GetComponent<ScoreZone>();
		if (zone) {
			countdown = timeLimit;
		}
	}
}
