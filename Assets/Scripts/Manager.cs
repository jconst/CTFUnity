using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Manager : MonoBehaviour
{
    // -- CONSTANTS --
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

    // -- VARIABLES --
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
        teamScoreText = InitScoreText(teams);
        allPlayers = SpawnPlayers();
        CreateBackgroundCamera();
        AttachCamerasToPlayers(allPlayers);
        CreateOverlayCamera();
    }

    Dictionary<string, int> InitScores(List<string> teamList) {
        return teamList.ToDictionary(t => t, t => 0);
    }

    Dictionary<string, GUIText> InitScoreText(List<string> teamList) {
        GameObject scoreBoard = GameObject.FindWithTag("ScoreBoard");
        Debug.Log(scoreBoard.transform);
        return teamList.ToDictionary(t => t, t => {
            GameObject go = Instantiate(Resources.Load("ScoreText")) as GameObject;
            GUIText gt = go.GetComponent<GUIText>();
            float width = 1f / teamList.Count();
            float index = (float)teamList.IndexOf(t);
            go.transform.parent = scoreBoard.transform;
            go.transform.localScale = new Vector2(width, 1);
            go.transform.localPosition = new Vector3(index*width - 0.25f, 0.1f, 1);
            gt.color = teamColors[t];
            gt.fontSize = (int)(Screen.height * 0.04);
            return gt;
        });
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
        GameObject cameraGO = Instantiate(Resources.Load("UICamera"),
                                          new Vector3(0f,0f,-10f), 
                                          Quaternion.Euler(180, 0, 0)) as GameObject;
        Camera camera = cameraGO.GetComponent<Camera>();
        camera.rect = new Rect(0, 0, 1, 1);
        camera.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    }

    void CreateOverlayCamera() {
        GameObject cameraGO = Instantiate(Resources.Load("UICamera"),
                                          new Vector3(0f,0f,-10f), 
                                          Quaternion.Euler(180, 0, 0)) as GameObject;
        Camera camera = cameraGO.GetComponent<Camera>();
        camera.rect = new Rect(0, 0, 1, 1);
        camera.clearFlags = CameraClearFlags.Nothing;
    }

    // -- GAME EVENTS --

    public void DidScore(Player scorer, Flag flag) {
        teamScores[scorer.team]++;
        flag.Reset();
        allPlayers.ForEach(p => p.KillPlayer());
    }
}
