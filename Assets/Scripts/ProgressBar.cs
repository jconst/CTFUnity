using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour {
//	float progressAmount = 0.05f;
	//public Player player;
	public string team;
	public float timeLimit;
	public float countdown;
	public float fullWidth = 2f;

	void Update() {
 		if (transform.localScale.x > fullWidth) {
//print(" before didScore");
			Destroy(this.gameObject);
		} else {
			float progressAmount = fullWidth*Time.deltaTime/timeLimit;

			Vector3 ls = transform.localScale;
	 		ls.x += progressAmount;
 			transform.localScale = ls;

	 		//Vector3 v = transform.localPosition;
	 		//v.x += progressAmount/2;
	 		//transform.localPosition = v;
		}
	}

	public void Reset() {
		Vector3 ls = transform.localScale;
		ls.x = 0;
		transform.localScale = ls;

		//Vector3 v = transform.localPosition;
		//v.x -= 1/2;
		//transform.localPosition = v;
	}
}