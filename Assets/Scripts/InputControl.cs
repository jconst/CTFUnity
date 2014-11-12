using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InputControl
{
    static public InputControl _s;
    static public InputControl S {
        get {
            if (_s == null) {
                _s = new InputControl();
            }
            return _s;
        }
    }

    const string keyboardPlayer = "2";

    string platformString {
        get {
            return (Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer ||
                    Application.platform == RuntimePlatform.OSXWebPlayer ||
                    Application.platform == RuntimePlatform.OSXDashboardPlayer)
                    ? "OSX" : "WIN";
        }
    }

    public Vector2 RunVelocity(string controllerNum) {
        Vector2 velocity = new Vector2(Input.GetAxis("HorizontalL" + controllerNum),
                                       Input.GetAxis("VerticalL" + controllerNum));

        //allow a player to be controlled by keyboard for testing:
        if (controllerNum == keyboardPlayer && velocity.magnitude < 0.2f) {
            velocity = new Vector2(Input.GetAxisRaw("Horizontal"),
                                   Input.GetAxisRaw("Vertical"));
        }
        return velocity;
    }

    public Vector2 TackleVelocity(string controllerNum) {
        Vector2 tackleForce = new Vector2(Input.GetAxis ("HorizontalR" + controllerNum + platformString),
                                          Input.GetAxis ("VerticalR" + controllerNum + platformString));
        //keyboard control for testing:
        if (controllerNum == keyboardPlayer && tackleForce.magnitude < 0.2f) {
            tackleForce = new Vector2(Input.GetAxisRaw("HorizontalTackle"),
                                      Input.GetAxisRaw("VerticalTackle"));
        }
        return tackleForce;
    }

    public bool ItemButtonDown(string controllerNum, int itemNum) {
        return Input.GetButtonDown("Item"+itemNum+controllerNum) ||
               (controllerNum == keyboardPlayer && Input.GetKeyDown(itemNum.ToString()));
    }
}
