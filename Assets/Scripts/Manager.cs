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

    public Dictionary<string, int> teamLayers =
       new Dictionary<string, int> {
       {"Blue", 9},
       {"Red", 10}
    };

    public float manaTime = 10f;
    const int countdownLength = 3;
    const int pointLimit = 5;

    // -- VARIABLES --
    public Dictionary<string, Vector2> spawnLocations =
       new Dictionary<string, Vector2> {
       {"Blue", new Vector2(0, -4f)},
       {"Red", new Vector2(0, 4f)}
    };
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
    private bool isPause = false;
    private bool tutorial = true;

    static public Manager S {
        get {
            return GameObject.FindObjectOfType(typeof(Manager)) as Manager;
        }
    }

    // -- SETUP --

    void Start() {
        Time.timeScale = 1.2f;
        teamScores = InitScores(teams);
        teamScoreText = InitScoreText(teams);
        allPlayers = SpawnPlayers();
        CreateCountdown();

        teamManaBars = InitManaBars (teams);

        teamBases = teams.ToDictionary(team => team,                                 //key
                                       team => GameObject.FindWithTag(team+"Side")); //value
        flag = GameObject.FindObjectOfType(typeof(Flag)) as Flag;

        AudioManager.Main.PlayNewSound("Background", loop: true);
           
        StartNewRound(false, null);
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
            float width = .8f / teamList.Count();
            float index = (float)teamList.IndexOf(t);
            go.transform.parent = scoreBoard.transform;
            go.transform.localScale = new Vector2(width, 1);
            go.transform.localPosition = new Vector3(index*width - 0.2f, 0.1f, 1);
            gt.color = teamColors[t];
            gt.fontSize = (int)(Screen.height * 0.04);
            return gt;
        });
    }

	Dictionary<string, ManaBar> InitManaBars(List<string> teamList) {
		return teamList.ToDictionary(t => t, t => {
			GameObject go = Instantiate(Resources.Load("ManaBar")) as GameObject;
			ManaBar mb = go.GetComponent<ManaBar>();
			mb.team = t;
			GUIText gt = go.GetComponentInChildren<GUIText>();
            if (itemPickups) {
                Destroy(gt.gameObject);
            }
			float index = (float)teamList.IndexOf(t);
			go.transform.position = new Vector3(Mathf.Abs(index - 0.04f), 0.5f, -1);
			gt.color = teamColors[t];
            gt.fontSize = Screen.width / 100;
			return mb;
		});
	}
	
	List<Player> SpawnPlayers() {
        if (PlayerOptions.teamForPlayer.Count == 0) {
            //for testing without needing to load menu
            PlayerOptions.teamForPlayer[0] = "Red";
            PlayerOptions.teamForPlayer[3] = "Blue";
        }
		return PlayerOptions.teamForPlayer.Select(kvp => SpawnPlayer(kvp.Value, kvp.Key)).ToList();
    }

    Player SpawnPlayer(string team, int index) {
        GameObject playerGO = Instantiate(Resources.Load("Player")) as GameObject;
        Player player = playerGO.GetComponent<Player>();

        Vector3 initialPos = spawnLocations[team];
        spawnLocations[team] = Quaternion.Euler(0,0,20) * spawnLocations[team];
        playerGO.transform.position = player.initialPos = initialPos;
        playerGO.layer = teamLayers[team];
        player.team = team;
        player.number = index;
        player.renderer.material.color = teamColors[team];

        return player;
    }

    void CreateCountdown() {
        GameObject countdownParent = Instantiate(Resources.Load("Countdown")) as GameObject;
        countdownGUIText = countdownParent.GetComponentInChildren<GUIText>();
        countdownBackground = countdownParent.GetComponentInChildren<GUITexture>();
        countdownGUIText.fontSize = Screen.width / 18;
    }

    // -- UPDATE --

    public void Update() {          
        if(Input.GetButtonDown("start") || Input.GetKeyDown(KeyCode.Return)) {
            if(tutorial) {
                tutorial = false;
                teamScores = InitScores(teams);
                StartNewRound(true, null);   
            } else {
                isPause = !isPause;
                if(isPause)
                  Time.timeScale = 0;
                else
                  Time.timeScale = 1;
            }
        }

        if(!tutorial) {
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
    }

    private void OnGUI() {
        Rect fullScreen = new Rect(0, 0, Screen.width, Screen.height);

        if(isPause)
            GUI.Window(0, fullScreen, PauseMenu, "PAUSED");
    }

    private void PauseMenu(int windowID) {
        
    }

	// -- GAME EVENTS --
    public void DidScore(Player scorer) {
        StartCoroutine(ScoreCoroutine(scorer.team));
    }

    private IEnumerator ScoreCoroutine(string team) {
        allPlayers.ForEach(p => p.Die());
        (GameObject.FindObjectsOfType(typeof(Turret)) as Turret[])
                   .ToList()
                   .ForEach(t => t.Deactivate());
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

    public void StartNewRound(bool showRules, string teamScored)
    {
        roundStarted = false;
        allPlayers.ForEach(p => {
            p.Reset();
            p.frozen = true;
        });
        flag.Reset();
        (GameObject.FindGameObjectsWithTag("ItemPickup") as GameObject[])
                   .ToList()
                   .ForEach(Destroy);
        timePassed = 0;

        StartCoroutine(CountdownCoroutine(showRules, teamScored));
    }

    public IEnumerator CountdownCoroutine(bool showRules, string teamScored)
    {
        countdownGUIText.enabled = countdownBackground.enabled = true;
        teamManas = InitManas(teams);

        if (showRules) {
            yield return new WaitForSeconds(1);
            
            countdownGUIText.text = "Hold the bomb in\nyour enemy's base!";
            yield return new WaitForSeconds(2);

            countdownGUIText.text = "First to " + pointLimit + " points wins!";
            yield return new WaitForSeconds(1.5f);
        }

        if (!tutorial) {
            for(int i=countdownLength; i > 0; i--) {
                countdownGUIText.text = i.ToString();
                yield return new WaitForSeconds(0.7f);
            }
            countdownGUIText.text = "Start!\n\n\n";
        }

        if(tutorial && teamScored == null) {
            countdownGUIText.text = "Press Start to skip tutorial";
            yield return new WaitForSeconds(2f);

            countdownGUIText.text = "Left stick for movement";
            yield return new WaitForSeconds(1.5f);

            countdownGUIText.text = "Right stick for tackling";
            yield return new WaitForSeconds(1.5f);

            countdownGUIText.text = "Face buttons to drop items";
            yield return new WaitForSeconds(1.5f);
        } else if (teamScored != null) {
            countdownGUIText.text = teamScored + " team scored!";
            yield return new WaitForSeconds(1);
        }

        allPlayers.ForEach(p => p.frozen = false);
        roundStarted = true;
        countdownBackground.enabled = false;

        yield return new WaitForSeconds(1);
        countdownGUIText.enabled = false;
        countdownGUIText.text = null;
    }
}
