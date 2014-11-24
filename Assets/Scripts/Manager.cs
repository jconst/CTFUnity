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
        {"Blue", new Color(58f/255f, 112f/255f, 225f/255f, 1f)},
        {"Red", Color.red}
    };

    public Dictionary<string, int> teamSizes =
       new Dictionary<string, int> {
        {"Blue", 1},
        {"Red", 1}
    };

    public Dictionary<string, Vector2> spawnLocations =
       new Dictionary<string, Vector2> {
       {"Blue", new Vector2(5f, -1f)},
       {"Red", new Vector2(-5f, 1f)}
    };

    public float manaTime = 10f;
    const int countdownLength = 3;

    // -- VARIABLES --
	public Dictionary<string, int> teamScores;
	public Dictionary<string, int> teamManas;
	public Dictionary<string, GUIText> teamScoreText;
	public Dictionary<string, ManaBar> teamManaBars;
    public List<Player> allPlayers;
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
		teamManaBars = InitManaBars (teams);
        allPlayers = SpawnPlayers();
        CreateOverlayCamera();

        StartNewRound();
    }

    Dictionary<string, int> InitScores(List<string> teamList) {
        return teamList.ToDictionary(t => t, t => 0);
    }
	Dictionary<string, int> InitManas(List<string> teamList) {
		return teamList.ToDictionary(t => t, t => 0);
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

	Dictionary<string, ManaBar> InitManaBars(List<string> teamList) {
		GameObject scoreBoard = GameObject.FindWithTag("ScoreBoard");
		return teamList.ToDictionary(t => t, t => {
			GameObject go = Instantiate(Resources.Load("ManaText")) as GameObject;
			ManaBar mb = go.GetComponent<ManaBar>();
			mb.team=t;
			GUIText gt= go.GetComponent<GUIText>();
			float index = (float)teamList.IndexOf(t);
			go.transform.position = new Vector3(Mathf.Abs(index - 0.04f), 0.3f, 1);
			gt.color = teamColors[t];
			return mb;
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
        GameObject playerGO = Instantiate(Resources.Load("Player")) as GameObject;
        Player player = playerGO.GetComponent<Player>();

        Vector3 initialPos = spawnLocations[team] + (new Vector2(index, index));
        playerGO.transform.position = player.initialPos = initialPos;
        player.team = team;
        player.number = index;
        player.renderer.material.color = teamColors[team];

        return player;
    }

    void CreateOverlayCamera() {
        GameObject cameraGO = Instantiate(Resources.Load("UICamera"),
                                          new Vector3(0f,0f,-10f), 
                                          Quaternion.Euler(180, 0, 0)) as GameObject;
        Camera camera = cameraGO.GetComponent<Camera>();
        camera.rect = new Rect(0, 0, 1, 1);
        camera.clearFlags = CameraClearFlags.Nothing;
    }

    // -- UPDATE --

    public void Update() {
        teamScoreText.ToList().ForEach(kvp => {
            GUIText gt = kvp.Value;
            int score = teamScores[kvp.Key];
            gt.text = score.ToString();
        });
		timePassed += Time.deltaTime;
		if (timePassed >= manaTime) {
			teams.ForEach(team => {
				teamManas[team] += 1;
				teamManas[team] = Mathf.Min(teamManas[team], 3);
			});
			timePassed=0;
		}
		teamManaBars.ToList().ForEach(kvp => {
			ManaBar gt = kvp.Value;
			gt.currMana = teamManas[kvp.Key];
		});
        if (countingDown) {
            int countRemaining = countdownLength - (int)Mathf.Floor(Time.time - roundStartTime);
            countdownGUIText.text = countRemaining > 0 ? countRemaining.ToString() : "Start!";
        }
    }
	
	// -- GAME EVENTS --

    public void StartNewRound()
    {
        StartCoroutine(NewRoundCoroutine());
    }

    public IEnumerator NewRoundCoroutine()
    {
        roundStartTime = Time.time;
        allPlayers.ForEach(p => {
            p.Reset();
            p.frozen = true;
        });
        Flag flag = GameObject.FindObjectOfType(typeof(Flag)) as Flag;
        flag.Reset();

        GameObject countdownParent = Instantiate(Resources.Load("Countdown")) as GameObject;
        countdownGUIText = countdownParent.GetComponentInChildren<GUIText>();

        yield return new WaitForSeconds(countdownLength+1);
        allPlayers.ForEach(p => p.frozen = false);

        Destroy(countdownParent);
    }

    public void DidScore(Player scorer) {
        teamScores[scorer.team]++;
        StartNewRound();
    }
}
