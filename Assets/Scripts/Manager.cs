using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Manager : MonoBehaviour
{
    public List<string> teams =
       new List<string> {
        "Blue",
        "Red"
    };

    public Dictionary<string, Color> teamColors =
       new Dictionary<string, Color> {
        {"Blue", Color.blue},
        {"Red", Color.red}
    };

    public Dictionary<string, int> teamSizes =
       new Dictionary<string, int> {
        {"Blue", 2},
        {"Red", 2}
    };

    public Dictionary<string, Vector2> spawnLocations =
       new Dictionary<string, Vector2> {
       {"Blue", new Vector2(-5f, -3f)},
       {"Red", new Vector2(3.5f, 5f)}
    }; 

    public List<Player> allPlayers;

    void Start() {
        allPlayers = SpawnPlayers();
        AttachCamerasToPlayers(allPlayers);
    }

    List<Player> SpawnPlayers() {
        return teamSizes.SelectMany(kvp => SpawnTeam(kvp.Key, kvp.Value)).ToList();
    }

    List<Player> SpawnTeam(string team, int size) {
        int teamNum = teams.FindIndex(t => t == team);
        return Enumerable.Range(0, size).Select(i => SpawnPlayer(team, (size*teamNum)+i)).ToList();
    }

    Player SpawnPlayer(string team, int index) {
        Vector2 posOffset = new Vector2(index, -index);
        GameObject playerGO = Instantiate(Resources.Load("Player"),
                                          spawnLocations[team]+posOffset, 
                                          Quaternion.identity) as GameObject;
        Player player = playerGO.GetComponent<Player>();
        player.team = team;
        player.number = index;
        player.renderer.material.color = teamColors[team];

        return player;
    }
    
    List<Camera> AttachCamerasToPlayers(List<Player> players) {
        return players.Select(p => AttachNewCameraToPlayer(p)).ToList();
    }

    Camera AttachNewCameraToPlayer(Player p) {
        Vector3 spawnLoc = p.transform.position;
        spawnLoc.z = -10f;
        GameObject cameraGO = Instantiate(Resources.Load("PlayerCamera"),
                                          spawnLoc, 
                                          Quaternion.identity) as GameObject;
        FollowCam fc = cameraGO.GetComponent<FollowCam>();
        fc.poi = p.gameObject;
        Camera camera = cameraGO.GetComponent<Camera>();
        camera.rect = CameraRectForPlayerNum(p.number);
        return camera;
    }

    Rect CameraRectForPlayerNum(int num) {
        float n = (float)num;
        float max = (float)allPlayers.Count;
        float colSize = (float)Math.Ceiling(Math.Sqrt(max));
        float scale = 1f/colSize;
        return new Rect((float)Math.Floor(scale*num)/colSize, scale*(n%colSize), scale, scale);
    }
}
