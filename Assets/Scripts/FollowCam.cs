using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{
    static public FollowCam S;

    [HideInInspector] public GameObject poi;
    private float camZ;
    private Vector2 minXY = new Vector2(-100, -100);
    // private float easing = 1f;

    void Awake()
    {
        S = this;
        camZ = this.transform.position.z;
    }

    void Update ()
    {
        if (poi == null) {
            return;
        }
        Vector3 dest = poi.transform.position;

        dest.x = Mathf.Max(minXY.x, dest.x);
        dest.y = Mathf.Max(minXY.y, dest.y);
        // dest = Vector3.Lerp(transform.position, dest, easing);
        dest.z = camZ;

        transform.position = dest;
    }
}