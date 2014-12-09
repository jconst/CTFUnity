using UnityEngine;
using System.Collections;

public class TransformInfluence : MonoBehaviour {
	
	public bool PosX, PosY, PosZ;
	
	public float Strengh, Smooth, SmoothStrengh, MinL;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(PosX) {
			Smooth = Mathf.Min (SmoothStrengh * Time.deltaTime, 1.0f);
			transform.position = transform.position + new Vector3(Mathf.Lerp(MinL, Mathf.Cos(Strengh*Time.time), Smooth), 0.0f, 0.0f);
		}
		if(PosY) {
			Smooth = Mathf.Min (SmoothStrengh * Time.deltaTime, 1.0f);
			transform.position = transform.position + new Vector3(0.0f, Mathf.Lerp(MinL, Mathf.Cos(Strengh*Time.time), Smooth), 0.0f);
		}
		if(PosZ) {
			Smooth = Mathf.Min (SmoothStrengh * Time.deltaTime, 1.0f);
			transform.position = transform.position + new Vector3(0.0f, 0.0f, Mathf.Lerp(MinL, Mathf.Cos(Strengh*Time.time), Smooth));
		}
	}
}
