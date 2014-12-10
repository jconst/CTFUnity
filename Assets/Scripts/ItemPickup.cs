using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour {

	public int number;
	public ItemSpawn isp;
	public Texture tex;

	void OnTriggerEnter2D(Collider2D coll)
	{
		Player p = coll.GetComponent<Player>();
		if (p) {
			Pickup(p);
		}
	}

	public void Pickup(Player p)
	{
		isp.spawned = false;
		if (number == 5) {
			Debug.Log("here");
			Manager.S.teamManas [p.team] = Mathf.Min (3, Manager.S.teamManas [p.team] + 1);
			AudioManager.Main.PlayNewSound("Mana");
			Destroy (this.gameObject);
		} else if (p.itemNo == -1) {
			p.itemIcon = Instantiate (Resources.Load ("InvIcon")) as GameObject;
			p.itemIcon.GetComponent<GUITexture> ().texture = tex;
			p.itemNo = number;

			p.itemIcon.transform.parent = Manager.S.teamManaBars[p.team].transform;
			p.itemIcon.transform.localScale = new Vector3 (1f, 0.35f, 1f);
			p.itemIcon.transform.localPosition = new Vector3 (0f, 0f, 1);
			Destroy (this.gameObject);
		}
	}
}
