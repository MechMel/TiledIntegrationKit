using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< HEAD
static class Saver {

    public static int level;

    public static void Save(GameObject player)
    {

        PlayerPrefs.SetInt("gold",player.GetComponent<PlayerUIBehaviour>().Coins);
        PlayerPrefs.SetInt("level max", PlayerPrefs.GetInt("level max")+1);
=======
static class saver {
    static void blah(object player)
    {


       // PlayerPrefs.SetInt("gold",);
>>>>>>> 5f16be3a0a4a329d02d3b71fa6c58700e09595db

    }
}
