using UnityEngine;
using System.Collections;

public class ReactorRingRotate : MonoBehaviour {
	
	public float RotSpeed;
	public Vector3 Axis;

	void Start () {
	
	}
	
	void Update () {
		transform.RotateAround(Axis, RotSpeed * Time.deltaTime);
	}
}
