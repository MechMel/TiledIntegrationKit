using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Saver {
    public static void Save(GameObject player)
    {


        PlayerPrefs.SetInt("gold",player.GetComponent<PlayerUIBehaviour>().Coins);

    }
}
