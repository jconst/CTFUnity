using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Shockwave : DropItem
{
    const int numEdgePoints = 200;
    const float maxScale = 8f;
    const float initialLifeTime = 0.6f;

    LineRenderer line;
    Vector3 startScale;
    Color startColor;

    void Start() {
        lifeTime = initialLifeTime;
        line = GetComponent <LineRenderer>();
        line.SetVertexCount(numEdgePoints);
        startColor = line.material.color;
        DrawEdge();
        Destroy(gameObject, lifeTime);
    }

    void DrawEdge() {
        startScale = transform.localScale;
        Enumerable.Range(0, numEdgePoints)
                  .ToList()
                  .ForEach(i => {
            float rad = ((float)i)/((float)numEdgePoints-1) * 2f*Mathf.PI;
            Vector3 linePart = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
            line.SetPosition(i, linePart);
        });
    }

    new void Update() {
        base.Update();
        float prog = 1-(lifeTime/initialLifeTime);
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
        Rigidbody2D body = coll.GetComponent<Rigidbody2D>();
        if (body) {
            Player p = coll.GetComponent<Player>();
            if (p) {
                if (p.team == owner.team) {
                    return;
                }
                p.hasKnockback = true;
            }
            body.AddForce(ForceForColl(coll), ForceMode2D.Impulse);
        }
    }

    Vector2 ForceForColl(Collider2D coll) {
        GameObject go = coll.gameObject;
        Vector2 toTarget = go.transform.position - transform.position;
        toTarget = toTarget.normalized * (maxScale - toTarget.magnitude);
        return toTarget * 10f;
    }
}
