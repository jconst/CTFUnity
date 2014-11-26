using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerOptions : MonoBehaviour {

	public static int maxPlayers = 4;
	public static int numPlayers = 0;
	public static Dictionary<int, string> teamForPlayer =
			  new Dictionary<int, string>();
}
