using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour {

	public int number;
	public ItemSpawn isp;
	public Texture tex;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Pickup(Player p)
	{
		isp.spawned = false;
		p.itemIcon = Instantiate (Resources.Load("InvIcon")) as GameObject;
		p.itemIcon.GetComponent<GUITexture> ().texture = tex;
		p.itemNo = number;
		Destroy (this.gameObject);

	}
}
