using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {
	
	private float Speed;
	private float LifeTime;
	private float TimeInScene;

	void Start () {
		Speed = 12.0f;
		LifeTime = 5.0f;
		TimeInScene = Time.time;
	}
	
	void Update () {
		transform.Translate(transform.forward * Speed * Time.deltaTime, Space.World);
		
		if(Time.time >= TimeInScene + LifeTime ) {
			Destroy(this.gameObject);
		}
	}
}
