using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainCamera : MonoBehaviour
{
    const float cameraEasing = 0.01f;
    const float maxCameraSize = 15f;
    const float minCameraSize = 6f;
 
    void Update() {
        fitAllPlayersInCamera();
    }

    void fitAllPlayersInCamera() {
        List<Player> allPlayers = Manager.S.allPlayers;
        const float padding = 0.2f;

        float maxX = allPlayers.Max(p => p.transform.position.x) + padding;
        float maxY = allPlayers.Max(p => p.transform.position.y) + padding;
        float minX = allPlayers.Min(p => p.transform.position.x) - padding;
        float minY = allPlayers.Min(p => p.transform.position.y) - padding;

        float adjustedNewSize;
        if (!Manager.S.roundStarted) {
            adjustedNewSize = maxCameraSize;
        } else {
            float newSize = Mathf.Max(maxX - minX, maxY - minY) / 2f;
            adjustedNewSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
        }
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, adjustedNewSize, cameraEasing);

        Vector3 lastPos = camera.transform.position;
        Vector3 newPos = new Vector3(Mathf.Lerp(minX, maxX, 0.5f), Mathf.Lerp(minY, maxY, 0.5f), -10);
        camera.transform.position = Vector3.Lerp(lastPos, newPos, 0.03f);
    }
}
