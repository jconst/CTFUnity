using UnityEngine;
using System.Collections;

public class Paralax : MonoBehaviour {

	MainCamera mc;
	float scale=0.25f;
	// Use this for initialization
	void Start () {
		mc = Camera.main.GetComponent<MainCamera> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 p = transform.position;
		p += mc.deltaPos*scale;
		transform.position = p;
	}
}
