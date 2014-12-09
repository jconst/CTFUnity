using UnityEngine;
using System.Collections;

public class FPSWalker : MonoBehaviour {

    float speed = 4.0f;
    float jumpSpeed = 5.0f;
    float gravity = 20.0f;

    public Vector3 moveDirection = Vector3.zero;
    public bool grounded = false;

    public bool CANJUMP = true;

    CharacterController controller;
	
	private int PlayerLayer;
	
	private bool PreviewSceneCheck() {
		PlayerLayer = LayerMask.NameToLayer("Player");
		if(PlayerLayer == -1) {
			Debug.LogWarning("PreviewScene Layer 'Player': is not defined. Create the 'Player' Layer at Layers Setup in the Inspector!. See the Manual at Chapter 1 and follow the Instructions!");
			return false;
		}
		return true;
	}

    void Start()
    {
		if(!PreviewSceneCheck()) {
			Debug.LogWarning("Preview Scene Startup Check Failed. Check the Console for some Warnings.");
		} else {
			gameObject.layer = LayerMask.NameToLayer("Player");
		}
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
		if(GlowCamera.GlowMenuActive())
			return;
			
    	if (grounded) { CANJUMP = true; }
        else { CANJUMP = false; }

        if (grounded)
        {
        	moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        	moveDirection = transform.TransformDirection(moveDirection);
        	moveDirection *= speed;

      		if (Input.GetButton("Jump"))
 			{
        		moveDirection.y = jumpSpeed;
         	}
     	} else {
       		moveDirection.y -= gravity * Time.deltaTime;
		}

  		var flags = controller.Move(moveDirection * Time.deltaTime);
   		grounded = (flags & CollisionFlags.CollidedBelow) != 0;
    }
	
}
