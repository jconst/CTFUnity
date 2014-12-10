using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour {
	public string team;
	public float timeLimit;
	public float countdown;
	float fullWidth = 2.5f;

	void Update() {
 		if (transform.localScale.x > fullWidth) {
			Destroy(this.gameObject);
		} else {
			float progressAmount = fullWidth*Time.deltaTime/timeLimit;

			Vector3 ls = transform.localScale;
	 		ls.x += progressAmount;
 			transform.localScale = ls;
		}
	}

	public void Reset() {
		Vector3 ls = transform.localScale;
		ls.x = 0;
		transform.localScale = ls;
	}
}