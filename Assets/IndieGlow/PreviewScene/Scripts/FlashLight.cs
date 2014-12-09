using UnityEngine;
using System.Collections;

public class FlashLight : MonoBehaviour {
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.F)) {
			light.enabled = !light.enabled;
		}
	}
	
	void OnGUI() {
		if(!GlowCamera.UseMenu) {
			GUI.Label(new Rect(10, 50, 250, 30), "Press F to Toggle FlashLight");
		}	
	}
	
}
