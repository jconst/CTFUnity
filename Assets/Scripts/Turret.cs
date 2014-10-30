using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Turret : MonoBehaviour
{
    public string team;
    public float fireFreq;

    void Start() {
        
    }
    
    void Update() {
        Vector2 toTarget = ClosestEnemyPlayer().transform.position - transform.position;
        transform.rotation = toTarget.ToQuaternion();
    }

    Player ClosestEnemyPlayer() {
        List<Player> players = (GameObject.FindObjectsOfType(typeof(Player)) as Player[]).ToList();     
        return players.Where(p => p.team != team)
                      .Aggregate((closestSoFar, cur) => HowFar(cur) < HowFar(closestSoFar) ? cur : closestSoFar);
    }

    float HowFar(Player p) {
        return (transform.position - p.transform.position).magnitude;
    }
}
