using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public string controllerNum;
	public string team;
	public int number;
	public Vector3 initialPos;
	GUIText score;

	
	public GameObject item1;
	public GameObject item2;
	public GameObject item3;
	public GameObject item4;

	float speed=4f;
	bool carrying=false;
	Flag flag;
	bool safe=true;
	GameObject spawnpoint;

	// Use this for initialization
	void Start () {
		initialPos = transform.position;
		score = GameObject.Find (team + "Score").GetComponent<GUIText> ();
		if (!item1) {
			item1 = new GameObject ();
		}
		if (!item2) {
			item2 = new GameObject ();
		}
		if (!item3) {
			item3 = new GameObject ();
		}
		if (!item4) {
			item4 = new GameObject ();
		}
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
		string droppedItemName=null;
		GameObject go=null;

		if(Input.GetButtonDown("Item1"+controllerNum))
		{
			go= Instantiate(item1) as GameObject;
			go.transform.position=transform.position;
			droppedItemName=item1.name;
		}
		if(Input.GetButtonDown("Item2"+controllerNum))
		{
			go= Instantiate(item2) as GameObject;
			go.transform.position=transform.position;
			droppedItemName=item2.name;
		}
		if(Input.GetButtonDown("Item3"+controllerNum))
		{
			go= Instantiate(item3) as GameObject;
			go.transform.position=transform.position;
			droppedItemName=item3.name;
		}
		if(Input.GetButtonDown("Item4"+controllerNum))
		{
			go= Instantiate(item4) as GameObject;
			go.transform.position=transform.position;
			droppedItemName=item4.name;
		}

		if(go && droppedItemName.Equals("SpawnPoint"))
		   {
			spawnpoint=go;
			SpawnPad sp=spawnpoint.GetComponent<SpawnPad>();
			sp.Owner=this;
			sp.Team=team;
		}

		Vector3 relativePos = Camera.main.WorldToViewportPoint (transform.position);

		if (relativePos.x > 1f)
			relativePos.x = 1f;
		if (relativePos.y > 1f)
			relativePos.y = 1f;
		if (relativePos.x < 0f)
			relativePos.x = 0f;
		if (relativePos.y < 0f)
			relativePos.y = 0f;
		transform.position = Camera.main.ViewportToWorldPoint (relativePos);
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
