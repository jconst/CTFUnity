using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManaBar : MonoBehaviour {
	
	GameObject[] blocks;
	public int currMana=0;
	public string team;
	int maxMana=3;
	

	// Use this for initialization
	void Start () {
		blocks = new GameObject[maxMana];

		for(int i=0; i<Mathf.Min (currMana, maxMana); i++)
		    {
			blocks[i]= Instantiate(Resources.Load ("Mana"+team)) as GameObject;
			blocks[i].transform.localScale=new Vector3(-0.12f,-0.05f,1f);
			blocks[i].transform.parent=this.gameObject.transform;
			blocks[i].transform.localPosition=new Vector3(0, 0.03f+0.05f*i,1f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		for (int i=0; i<currMana; i++) {
						if (blocks [i] == null) {
								blocks [i] = Instantiate (Resources.Load ("Mana" + team)) as GameObject;
								blocks [i].transform.localScale = new Vector3 (-0.12f, -0.05f, 1f);
								blocks [i].transform.parent = this.gameObject.transform;
								blocks [i].transform.localPosition = new Vector3 (0, 0.03f + 0.05f * i, 1f);
						}
				}
		for (int i=currMana; i<maxMana; i++) {
						if (blocks [i])
								Destroy (blocks [i]);
				}
	}
}
