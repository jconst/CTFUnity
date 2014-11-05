using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public string controllerNum;
	public string team;
	public int number;
	public Vector3 initialPos;

	public List<string> dropItems =
	   new List<string> {
	   	"Fire",
	   	"SpawnPad",
	   	"Turret"
	};

	public Flag flag;
	public bool carrying=false;
	float speed=4f;
	GameObject spawnpoint;

	// Use this for initialization
	void Start () {
		initialPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 velocity = Vector3.zero;
		Vector3 pos = transform.position;
		velocity.x = Input.GetAxis ("Horizontal" + controllerNum)*speed;
		velocity.y = Input.GetAxis ("Vertical" + controllerNum)*speed;

		//allow p1 to be controlled by keyboard for testing:
		if (controllerNum == "1" && velocity.magnitude == 0) {
			velocity.x = Input.GetAxisRaw("Horizontal")*speed;
			velocity.y = Input.GetAxisRaw("Vertical")*speed;
		}

		pos += velocity * Time.deltaTime;
		transform.position = pos;
		string droppedItemName = null;
		GameObject go = null;

		dropItems.Each((item, indexFromZero) => {
			int index = indexFromZero+1;
			if (Input.GetButtonDown("Item"+index+controllerNum) ||
			    (controllerNum == "1" && Input.GetKeyDown(index.ToString()))) {
				go = Instantiate(Resources.Load(item)) as GameObject;
				go.transform.position = transform.position;
				droppedItemName = item;
			}
		});

		if(go && droppedItemName == "SpawnPad") {
			if (spawnpoint) 
				Destroy(spawnpoint);
			SpawnPad sp = go.GetComponent<SpawnPad>();
			sp.owner = this;
			spawnpoint = go;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		//needs tackle mechanics
	}

	public void KillPlayer()
	{
		if (spawnpoint) {
			transform.position=spawnpoint.transform.position;
			Destroy(spawnpoint);
			spawnpoint=null;
		}
		else transform.position = initialPos;
		if (carrying)
			flag.Reset();
		carrying = false;
		speed = 4f;
	}

	public void InvalidateSpawn()
	{
		spawnpoint = null;
	}
}
