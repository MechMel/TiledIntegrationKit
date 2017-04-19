using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PDKSerializable2DHashestOfInts
{
    [SerializeField]
    // This will store all the data from a two-dimensional array in a one-dimensional format
    private PDKLayer.PDKObjectIDHashSet[] oneDimensionArray;
    // These store the width and the height of the two-dimensional array
    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;
    // These allow other classes to read _width and _height, 
    // but prevent external classes from over writing _width and _height.
    public int Width
    {
        get { return _width; }
    }
    public int Height
    {
        get { return _height; }
    }

    
    public PDKSerializable2DHashestOfInts(int width, int height)
    {
        // Set the width and height of the two-dimensional array
        _width = width;
        _height = height;
        // Initialize the one-dimensional array
        oneDimensionArray = new PDKLayer.PDKObjectIDHashSet[_width * _height];
    }

    // This returns an item in the one-dimensional array, bassed on user-specified 2D coordinates
    public PDKLayer.PDKObjectIDHashSet GetIDs(int x, int y)
    {
        // Convert the users-specified 2D coordinates into an one-demnsional index
        int rasterIndex = (Mathf.Abs(y) * _width) + Mathf.Abs(x);
        // Return the item from the user-specified coordinates
        return oneDimensionArray[rasterIndex];
    }

    // This sets an item in the one-dimensional array, bassed on user-specified 2D coordinates
    public void SetIDs(int x, int y, PDKLayer.PDKObjectIDHashSet idsToSet)
    {
        // Convert the users-specified 2D coordinates into an one-demnsional index
        int rasterIndex = (Mathf.Abs(y) * _width) + Mathf.Abs(x);
        // Set the item at the user-specified coordinates
        oneDimensionArray[rasterIndex] = idsToSet;
    }

    // This adds an item in the one-dimensional array, bassed on user-specified 2D coordinates
    public void AddID(int x, int y, int idToAdd)
    {
        // Convert the users-specified 2D coordinates into an one-demnsional index
        int rasterIndex = (Mathf.Abs(y) * _width) + Mathf.Abs(x);
        // Add the item at the user-specified coordinates
        oneDimensionArray[rasterIndex].Add(idToAdd);
    }

    // This removes an item in the one-dimensional array, bassed on user-specified 2D coordinates
    public void RemoveID(int x, int y, int idToRemove)
    {
        // Convert the users-specified 2D coordinates into an one-demnsional index
        int rasterIndex = (Mathf.Abs(y) * _width) + Mathf.Abs(x);
        // Remove the item at the user-specified coordinates
        oneDimensionArray[rasterIndex].Add(idToRemove);
    }
}
