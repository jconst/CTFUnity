using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DecoyInput : InputControl
{
    public NavMeshPath path;
    List<Vector3> waypoints;
    Vector3 currentWaypoint;
    Decoy decoy;

    public DecoyInput(Decoy d) {
        decoy = d;
        path = new NavMeshPath();
    }

    public void UpdateWaypoints() {
        waypoints = path.corners.Select<Vector3, Vector3>(convertXZToXY).ToList();
        UpdateWaypoint();
    }

    void UpdateWaypoint() {
        // Vector2 closestWaypoint = waypoints.Aggregate(Vector2.zero, (acc, next) => {
        //     float accDist = (decoy.transform.position - acc).magnitude;
        //     float nextDist = (decoy.transform.position - next).magnitude;
        //     return (accDist > nextDist) ? acc : next;
        // });
        currentWaypoint = waypoints.Count > 0 ? waypoints[0] : decoy.transform.position;
    }

    public override Vector2 RunVelocity(int playerNum)
    {
        Vector2 toWaypoint = currentWaypoint - decoy.transform.position;
        if (toWaypoint.magnitude < 0.2f && waypoints.Count > 0) {
            waypoints.RemoveAt(0);
            UpdateWaypoint();
            toWaypoint = currentWaypoint - decoy.transform.position;
        }
        return toWaypoint.normalized;
    }

    public override Vector2 TackleVelocity(int playerNum)
    {
        return Vector2.zero;
    }

    public override bool ItemButtonDown(int playerNum, int itemNum)
    {
        return false;
    }

    Vector3 convertXZToXY(Vector3 v) {
        return new Vector3(v.x, v.z, 0);
    }
}
