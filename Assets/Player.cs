using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public string controllerNum;
	public string color;
	public string otherColor;
	public GameObject flag;
	public Vector3 initialPos;

	
	public GameObject item1;
	public GameObject item2;
	public GameObject item3;
	public GameObject item4;

	float speed=4f;
	bool safe=true;

	// Use this for initialization
	void Start () {
		flag = GameObject.FindGameObjectWithTag (color + "Flag");
		initialPos = transform.position;

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
		pos += velocity * Time.deltaTime;
		transform.position = pos;

		if(Input.GetButtonDown("Item1"+controllerNum))
		   {
			GameObject go= Instantiate(item1) as GameObject;
			go.transform.position=transform.position;
		}
		if(Input.GetButtonDown("Item2"+controllerNum))
		{
			GameObject go= Instantiate(item2) as GameObject;
			go.transform.position=transform.position;
		}
		if(Input.GetButtonDown("Item3"+controllerNum))
		{
			GameObject go= Instantiate(item3) as GameObject;
			go.transform.position=transform.position;
		}
		if(Input.GetButtonDown("Item4"+controllerNum))
		{
			GameObject go= Instantiate(item4) as GameObject;
			go.transform.position=transform.position;
		}
	
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag (color + "Side")) 
		
			safe=true;
		
		if (col.CompareTag (otherColor + "Side"))
						safe = false;
		if (col.CompareTag ("Player")) 
		{
			if(!safe)
				KillPlayer();
		}
	}

	public void KillPlayer()
	{
		GameObject player = Instantiate (this.gameObject) as GameObject;
		player.transform.position = initialPos;
		Player p = player.GetComponent<Player> ();
		p.initialPos = initialPos;
		p.speed = 4f;
		Destroy (this.gameObject);
	}
}
