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
	public bool frozen = false;

	public Flag flag;
	public SpawnPad spawnpoint;

	const float runSpeed = 5f;
	const float tackleAveSpeed = 5.5f;
	const float tackleDuration = 0.55f;
	const float tackleCooldown = 0.85f;
	const float respawnTime = 1f;

	public Vector2 tackleDirection;
	float tackleStartTime;
	public bool tackling = false;
	public bool hasKnockback = false;
	public float currentBoost = 1f;

 	private List<string> dropItems =
	    new List<string> {
	   	"Turret",
	   	"SmokeBomb",
	   	"Shockwave",
	   	"Boost"
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

	void Update () {
		CheckDrop();
		CheckTackle();
		MoveStep();
	}

	void MoveStep()
	{
		if (frozen)
			return;

		if (hasKnockback) {
			if (rigidbody2D.velocity.magnitude < 0.2f) {
				//unset hasKnockback after we've stopped moving
				hasKnockback = false;
			}
		}
		else {
			float tackleProg = (Time.time - tackleStartTime)/tackleDuration;
			float curve = (float)Math.Cos(tackleProg * Math.PI);
			float speed = runSpeed;
			if (tackleProg > 1f) {
				tackling = false;
				if (tackleProg < 1.5f) {
					//ease up normal running speed as "standing up" after tackle
					speed = runSpeed + (runSpeed * curve);
				}
			}
			if (tackling) {
				float tackleCurSpeed = tackleAveSpeed + (tackleAveSpeed * curve);
				rigidbody2D.velocity = tackleDirection * tackleCurSpeed;
			} 
			else {
				Vector2 velocity = InputControl.S.RunVelocity(controllerNum);
				speed *= (carrying ? 0.9f : 1f);
				if (velocity.magnitude > 0.2f ||
					lastInputVelocity.magnitude > 0.1f) {
					rigidbody2D.velocity = velocity.normalized * speed;
				}
				lastInputVelocity = velocity;
			}
			rigidbody2D.velocity *= currentBoost;
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
		if (!dropItem.TryDrop(this) ||
			!Manager.S.SubManaCost(this, dropItem.manaCost)) {
			Destroy(go);
			return;
		}
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
		hasKnockback = false;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		Player p = coll.gameObject.GetComponent<Player>();
		if (p && p.team != team && p.tackling) {
			if (!tackling) {
				Die();
			}
		    if (tackling && carrying) {
				flag.Drop();
			}
		}
		if (tackling)
			hasKnockback = true;
	}
	
	public void Die()
	{
		StartCoroutine(DieCoroutine());
	}

	public IEnumerator DieCoroutine()
	{
		renderer.enabled = false;
		collider2D.enabled = false;
		particleSystem.startColor = Manager.S.teamColors[team];
		particleSystem.Play();
		yield return new WaitForSeconds(respawnTime);
		Reset();
	}

	public void Reset()
	{
		renderer.enabled = true;
		collider2D.enabled = true;
		if (spawnpoint) {
			transform.position = spawnpoint.transform.position;
			Destroy(spawnpoint.gameObject);
			spawnpoint=null;
		}
		else 
			transform.position = initialPos;
		if (carrying) {
			flag.Drop();
		}
		flag = null;
		rigidbody2D.velocity = Vector2.zero;
	}

	public void InvalidateSpawn()
	{
		spawnpoint = null;
	}
}
