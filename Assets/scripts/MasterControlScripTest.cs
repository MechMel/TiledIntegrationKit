using UnityEngine;
using System.Collections;
<<<<<<< HEAD
=======
using System.Collections.Generic;
>>>>>>> parent of 922254f... Renamed TIKEditorLevelControl / Removed old test classes

public class MasterControlScripTest : MonoBehaviour
{
    public int experience;

    public int trial;

    public int Level
    {
        get { return experience / 750; }
    }
<<<<<<< HEAD
    // test
=======

    public Texture2D texture2d;

    private Dictionary<string, Texture2D> TilesetTextures;
>>>>>>> parent of 922254f... Renamed TIKEditorLevelControl / Removed old test classes

    public void SetLevel()
    {
        trial = experience;
    }
}
