using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{
    static public FollowCam S;

    public float easing = 1f;
    public Vector2 minXY;
    public GameObject poi;
    public bool _____________________________;
    public float camZ;

    void Awake()
    {
        S = this;
        camZ = this.transform.position.z;
    }

    void Update ()
    {
        Vector3 dest = poi.transform.position;

        dest.x = Mathf.Max(minXY.x, dest.x);
        dest.y = Mathf.Max(minXY.y, dest.y);
        // dest = Vector3.Lerp(transform.position, dest, easing);
        dest.z = camZ;
        
        transform.position = dest;
    }
}