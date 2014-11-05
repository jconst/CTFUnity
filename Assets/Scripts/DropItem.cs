using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DropItem : MonoBehaviour
{
    public Player owner;
    public float manaCost;

    public void WasDroppedByPlayer(Player player) {
        owner = player;
    }
}
