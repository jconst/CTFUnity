using UnityEngine;
using System.Collections;

public class SelectTeam : MonoBehaviour {

	float timePassed = 0;
	float blinkTime = 0.75f;
	bool canPressStart = false;

	// Use this for initialization
	void Start () {
		GameObject p = GameObject.FindGameObjectWithTag("player1");
		p.renderer.enabled = false;

		p = GameObject.FindGameObjectWithTag("player2");
		p.renderer.enabled = false;

		p = GameObject.FindGameObjectWithTag("player3");
		p.renderer.enabled = false;

		p = GameObject.FindGameObjectWithTag("player4");
		p.renderer.enabled = false;

		GameObject pS = GameObject.FindGameObjectWithTag("pressStart");
		pS.guiText.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

		// Everytime a player hits 'A', add a controller that 
		if( Input.GetButtonDown("Item11") ) {
			GameObject p = GameObject.FindGameObjectWithTag("player1");
			if(!p.renderer.enabled) {
				p.renderer.enabled = true;
				PlayerOptions.numPlayers++;
			}
			if(p.transform.position.x < 0 || p.transform.position.x > 0) {
				if(p.transform.position.x < 0) {
					PlayerOptions.playerColors[0] = "Red";
				} else {
					PlayerOptions.playerColors[0] = "Blue";
				}
				p.renderer.material.color = Color.grey;
				PlayerOptions.ready++;
			}
		}
		if( Input.GetButtonDown("Item12") ) {
			GameObject p = GameObject.FindGameObjectWithTag("player2");
			if(!p.renderer.enabled) {
				p.renderer.enabled = true;
				PlayerOptions.numPlayers++;
			}
			if(p.transform.position.x < 0 || p.transform.position.x > 0) {
				if(p.transform.position.x < 0) {
					PlayerOptions.playerColors[1] = "Red";
				} else {
					PlayerOptions.playerColors[1] = "Blue";
				}
				p.renderer.material.color = Color.grey;
				PlayerOptions.ready++;
			}
		}
		if( Input.GetButtonDown("Item13") ) {
			GameObject p = GameObject.FindGameObjectWithTag("player3");
			if(!p.renderer.enabled) {
				p.renderer.enabled = true;
				PlayerOptions.numPlayers++;
			}
			if(p.transform.position.x < 0 || p.transform.position.x > 0) {
				if(p.transform.position.x < 0) {
					PlayerOptions.playerColors[2] = "Red";
				} else {
					PlayerOptions.playerColors[2] = "Blue";
				}
				p.renderer.material.color = Color.grey;
				PlayerOptions.ready++;
			}
		}
		if( Input.GetButtonDown("Item14") ) {
			GameObject p = GameObject.FindGameObjectWithTag("player4");
			if(!p.renderer.enabled) {
				p.renderer.enabled = true;
				PlayerOptions.numPlayers++;
			}
			if(p.transform.position.x < 0 || p.transform.position.x > 0) {
				if(p.transform.position.x < 0) {
					PlayerOptions.playerColors[3] = "Red";
				} else {
					PlayerOptions.playerColors[3] = "Blue";
				}
				p.renderer.material.color = Color.grey;
				PlayerOptions.ready++;
			}
		}

		// they can move left and right to pick a team
		// p1
		if(Input.GetAxis("HorizontalL1") > 0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player1");
			if(p.renderer.enabled && PlayerOptions.playerColors[0] == "") {
				Vector3 i = p.transform.position;
				i.x = 3;
				p.transform.position = i;
			}
		} else if(Input.GetAxis("HorizontalL1") < -0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player1");
			if(p.renderer.enabled && PlayerOptions.playerColors[0] == "") {
				Vector3 i = p.transform.position;
				i.x = -3;
				p.transform.position = i;
			}
		}
		// p2
		if(Input.GetAxis("HorizontalL2") > 0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player2");
			if(p.renderer.enabled && PlayerOptions.playerColors[1] == "") {
				Vector3 i = p.transform.position;
				i.x = 3;
				p.transform.position = i;
			}
		} else if(Input.GetAxis("HorizontalL2") < -0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player2");
			if(p.renderer.enabled && PlayerOptions.playerColors[1] == "") {
				Vector3 i = p.transform.position;
				i.x = -3;
				p.transform.position = i;
			}
		}
		// p3
		if(Input.GetAxis("HorizontalL3") > 0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player3");
			if(p.renderer.enabled && PlayerOptions.playerColors[2] == "") {
				Vector3 i = p.transform.position;
				i.x = 3;
				p.transform.position = i;
			}
		} else if(Input.GetAxis("HorizontalL3") < -0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player3");
			if(p.renderer.enabled && PlayerOptions.playerColors[2] == "") {
				Vector3 i = p.transform.position;
				i.x = -3;
				p.transform.position = i;
			}
		}
		// p4
		if(Input.GetAxis("HorizontalL4") > 0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player4");
			if(p.renderer.enabled && PlayerOptions.playerColors[3] == "") {
				Vector3 i = p.transform.position;
				i.x = 3;
				p.transform.position = i;
			}
		} else if(Input.GetAxis("HorizontalL4") < -0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player4");
			if(p.renderer.enabled && PlayerOptions.playerColors[3] == "") {
				Vector3 i = p.transform.position;
				i.x = -3;
				p.transform.position = i;
			}
		}

		// Blink Press Start when teams are ready
		if(PlayerOptions.numPlayers >= 2 && (PlayerOptions.ready == PlayerOptions.numPlayers) ) {
			canPressStart = true;
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
		}

		// Press Start Handler
		if(Input.GetButtonDown("start") && canPressStart) {
			Application.LoadLevel("_Scene1");
		}

	}
}
