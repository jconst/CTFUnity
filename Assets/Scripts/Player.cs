using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public string controllerNum;
	public string color;
	public string otherColor;
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

	// Use this for initialization
	void Start () {
		initialPos = transform.position;
		score = GameObject.Find (color + "Score").GetComponent<GUIText> ();
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
		Debug.Log ("dongs");
		if (col.CompareTag (color + "Side")) {
			safe = true;
			Debug.Log ("dongsyay");
		}
		
		if (col.CompareTag (otherColor + "Side")) {
			safe = false;
			Debug.Log ("dongspoop");
		}
		if (col.CompareTag ("Player")) 
		{
			if(!safe&&col.gameObject.GetComponent<Player>().color.Equals(otherColor))
				KillPlayer();
		}
		if (col.CompareTag (otherColor + "Flag")) {
			carrying=true;
			col.gameObject.GetComponent<Flag>().Pickup(this.gameObject);
			flag=col.gameObject.GetComponent<Flag>();
		}
		if (col.CompareTag (color + "Flag") && carrying) {
			carrying=false;
			int sc = int.Parse(score.text);
			sc+=1;
			score.text=sc.ToString();
			flag.Reset();
			flag=null;
		}
	}

	public void KillPlayer()
	{
		transform.position = initialPos;
		if (carrying)
			flag.Reset();
		carrying = false;
		speed = 4f;
	}
}
