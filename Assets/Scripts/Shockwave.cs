﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Shockwave : DropItem
{
    const float burstDuration = 1f;
    int numEdgePoints = 200;

    LineRenderer line;
    List<Vector3> points;
    Vector3 startScale;
    float startTime;
    float maxScale = 3f;

    void Start() {
        line = GetComponent <LineRenderer>();
        line.SetVertexCount(numEdgePoints);
        startTime = Time.time;
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
            p.KillPlayer();
        }
    }
}
