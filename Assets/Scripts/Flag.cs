using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour {

	public string color;
	GameObject carrier;
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

	public void Pickup(GameObject go)
	{
		carrier = go;
		following = true;

	}

	public void Reset()
	{
		following = false;
		transform.position = initialPosit;
		carrier = null;
	}
}
