using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour {

	float timePassed = 0;
	float blinkTime = 0.75f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		timePassed += Time.deltaTime;

		if(timePassed > blinkTime) {
			
			GameObject pS = GameObject.FindGameObjectWithTag("pressStart");
			
			if(pS.guiText.enabled) {
				pS.guiText.enabled = false;
			} else {
				pS.guiText.enabled = true;
			}
			
			timePassed = 0;
		}

		if(Input.GetButtonDown("start")) {
			Application.LoadLevel("_SelectTeam");
		}

	}
}
