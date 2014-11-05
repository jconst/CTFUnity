using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour
{
	Player carrier;
	Vector3 initialPosit;
	bool following=false;

	// Use this for initialization
	void Start () {
		initialPosit = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (following) {
			transform.position = carrier.transform.position;
		}
	}

	public void Pickup(Player p)
	{
		carrier = p;
		following = true;
		p.flag = this;
		p.carrying=true;
	}

	public void Reset()
	{
		following = false;
		transform.position = initialPosit;
		carrier = null;
	}

	public void OnTriggerEnter2D(Collider2D coll)
	{
		Player player = coll.GetComponent<Player>();
		if (player) {
			Pickup(player);
		}
	}
}
