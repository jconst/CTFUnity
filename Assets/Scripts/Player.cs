using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	public string team;
	public int number;
	public Vector3 initialPos;
	public Vector2 lastInputVelocity;

	public Flag flag;
	public SpawnPad spawnpoint;

	const float runSpeed = 5f;
	const float tackleAveSpeed = 5.5f;
	const float tackleDuration = 0.55f;
	const float tackleCooldown = 0.8f;

	public Vector2 tackleDirection;
	float tackleStartTime;
	public bool tackling = false;
	private bool tackleInterrupted = false;

	const string keyboardPlayer = "3";

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

	public bool carrying {
		get {
			return flag != null;
		}
	}

	void Start () {
		initialPos = transform.position;
	}
	
	void Update () {
		CheckDrop();
		CheckTackle();
		MoveStep();
	}

	void MoveStep() {

		float tackleProg = (Time.time - tackleStartTime)/tackleDuration;
		float curve = (float)Math.Cos(tackleProg * Math.PI);
		float speed = runSpeed;

		if (tackleInterrupted && rigidbody2D.velocity.magnitude < 0.2f) {
			//unset tackleInterrupted after knockback is done
			tackleInterrupted = false;
		}
		if (tackleProg > 1f) {
			tackling = false;
			//ease up normal running speed as "standing up" after tackle
			if (tackleProg < 1.5f) {
				speed = runSpeed + (runSpeed * curve);
			}
		} 
		if (tackling && !tackleInterrupted) {
			float tackleCurSpeed = tackleAveSpeed + (tackleAveSpeed * curve);
			rigidbody2D.velocity = tackleDirection * tackleCurSpeed;
		} 
		//Moving normally:
		if (!tackling && !tackleInterrupted) {
			Vector2 velocity = new Vector2(Input.GetAxis("HorizontalL" + controllerNum),
						        		   Input.GetAxis("VerticalL" + controllerNum));

			//allow a player to be controlled by keyboard for testing:
			if (controllerNum == keyboardPlayer && velocity.magnitude < 0.2f) {
				velocity = new Vector2(Input.GetAxisRaw("Horizontal"),
							           Input.GetAxisRaw("Vertical"));
			}
			if (carrying) {
				speed *= 0.9f;
			}
			if (velocity.magnitude > 0.2f ||
				lastInputVelocity.magnitude > 0.1f) {
				rigidbody2D.velocity = velocity.normalized * speed;
			}
			lastInputVelocity = velocity;
		}
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
		Vector3 newPos = transform.position;
		newPos.z = go.transform.position.z;
		go.transform.position = newPos;

		DropItem dropItem = go.GetComponent<DropItem>();
		dropItem.WasDroppedByPlayer(this);
	}

	void CheckTackle() {
		float timeSinceTackle = Time.time - tackleStartTime;
		if (tackling || timeSinceTackle < tackleCooldown)
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
		tackleInterrupted = false;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		Player p = coll.gameObject.GetComponent<Player>();
		if (p && p.team != team && p.tackling) {
			if (!tackling) {
				KillPlayer();
			}
		    if (tackling && carrying) {
				flag.Drop(this);
			}
		}
		if (tackling)
			tackleInterrupted = true;
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
			flag.Drop(this);
		}
		flag = null;
		rigidbody2D.velocity = Vector2.zero;
	}

	public void InvalidateSpawn()
	{
		spawnpoint = null;
	}
}
