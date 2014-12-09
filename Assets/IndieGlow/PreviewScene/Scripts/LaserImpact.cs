using UnityEngine;
using System.Collections;

public class LaserImpact : MonoBehaviour {
	
	private float LifeTime;
	private float TimeInScene;
	
	void Start () {
		LifeTime = 5.0f;
		TimeInScene = Time.time;
	}
	
	void Update () {
		if(Time.time >= TimeInScene + LifeTime ) {
			Destroy(this.gameObject);
		}
	}
}
