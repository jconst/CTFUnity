using UnityEngine;
using System.Collections;

public class SelectTeam : MonoBehaviour {

	public static int numPlayers = 0;
	public static int ready = 0;
	public static string p1Color = "";
	public static string p2Color = "";
	public static string p3Color = "";
	public static string p4Color = "";

	float timePassed = 0;
	float blinkTime = 0.75f;

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
				numPlayers++;
			}
			if(p.transform.position.x < 0 || p.transform.position.x > 0) {
				if(p.transform.position.x < 0) {
					p1Color = "red";
				} else {
					p1Color = "blue";
				}
				p.renderer.material.color = Color.grey;
				ready++;
			}
		}
		if( Input.GetButtonDown("Item21") ) {
			GameObject p = GameObject.FindGameObjectWithTag("player2");
			if(!p.renderer.enabled) {
				p.renderer.enabled = true;
				numPlayers++;
			}
			if(p.transform.position.x < 0 || p.transform.position.x > 0) {
				if(p.transform.position.x < 0) {
					p2Color = "red";
				} else {
					p2Color = "blue";
				}
				p.renderer.material.color = Color.grey;
				ready++;
			}
		}
		if( Input.GetButtonDown("Item31") ) {
			GameObject p = GameObject.FindGameObjectWithTag("player3");
			if(!p.renderer.enabled) {
				p.renderer.enabled = true;
				numPlayers++;
			}
			if(p.transform.position.x < 0 || p.transform.position.x > 0) {
				if(p.transform.position.x < 0) {
					p3Color = "red";
				} else {
					p3Color = "blue";
				}
				p.renderer.material.color = Color.grey;
				ready++;
			}
		}
		if( Input.GetButtonDown("Item41") ) {
			GameObject p = GameObject.FindGameObjectWithTag("player4");
			if(!p.renderer.enabled) {
				p.renderer.enabled = true;
				numPlayers++;
			}
			if(p.transform.position.x < 0 || p.transform.position.x > 0) {
				if(p.transform.position.x < 0) {
					p4Color = "red";
				} else {
					p4Color = "blue";
				}
				p.renderer.material.color = Color.grey;
				ready++;
			}
		}

		// they can move left and right to pick a team
		// p1
		if(Input.GetAxis("HorizontalL1") > 0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player1");
			if(p.renderer.enabled && p1Color == "") {
				Vector3 i = p.transform.position;
				i.x = 3;
				p.transform.position = i;
			}
		} else if(Input.GetAxis("HorizontalL1") < -0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player1");
			if(p.renderer.enabled && p1Color == "") {
				Vector3 i = p.transform.position;
				i.x = -3;
				p.transform.position = i;
			}
		}
		// p2
		if(Input.GetAxis("HorizontalL2") > 0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player2");
			if(p.renderer.enabled && p2Color == "") {
				Vector3 i = p.transform.position;
				i.x = 3;
				p.transform.position = i;
			}
		} else if(Input.GetAxis("HorizontalL2") < -0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player2");
			if(p.renderer.enabled && p2Color == "") {
				Vector3 i = p.transform.position;
				i.x = -3;
				p.transform.position = i;
			}
		}
		// p3
		if(Input.GetAxis("HorizontalL3") > 0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player3");
			if(p.renderer.enabled && p3Color == "") {
				Vector3 i = p.transform.position;
				i.x = 3;
				p.transform.position = i;
			}
		} else if(Input.GetAxis("HorizontalL3") < -0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player3");
			if(p.renderer.enabled && p3Color == "") {
				Vector3 i = p.transform.position;
				i.x = -3;
				p.transform.position = i;
			}
		}
		// p4
		if(Input.GetAxis("HorizontalL4") > 0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player4");
			if(p.renderer.enabled && p4Color == "") {
				Vector3 i = p.transform.position;
				i.x = 3;
				p.transform.position = i;
			}
		} else if(Input.GetAxis("HorizontalL4") < -0.5) {
			GameObject p = GameObject.FindGameObjectWithTag("player4");
			if(p.renderer.enabled && p4Color == "") {
				Vector3 i = p.transform.position;
				i.x = -3;
				p.transform.position = i;
			}
		}

		// Blink Press Start when teams are ready
		if(numPlayers >= 2 && (ready == numPlayers) ) {
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
		if(Input.GetButtonDown("start")) {
			print ("poop");
		}

	}
}
