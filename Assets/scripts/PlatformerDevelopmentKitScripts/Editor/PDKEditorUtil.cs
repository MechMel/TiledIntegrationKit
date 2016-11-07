using UnityEngine;
using System.Collections;
using UnityEditor;

public class PDKEditorUtil : Editor
{
    // Creates an interger field tied to a variable instance
    public bool Field(string fieldName, ref int intInstance)
    {
        // Check to see if this field has been changed
        EditorGUI.BeginChangeCheck();
        // Display this field
        intInstance = EditorGUILayout.IntField(fieldName, intInstance);
        // If this field has been changed
        if (EditorGUI.EndChangeCheck())
        {
            // Tell the caller this field has been changed
            return true;
        }
        else
        {
            // Tell the caller this field has not been changed
            return false;
        }
    }

    // Creates an boolean field tied to a variable instance
    public bool Field(string fieldName, ref bool boolInstance)
    {
        // Check to see if this field has been changed
        EditorGUI.BeginChangeCheck();
        // Display this field
        boolInstance = EditorGUILayout.Toggle(fieldName, boolInstance);
        // If this field has been changed
        if (EditorGUI.EndChangeCheck())
        {
            // Tell the caller this field has been changed
            return true;
        }
        else
        {
            // Tell the caller this field has not been changed
            return false;
        }
    }

    // Creates an TextAsset field tied to a variable instance
    public bool Field(string fieldName, ref TextAsset textAssetInstance)
    {
        // Check to see if this field has been changed
        EditorGUI.BeginChangeCheck();
        // Display this field
        textAssetInstance = (TextAsset)EditorGUILayout.ObjectField(fieldName, textAssetInstance, typeof(TextAsset), false);
        // If this field has been changed
        if (EditorGUI.EndChangeCheck())
        {
            // Tell the caller this field has been changed
            return true;
        }
        else
        {
            // Tell the caller this field has not been changed
            return false;
        }
    }

    // Creates a field for map type selection
    public bool Field(string fieldName, ref PDKLevelConfigurator.mapTypes mapTypesInstance)
    {
        // Check to see if this field has been changed
        EditorGUI.BeginChangeCheck();
        // Display this field
        mapTypesInstance = (PDKLevelConfigurator.mapTypes)EditorGUILayout.EnumPopup(fieldName, mapTypesInstance);
        // If this field has been changed
        if (EditorGUI.EndChangeCheck())
        {
            // Tell the caller this field has been changed
            return true;
        }
        else
        {
            // Tell the caller this field has not been changed
            return false;
        }
    }

    // Creates a field for map type selection
    public bool Field(string fieldName, ref Texture2D textureInstance)
    {
        // Check to see if this field has been changed
        EditorGUI.BeginChangeCheck();
        // Display this field
        textureInstance = (Texture2D)EditorGUILayout.ObjectField(fieldName, textureInstance, typeof(Texture2D), false);
        // If this field has been changed
        if (EditorGUI.EndChangeCheck())
        {
            // Tell the caller this field has been changed
            return true;
        }
        else
        {
            // Tell the caller this field has not been changed
            return false;
        }
    }

    // Creates an boolean field tied to a variable instance
    public bool Button(string buttonName)
    {
        // Check to see if this field has been changed
        EditorGUI.BeginChangeCheck();
        // Display this buton
        GUILayout.Button(buttonName);
        // If this field has been changed
        if (EditorGUI.EndChangeCheck())
        {
            // Tell the caller this field has been changed
            return true;
        }
        else
        {
            // Tell the caller this field has not been changed
            return false;
        }
    }
}
