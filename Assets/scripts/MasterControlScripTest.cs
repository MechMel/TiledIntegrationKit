using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterControlScripTest : MonoBehaviour
{
    public int experience;

    public int trial;

    public int Level
    {
        get { return experience / 750; }
    }

    public Texture2D texture2d;

    private Dictionary<string, Texture2D> TilesetTextures;

    public void SetLevel()
    {
        trial = experience;
    }
}
