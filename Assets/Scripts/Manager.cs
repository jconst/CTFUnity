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
    const int pointLimit = 5;

    // -- VARIABLES --
	public Dictionary<string, int> teamScores;
	public Dictionary<string, int> teamManas;
	public Dictionary<string, GUIText> teamScoreText;
	public Dictionary<string, ManaBar> teamManaBars;
    public Dictionary<string, GameObject> teamBases;
    public List<Player> allPlayers;
    public Flag flag;
    public GUIText countdownGUIText;
    public GUITexture countdownBackground;

    private float timePassed=0;
    public bool roundStarted = false;
    public bool itemPickups = false;

    static public Manager S {
        get {
            return GameObject.FindObjectOfType(typeof(Manager)) as Manager;
        }
    }

    // -- SETUP --

    void Start() {
        teamScores = InitScores(teams);
		teamManas = InitManas(teams);
        teamScoreText = InitScoreText(teams);
        allPlayers = SpawnPlayers();
        CreateOverlayCamera();
        CreateCountdown();

        if (itemPickups) {
            foreach(GameObject go in GameObject.FindGameObjectsWithTag("ItemSpawn"))
            {
                go.GetComponent<ItemSpawn>().activ=itemPickups;
            }
        }
        else
            teamManaBars = InitManaBars (teams);
        teamBases = teams.ToDictionary(team => team,                                 //key
                                       team => GameObject.FindWithTag(team+"Side")); //value
        flag = GameObject.FindObjectOfType(typeof(Flag)) as Flag;

        StartNewRound(true, null);
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
		if(PlayerOptions.playerColors[index] != "") {
			team = PlayerOptions.playerColors[index];
		}
        GameObject playerGO = Instantiate(Resources.Load("Player")) as GameObject;
        Player player = playerGO.GetComponent<Player>();

        Vector3 initialPos = spawnLocations[team] + (new Vector2(index, index));
        playerGO.transform.position = player.initialPos = initialPos;
        player.team = team;
        player.number = index;
        player.renderer.material.color = teamColors[team];
		player.pickups = itemPickups;

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

    void CreateCountdown() {
        GameObject countdownParent = Instantiate(Resources.Load("Countdown")) as GameObject;
        countdownGUIText = countdownParent.GetComponentInChildren<GUIText>();
        countdownBackground = countdownParent.GetComponentInChildren<GUITexture>();
    }

    // -- UPDATE --

    public void Update() {
        teamScoreText.ToList().ForEach(kvp => {
            GUIText gt = kvp.Value;
            int score = teamScores[kvp.Key];
            gt.text = score.ToString();
        });
		timePassed += Time.deltaTime;
		if (!itemPickups) {
    		if (timePassed >= manaTime) {
				teams.ForEach (team => {
					teamManas [team] += 1;
					teamManas [team] = Mathf.Min (teamManas [team], 3);
				});
				timePassed = 0;
    		}
    		teamManaBars.ToList ().ForEach (kvp => {
				ManaBar gt = kvp.Value;
				gt.currMana = teamManas [kvp.Key];
    		});
		}
    }

	// -- GAME EVENTS --
    public void StartNewRound(bool showRules, string teamScored)
    {
        roundStarted = false;
        allPlayers.ForEach(p => {
            p.Reset();
            p.frozen = true;
        });
        flag.Reset();

        StartCoroutine(CountdownCoroutine(showRules, teamScored));
    }

    public IEnumerator CountdownCoroutine(bool showRules, string teamScored)
    {
        countdownGUIText.enabled = countdownBackground.enabled = true;

        if (showRules) {
            countdownGUIText.text = "Hold the bomb in\nyour enemy's base!";
            yield return new WaitForSeconds(2);

            countdownGUIText.text = "First to " + pointLimit + " points wins!";
            yield return new WaitForSeconds(1.5f);
        }

        if (teamScored != null) {
            countdownGUIText.text = teamScored + " team scored!";
            yield return new WaitForSeconds(1);
        }

        for(int i=countdownLength; i > 0; i--) {
            countdownGUIText.text = i.ToString();
            yield return new WaitForSeconds(0.7f);
        }

        countdownGUIText.text = "Start!\n\n\n";
        allPlayers.ForEach(p => p.frozen = false);
        roundStarted = true;
        countdownBackground.enabled = false;

        yield return new WaitForSeconds(1);
        countdownGUIText.enabled = false;
        countdownGUIText.text = null;
    }

    public void DidScore(Player scorer) {
        StartCoroutine(ScoreCoroutine(scorer.team));
    }

    private IEnumerator ScoreCoroutine(string team) {
        allPlayers.ForEach(p => p.Die());
        yield return new WaitForSeconds(1);        
        if (++teamScores[team] >= pointLimit) {
            countdownGUIText.enabled = countdownBackground.enabled = true;
            countdownGUIText.text = team + " team wins!";
            yield return new WaitForSeconds(2);
            Application.LoadLevel(0);
        } else {
            StartNewRound(false, team);
        }
    }
}
