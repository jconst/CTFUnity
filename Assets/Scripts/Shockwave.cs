using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Shockwave : DropItem
{
    const float burstDuration = 0.5f;
    const int numEdgePoints = 200;
    const float maxScale = 5f;

    LineRenderer line;
    List<Vector3> points;
    Vector3 startScale;
    Color startColor;
    float startTime;

    void Start() {
        line = GetComponent <LineRenderer>();
        line.SetVertexCount(numEdgePoints);
        startTime = Time.time;
        startColor = line.material.color;
        DrawEdge();
        Destroy(gameObject, burstDuration);
    }

    void DrawEdge() {
        startScale = transform.localScale;
        Enumerable.Range(0, numEdgePoints)
                  .ToList()
                  .ForEach(i => {
            float rad = ((float)i)/((float)numEdgePoints) * 2f*Mathf.PI;
            Vector3 linePart = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
            line.SetPosition(i, linePart);
        });
    }

    void Update() {
        float prog = (Time.time - startTime)/burstDuration;
        float scaleFactor = maxScale * (0.2f+prog);
        transform.localScale = Vector3.Scale(startScale.normalized, new Vector3(scaleFactor, 1, scaleFactor));
        line.material.color = Color.Lerp(startColor, Color.clear, prog);
    }

    void OnTriggerEnter2D(Collider2D coll) {
        HitEdge(coll);
    }

    void OnTriggerExit2D(Collider2D coll) {
        HitEdge(coll);
    }

    void HitEdge(Collider2D coll) {
        Player p = coll.GetComponent<Player>();
        if (p && p.team != owner.team && !p.tackling) {
            Vector2 toPlayer = p.transform.position - transform.position;
            toPlayer = toPlayer.normalized * (maxScale - toPlayer.magnitude);
            toPlayer *= 8f;
            p.rigidbody2D.AddForce(toPlayer, ForceMode2D.Impulse);
        }
    }
}
