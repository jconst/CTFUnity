using UnityEngine;
using System.Collections;

public class SelectTeam : MonoBehaviour {

	public static int numPlayers = 0;
	public static int bluePlayers = 0;
	public static int redPlayers = 0;

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
	}
	
	// Update is called once per frame
	void Update () {
		// Everytime a player hits 'A', add a controller that 
		if( Input.GetButtonDown("Item11") ) {
			GameObject p = GameObject.FindGameObjectWithTag("player1");
			p.renderer.enabled = true;
			numPlayers++;
		}
		if( Input.GetButtonDown("Item21") ) {
			GameObject p = GameObject.FindGameObjectWithTag("player2");
			p.renderer.enabled = true;
			numPlayers++;
		}
		if( Input.GetButtonDown("Item31") ) {
			GameObject p = GameObject.FindGameObjectWithTag("player3");
			p.renderer.enabled = true;
			numPlayers++;
		}
		if( Input.GetButtonDown("Item41") ) {
			GameObject p = GameObject.FindGameObjectWithTag("player4");
			p.renderer.enabled = true;
			numPlayers++;
		}

		// they can move left and right to pick a team
		// p1
		if(Input.GetAxis("HorizontalL1") > 0.5) {
			// Move blue
			GameObject p = GameObject.FindGameObjectWithTag("player1");
			if(p.renderer.enabled) {
				Vector3 i = p.transform.position;
				i.x = 3;
				p.transform.position = i;
			}
		} else if(Input.GetAxis("HorizontalL1") < -0.5) {
			// Move red
			GameObject p = GameObject.FindGameObjectWithTag("player1");
			if(p.renderer.enabled) {
				Vector3 i = p.transform.position;
				i.x = -3;
				p.transform.position = i;
			}
		}
		// p2
		if(Input.GetAxis("HorizontalL2") > 0.5) {
			// Move blue
			GameObject p = GameObject.FindGameObjectWithTag("player2");
			if(p.renderer.enabled) {
				Vector3 i = p.transform.position;
				i.x = 3;
				p.transform.position = i;
			}
		} else if(Input.GetAxis("HorizontalL2") < -0.5) {
			// Move red
			GameObject p = GameObject.FindGameObjectWithTag("player2");
			if(p.renderer.enabled) {
				Vector3 i = p.transform.position;
				i.x = -3;
				p.transform.position = i;
			}
		}
		// p3
		if(Input.GetAxis("HorizontalL3") > 0.5) {
			// Move blue
			GameObject p = GameObject.FindGameObjectWithTag("player3");
			if(p.renderer.enabled) {
				Vector3 i = p.transform.position;
				i.x = 3;
				p.transform.position = i;
			}
		} else if(Input.GetAxis("HorizontalL3") < -0.5) {
			// Move red
			GameObject p = GameObject.FindGameObjectWithTag("player3");
			if(p.renderer.enabled) {
				Vector3 i = p.transform.position;
				i.x = -3;
				p.transform.position = i;
			}
		}
		// p4
		if(Input.GetAxis("HorizontalL4") > 0.5) {
			// Move blue
			GameObject p = GameObject.FindGameObjectWithTag("player4");
			if(p.renderer.enabled) {
				Vector3 i = p.transform.position;
				i.x = 3;
				p.transform.position = i;
			}
		} else if(Input.GetAxis("HorizontalL4") < -0.5) {
			// Move red
			GameObject p = GameObject.FindGameObjectWithTag("player4");
			if(p.renderer.enabled) {
				Vector3 i = p.transform.position;
				i.x = -3;
				p.transform.position = i;
			}
		}

	}
}
