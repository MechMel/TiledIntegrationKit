
using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(MasterControlScripTest))]
public class EditorTest : Editor
{
    public override void OnInspectorGUI()
    {

        EditorGUI.BeginChangeCheck();

        MasterControlScripTest myTarget = (MasterControlScripTest)target;

        myTarget.experience = EditorGUILayout.IntField("Experience", myTarget.experience);
        EditorGUILayout.LabelField("Trial", myTarget.trial.ToString());
        EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
        myTarget.SetLevel();

        // Block of code with controls
        // that may set GUI.changed to true.

        if (EditorGUI.EndChangeCheck())
        {
            // Code to execute if GUI.changed
            // was set to true inside the block of code above.
            Debug.Log("Value was changed do stuff here");
        }
    }


}
