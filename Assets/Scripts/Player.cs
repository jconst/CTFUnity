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
	const float tackleCooldown = 0.85f;

	public Vector2 tackleDirection;
	float tackleStartTime;
	public bool tackling = false;
	private bool tackleInterrupted = false;

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
			Vector2 velocity = InputControl.S.RunVelocity(controllerNum);
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
			if (InputControl.S.ItemButtonDown(controllerNum, index)) {
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
		Vector2 tackleForce = InputControl.S.TackleVelocity(controllerNum);
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
