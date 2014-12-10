using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour {
	public string team;
	public float timeLimit;
	float fullWidth = 2.5f;

	void Update() {
		float curWidth = fullWidth * (1-(Manager.S.flag.countdown/Flag.timeLimit));
			Vector3 ls = transform.localScale;
	 		ls.x += progressAmount;
 			transform.localScale = ls;
		}
	}

	public void Reset() {
		Vector3 ls = transform.localScale;
 		ls.x = curWidth;
		transform.localScale = ls;
	}
}