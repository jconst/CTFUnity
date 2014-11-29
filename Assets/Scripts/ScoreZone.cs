using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoreZone : MonoBehaviour
{
    public string team;
    GameObject progressBar;
    Bomb bomb;
/*
    void OnTriggerEnter2D(Collider2D coll) {

print("Instantiated object");
print(coll);

		bomb = coll.gameObject.GetComponent<Bomb>();

		if (bomb) {

print("Inside bomb collision");		

			progressBar = (GameObject)Instantiate(Resources.Load("ProgressBar"));
			ProgressBar pb = progressBar.GetComponent<ProgressBar>();
	        pb.team = team;

	        Vector3 lp = pb.transform.localPosition;
	        lp.y = bomb.transform.position.y - 1;
	        pb.transform.localPosition = lp;
		} 
    }

    void OnTriggerExit2D(Collider2D coll) {
    	if (progressBar) {
    		Destroy(progressBar);
    	}
    }
    */
}
