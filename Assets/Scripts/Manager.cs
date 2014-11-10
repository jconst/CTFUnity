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
        {"Blue", 3},
        {"Red", 3}
    };

    public Dictionary<string, Vector2> spawnLocations =
       new Dictionary<string, Vector2> {
       {"Blue", new Vector2(-5f, -3f)},
       {"Red", new Vector2(3.5f, 5f)}
    };

    public Dictionary<string, int> teamScores;
    public Dictionary<string, GUIText> teamScoreText;

    public List<Player> allPlayers;

    static public Manager S {
        get {
            return GameObject.FindObjectOfType(typeof(Manager)) as Manager;
        }
    } 

    // -- SETUP --

    void Start() {
        teamScores = InitScores(teams);
        allPlayers = SpawnPlayers();
        CreateBackgroundCamera();
        AttachCamerasToPlayers(allPlayers);
    }

    Dictionary<string, int> InitScores(List<string> teamList) {
        return teamList.ToDictionary(t => t, t => 0);
    }

    Dictionary<string, GUIText> InitScoreText(List<string> teamList) {
        return teamList.ToDictionary(t => t, t => new GUIText());
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
        Rect rect = new Rect((float)Math.Floor(scale*num)/colSize, scale*(n%colSize), scale, scale);

        float padding = 0.01f;
        rect.xMax -= padding;
        rect.yMax -= padding;
        return rect;
    }

    void CreateBackgroundCamera() {
        GameObject cameraGO = Instantiate(Resources.Load("PlayerCamera"),
                                          new Vector3(0f,0f,-10f), 
                                          Quaternion.Euler(180, 0, 0)) as GameObject;
        Camera camera = cameraGO.GetComponent<Camera>();
        camera.rect = new Rect(0, 0, 1, 1);
        camera.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    }

    // -- GAME EVENTS --
    public void DidScore(Player scorer, Flag flag) {
        teamScores[scorer.team]++;
        flag.Reset();
        (GameObject.FindObjectsOfType(typeof(Player)) as Player[])
                   .ToList()
                   .ForEach(p => p.KillPlayer());
    }
}
