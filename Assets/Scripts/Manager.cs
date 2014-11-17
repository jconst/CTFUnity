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
        {"Blue", 1},
        {"Red", 1}
    };

    public Dictionary<string, Vector2> spawnLocations =
       new Dictionary<string, Vector2> {
       {"Blue", new Vector2(6f, -3.5f)},
       {"Red", new Vector2(-6f, 1.5f)}
    };

    const int countdownLength = 3;
    const bool splitScreen = false;
    const float manaTime = 10f;
    const float cameraEasing = 0.01f;

    // -- VARIABLES --
	public Dictionary<string, int> teamScores;
	public Dictionary<string, float> teamManas;
	public Dictionary<string, GUIText> teamScoreText;
	public Dictionary<string, GUIText> teamManaText;
    public List<Player> allPlayers;
    public List<Camera> playerCameras;
    public Camera mainCamera;
    public float roundStartTime = 0f;
	private float timePassed=0;

    public GUIText countdownGUIText;

    static public Manager S {
        get {
            return GameObject.FindObjectOfType(typeof(Manager)) as Manager;
        }
    }

    public bool countingDown {
        get {
            return (countdownLength - (Time.time - roundStartTime)) > -1;
        }
    }

    // -- SETUP --

    void Start() {
        teamScores = InitScores(teams);
		teamManas = InitManas(teams);
        teamScoreText = InitScoreText(teams);
		teamManaText = InitManaText (teams);
        allPlayers = SpawnPlayers();
        // CreateBackgroundCamera();
        // playerCameras = AttachCamerasToPlayers(allPlayers);
        CreateOverlayCamera();

        InitCountdown();
        StartNewRound();
    }

    Dictionary<string, int> InitScores(List<string> teamList) {
        return teamList.ToDictionary(t => t, t => 0);
    }
	Dictionary<string, float> InitManas(List<string> teamList) {
		return teamList.ToDictionary(t => t, t => 0f);
	}
	
	Dictionary<string, GUIText> InitScoreText(List<string> teamList) {
        GameObject scoreBoard = GameObject.FindWithTag("ScoreBoard");
        return teamList.ToDictionary(t => t, t => {
            GameObject go = Instantiate(Resources.Load("ScoreText")) as GameObject;
            GUIText gt = go.GetComponent<GUIText>();
            float width = .5f / teamList.Count();
            float index = (float)teamList.IndexOf(t);
            go.transform.parent = scoreBoard.transform;
            go.transform.localScale = new Vector2(width, 1);
            go.transform.localPosition = new Vector3(index*width - 0.15f, 0.1f, 1);
            gt.color = teamColors[t];
            gt.fontSize = (int)(Screen.height * 0.04);
            return gt;
        });
    }

	Dictionary<string, GUIText> InitManaText(List<string> teamList) {
		GameObject scoreBoard = GameObject.FindWithTag("ScoreBoard");
		return teamList.ToDictionary(t => t, t => {
			GameObject go = Instantiate(Resources.Load("ManaText")) as GameObject;
			GUIText gt = go.GetComponent<GUIText>();
			float width = 1.4f / teamList.Count();
			float index = (float)teamList.IndexOf(t);
			go.transform.parent = scoreBoard.transform;
			go.transform.localScale = new Vector2(width, 1);
			go.transform.localPosition = new Vector3(index*width - 0.35f, 0, 1);
			gt.color = teamColors[t];
			gt.fontSize = (int)(Screen.height * 0.03);
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
        Vector2 posOffset = new Vector2(index, index);
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
	
    void InitCountdown()
    {
        GameObject countdownParent = GameObject.FindWithTag("Countdown");
        countdownGUIText = countdownParent.GetComponentInChildren<GUIText>();
    }

    // -- UPDATE --

    public void Update() {
        playerCameras.ForEach(c => c.enabled = splitScreen);
        teamScoreText.ToList().ForEach(kvp => {
            GUIText gt = kvp.Value;
            int score = teamScores[kvp.Key];
            gt.text = score.ToString();
        });
		timePassed += Time.deltaTime;
		if (timePassed >= manaTime) {
			teams.ForEach(team => {
				teamManas[team]+=1f;
				teamManas[team]=Mathf.Min(teamManas[team], 10f);
			});
			timePassed=0;
		}
		teamManaText.ToList().ForEach(kvp => {
			GUIText gt = kvp.Value;
			gt.text = teamManas[kvp.Key].ToString();
		});
        if (countingDown) {
            int countRemaining = countdownLength - (int)Mathf.Floor(Time.time - roundStartTime);
            countdownGUIText.text = countRemaining > 0 ? countRemaining.ToString() : "Start!";
        } else {
    		fitAllPlayersInCamera();
	    }
    }

    void fitAllPlayersInCamera() {
        float maxX = allPlayers.Max(p => p.transform.position.x) + 1f;
        float maxY = allPlayers.Max(p => p.transform.position.y) + 1f;
        float minX = allPlayers.Min(p => p.transform.position.x) - 1f;
        float minY = allPlayers.Min(p => p.transform.position.y) - 1f;

        float lastSize = mainCamera.orthographicSize;
        float newSize = Mathf.Max(maxX - minX, maxY - minY) / 2f;
        float minSize = 6f;
        float adjustedNewSize = Mathf.Max(newSize, minSize);
        mainCamera.orthographicSize = Mathf.Lerp(lastSize, adjustedNewSize, cameraEasing);

        Vector3 lastPos = mainCamera.transform.position;
        Vector3 newPos = new Vector3(Mathf.Lerp(minX, maxX, 0.5f), Mathf.Lerp(minY, maxY, 0.5f), -10);
        mainCamera.transform.position = Vector3.Lerp(lastPos, newPos, 0.03f);
    }
	
	// -- GAME EVENTS --

    public void StartNewRound()
    {
        StartCoroutine(NewRoundCoroutine());
    }

    public IEnumerator NewRoundCoroutine()
    {
        roundStartTime = Time.time;
        allPlayers.ForEach(p => p.frozen = true);

        yield return new WaitForSeconds(countdownLength+1);
        allPlayers.ForEach(p => p.frozen = false);

        GameObject countdownParent = GameObject.FindWithTag("Countdown");
        Destroy(countdownParent);
    }

    public void DidScore(Player scorer, Flag flag) {
        teamScores[scorer.team]++;
        flag.Reset();
        allPlayers.ForEach(p => p.KillPlayer());
    }

	public bool SubManaCost(Player dropper, float cost)
	{
		if (teamManas [dropper.team] < cost)
			return false;
		teamManas [dropper.team] -= cost;
		return true;
	}
}
