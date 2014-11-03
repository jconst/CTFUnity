using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public string controllerNum;
	public string team;
	public int number;
	public Vector3 initialPos;
	GUIText score;

	public List<string> dropItems =
	   new List<string> {
	   	"Fire",
	   	"SpawnPad",
	   	"Turret"
	};

	float speed=4f;
	bool carrying=false;
	Flag flag;
	bool safe=true;
	GameObject spawnpoint;

	// Use this for initialization
	void Start () {
		initialPos = transform.position;
		score = GameObject.Find (team + "Score").GetComponent<GUIText> ();
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

		if(go && droppedItemName.Equals("SpawnPad")) {
			if(spawnpoint) Destroy(spawnpoint);
			SpawnPad sp=go.GetComponent<SpawnPad>();
			sp.Owner=this;
			spawnpoint=go;
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag (team + "Side")) {
			safe = true;
		} else if (col.tag.EndsWith("Side")) {
			safe = false;
		}
		if (col.CompareTag ("Player")) 
		{
			if(!safe && col.gameObject.GetComponent<Player>().team != team)
				KillPlayer();
		}
		if (col.tag.EndsWith("Flag") && !col.tag.StartsWith(team)) {
			carrying=true;
			col.gameObject.GetComponent<Flag>().Pickup(this.gameObject);
			flag=col.gameObject.GetComponent<Flag>();
		}
		if (col.CompareTag (team + "Flag") && carrying) {
			carrying=false;
			int sc = int.Parse(score.text);
			sc+=1;
			score.text=sc.ToString();
			flag.Reset();
			flag=null;
		}
		if (col.CompareTag ("SpawnPad") && 
						!col.gameObject.GetComponent<SpawnPad> ().Team.Equals (team)) {
			col.gameObject.GetComponent<SpawnPad>().Owner.InvalidateSpawn();
			Destroy(col.gameObject);
				}
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
