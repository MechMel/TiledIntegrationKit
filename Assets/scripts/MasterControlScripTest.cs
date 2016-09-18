using UnityEngine;
using System.Collections;

public class MasterControlScripTest : MonoBehaviour
{
    public int experience;

    public int trial;

    public int Level
    {
        get { return experience / 750; }
    }

    public void SetLevel()
    {
        trial = experience;
    }
}
