using UnityEngine;
using System.Collections;

public class LaserHandler2 : MonoBehaviour {
	
	public GameObject ImpactParticle;
	
	private int LaserLayer;
	
	private bool PreviewSceneCheck() {
		LaserLayer = LayerMask.NameToLayer("Laser");
		if(LaserLayer == -1) {
			Debug.LogWarning("PreviewScene Layer 'Laser': is not defined. Create the 'Laser' Layer at Layers Setup in the Inspector!. See the Manual at Chapter 1 and follow the Instructions!");
			return false;
		}
		return true;
	}
	
	void Awake() {
		if(!PreviewSceneCheck()) {
			Debug.LogWarning("Preview Scene Startup Check Failed. Check the Console for some Warnings.");
		} else {
			gameObject.layer = LayerMask.NameToLayer("Laser");
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter(Collision collision) {
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
        Vector3 pos = contact.point;
        Instantiate(ImpactParticle, pos, rot);
        Destroy(gameObject);
    }
}
