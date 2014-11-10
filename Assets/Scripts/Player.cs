using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	public string team;
	public int number;
	public Vector3 initialPos;

	public Flag flag;
	public bool carrying = false;
	public SpawnPad spawnpoint;

	public Vector2 tackleDirection;
	public Vector2 velocity;
	public float tackleCurSpeed = 0f;
	public bool tackling = false;
	const float runSpeed = 4.5f;
	const float tackleAveSpeed = 4.1f;
	const float tackleDuration = 0.5f;
	float tackleStartTime;

	const string keyboardPlayer = "2";

 	private List<string> dropItems =
	    new List<string> {
	   	"Fire",
	   	"SmokeBomb",
	   	"Turret",
	   	"Shockwave"
	};

	public string controllerNum {
		get {
			return (number+1).ToString();
		}
	}

	void Start () {
		initialPos = transform.position;
	}
	
	void Update () {
		CheckDrop();
		CheckTackle();
		MoveStep();
		if (Input.GetButtonDown("Item1"+controllerNum)) {
			Debug.Log("pressed");
		}
	}

	void MoveStep() {

		float tackleProg = (Time.time - tackleStartTime)/tackleDuration;
		float curve = (float)Math.Cos(tackleProg * Math.PI);
		float speed = runSpeed;
		if (tackleProg > 1f) {
			tackling = false;
			//ease up normal running speed as "standing up" after tackle
			if (tackleProg < 1.5f) {
				speed = runSpeed + (runSpeed * curve);
			}
		}
		if (tackling) {
			tackleCurSpeed = tackleAveSpeed + (tackleAveSpeed * curve);
			velocity = tackleDirection * tackleCurSpeed;
		} else {
			velocity = new Vector2(Input.GetAxis("HorizontalL" + controllerNum),
								   Input.GetAxis("VerticalL" + controllerNum));

			//allow a player to be controlled by keyboard for testing:
			if (controllerNum == keyboardPlayer && velocity.magnitude < 0.2f) {
				velocity = new Vector2(Input.GetAxisRaw("Horizontal"),
							           Input.GetAxisRaw("Vertical"));
			}
			if (carrying) {
				speed *= 0.85f;
			}
			velocity = velocity.normalized * speed;
		}
		transform.position += (Vector3)velocity * Time.deltaTime;
	}

	void CheckDrop() {
		dropItems.Each((item, indexFromZero) => {
			int index = indexFromZero+1;
			if (Input.GetButtonDown("Item"+index+controllerNum) ||
			    (controllerNum == keyboardPlayer && Input.GetKeyDown(index.ToString()))) {
				DropNewItem(item);
			}
		});
	}

	public void DropNewItem(string itemName)
	{
		GameObject go = (GameObject)Instantiate(Resources.Load(itemName));
		go.transform.position = transform.position;
		DropItem dropItem = go.GetComponent<DropItem>();
		dropItem.WasDroppedByPlayer(this);
	}

	void CheckTackle() {
		if (tackling)
			return;
		Vector2 tackleForce = new Vector2(Input.GetAxis ("HorizontalR" + controllerNum),
										  Input.GetAxis ("VerticalR" + controllerNum));
		//keyboard control for testing:
		if (controllerNum == keyboardPlayer && tackleForce.magnitude < 0.2f) {
			tackleForce = new Vector2(Input.GetAxisRaw("HorizontalTackle"),
						              Input.GetAxisRaw("VerticalTackle"));
		}
		if (tackleForce.magnitude >= 1f) {
			Tackle(tackleForce);
		}
	}

	void Tackle(Vector2 force) {
		tackleDirection = force.normalized;
		tackleStartTime = Time.time;
		tackling = true;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		Player p = coll.gameObject.GetComponent<Player>();
		if (p && p.team != team && p.tackling) {
		    if (tackling) {
		    	if (p.tackleCurSpeed > tackleCurSpeed) {
		    		KillPlayer();
		    	}
			} else {
				KillPlayer();
			}
		}
	}

	public void KillPlayer()
	{
		if (spawnpoint) {
			transform.position=spawnpoint.transform.position;
			Destroy(spawnpoint.gameObject);
			spawnpoint=null;
		}
		else transform.position = initialPos;
		if (carrying) {
			flag.Drop();
		}
		carrying = false;
	}

	public void InvalidateSpawn()
	{
		spawnpoint = null;
	}
}
