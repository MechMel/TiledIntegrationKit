using UnityEngine;
using System.Collections;
using UnityEditor;

public class PDKEditorUtil : Editor
{
    // Creates an interger field tied to a variable instance
    public void Field(string fieldName)
    {
        // Display this field
        EditorGUILayout.LabelField(fieldName);
    }

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

    // Creates an text field tied to a variable instance
    public bool Field(string fieldName, ref string stringInstance)
    {
        // Check to see if this field has been changed
        EditorGUI.BeginChangeCheck();
        // Display this field
        stringInstance = EditorGUILayout.TextField(fieldName, stringInstance);
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

    // Creates a field for a texture 2D
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

    // Creates a field for an object
    public bool Field(string fieldName, ref Object objectInstance)
    {
        // Check to see if this field has been changed
        EditorGUI.BeginChangeCheck();
        // Display this field
        objectInstance = (Object)EditorGUILayout.ObjectField(fieldName, objectInstance, typeof(Object), false);
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

    // Creates a field for an int array
    public void Field(string fieldName, string elementName, ref int[] arrayInstance)
    {
        // If the given array does not exist
        if (arrayInstance == null)
        {
            // Instatiate it
            arrayInstance = new int[0];
        }

        // This contorls the lenght of the given array
        int size = arrayInstance.Length;
        
        // Display the field name
        Field(fieldName);
        // When the size is changed
        if (Field("Size", ref size))
        {
            // This is used to copy the given array
            int[] newSolidTiles = new int[size];

            // For each int in the new array
            for (int thisIntIndex = 0; thisIntIndex < size; thisIntIndex++)
            {
                if (thisIntIndex < arrayInstance.Length) // If not all ints have been copied from the given array
                {
                    // Copy this int to the new array
                    newSolidTiles[thisIntIndex] = arrayInstance[thisIntIndex];
                }
                else // If all ints have been copied from the given array
                {
                    // Copy the last int of the given array
                    newSolidTiles[thisIntIndex] = arrayInstance[arrayInstance.Length - 1];
                }
            }
            // Adjust the size of the given array
            arrayInstance = new int[size];
            // Copy the ints back to the given array
            newSolidTiles.CopyTo(arrayInstance, 0);
        }
        // For each int in the given array
        for (int thisTileIndex = 0; thisTileIndex < size; thisTileIndex++)
        {
            // Display a field for this tile
            Field(elementName + " " + thisTileIndex, ref arrayInstance[thisTileIndex]);
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
