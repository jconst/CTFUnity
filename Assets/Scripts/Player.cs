using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	public string controllerNum;
	public string team;
	public int number;
	public Vector3 initialPos;

	public Flag flag;
	public bool carrying = false;
	public SpawnPad spawnpoint;

	const float runSpeed = 4f;
	const float tackleSpeed = 4f;
	const float tackleDuration = 0.7f;
	bool tackling = false;
	float tackleStartTime;
	Vector2 tackleDirection;

	public List<string> dropItems =
	   new List<string> {
	   	"Fire",
	   	"SpawnPad",
	   	"Turret"
	};

	Vector2 velocity {
		get {
			float tackleProg = (Time.time - tackleStartTime)/tackleDuration;
			if (tackleProg >= 1f) {
				tackling = false;
			} 
			if (tackling) {
				float curve = tackleSpeed * (float)Math.Cos(tackleProg * Math.PI);
				return tackleDirection * (tackleSpeed + curve);
			}
			Vector3 velocity = Vector3.zero;
			velocity = new Vector2(Input.GetAxis ("HorizontalL" + controllerNum)*runSpeed,
								   Input.GetAxis ("VerticalL" + controllerNum)*runSpeed);

			//allow p1 to be controlled by keyboard for testing:
			if (controllerNum == "1" && velocity.magnitude < 0.2f) {
				return new Vector2(Input.GetAxisRaw("Horizontal")*runSpeed,
							       Input.GetAxisRaw("Vertical")*runSpeed);
			}
			return velocity;
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

	void OnCollisionEnter2D(Collision2D coll)
	{
		//needs tackle mechanics
	}

	void MoveStep() {
		transform.position += (Vector3)velocity * Time.deltaTime;
	}

	void CheckDrop() {
		dropItems.Each((item, indexFromZero) => {
			int index = indexFromZero+1;
			if (Input.GetButtonDown("Item"+index+controllerNum) ||
			    (controllerNum == "1" && Input.GetKeyDown(index.ToString()))) {
				DropNewItem(item);
			}
		});
	}

	public void DropNewItem(string itemName)
	{
		DropItem dropItem = ((GameObject)Instantiate(Resources.Load(itemName),
				                          			 transform.position,
					                    			 Quaternion.identity)).GetComponent<DropItem>();
		dropItem.WasDroppedByPlayer(this);
	}

	void CheckTackle() {
		if (tackling)
			return;
		Vector2 tackleForce = new Vector2(Input.GetAxis ("HorizontalR" + controllerNum),
										  Input.GetAxis ("VerticalR" + controllerNum));
		if (tackleForce.magnitude > 0.5f) {
			Tackle(tackleForce);
		}
	}

	void Tackle(Vector2 force) {
		tackleDirection = force.normalized;
		tackleStartTime = Time.time;
		tackling = true;
	}

	public void KillPlayer()
	{
		if (spawnpoint) {
			transform.position=spawnpoint.transform.position;
			Destroy(spawnpoint.gameObject);
			spawnpoint=null;
		}
		else transform.position = initialPos;
		if (carrying)
			flag.Reset();
		carrying = false;
	}

	public void InvalidateSpawn()
	{
		spawnpoint = null;
	}
}
