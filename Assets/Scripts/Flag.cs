using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour
{
	Player carrier;
	Vector3 initialPosit;
	GUIText redscore;
	GUIText bluescore;


	bool following=false;
	float redCount=0;
	float blueCount=0;
	bool redTimer=false;
	bool blueTimer=false;
	float timeLimit=5f;

	// Use this for initialization
	void Start () {
		initialPosit = transform.position;
		redscore = GameObject.Find ("RedScore").GetComponent<GUIText> ();
		bluescore = GameObject.Find ("BlueScore").GetComponent<GUIText> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (following) {
			transform.position = carrier.transform.position;
		}
		if (redTimer) 
		{
			redCount+=Time.deltaTime;
		}
		if (blueTimer) 
		{
			blueCount+=Time.deltaTime;
		}
		if (blueCount >= timeLimit) 
		{
			Score (redscore);
			blueCount=0;
			blueTimer=false;
			Reset();
		}
		if (redCount >= timeLimit) 
		{
			Score (bluescore);
			redTimer=false;
			redCount=0;
			Reset();
		}
	}

	public void Pickup(Player p)
	{
		carrier = p;
		following = true;
		p.flag = this;
		p.carrying=true;
	}

	void Score(GUIText side)
	{
		int sc = int.Parse (side.text);
		sc += 1;
		side.text = sc.ToString ();
		}

	public void Reset()
	{
		following = false;
		transform.position = initialPosit;
		carrier = null;
	}

	public void OnTriggerEnter2D(Collider2D coll)
	{
		Player player = coll.GetComponent<Player>();
		if (player) {
						Pickup (player);
				} 
		else if (coll.tag.Contains ("Side")) {
			if(coll.tag.Contains("Blue"))
			   {
				blueTimer=true;
			}
			if(coll.tag.Contains("Red"))
			{
				redTimer=true;
			}
		}
	}
	public void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.tag.Contains ("Side")) {
			if(coll.tag.Contains("Blue"))
			{
				blueTimer=false;
				blueCount=0;
			}
			if(coll.tag.Contains("Red"))
			{
				redTimer=false;
				redCount=0;
			}
		}
	}

}
