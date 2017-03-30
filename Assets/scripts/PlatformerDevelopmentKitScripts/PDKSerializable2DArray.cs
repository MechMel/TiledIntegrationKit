using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PDKSerializable2DArray
{
    [SerializeField]
    // This will store all the data from a two-dimensional array in a one-dimensional format
    private PDKLayer.PDKDehydratedObjectsHashSet[] oneDimensionArray;
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

    
    public PDKSerializable2DArray(int width, int height)
    {
        // Set the width and height of the two-dimensional array
        _width = width;
        _height = height;
        // Initialize the one-dimensional array
        oneDimensionArray = new PDKLayer.PDKDehydratedObjectsHashSet[_width * _height];
    }

    // This returns an item in the one-dimensional array, bassed on user-specified 2D coordinates
    public PDKLayer.PDKDehydratedObjectsHashSet GetItem(int x, int y)
    {
        // Convert the users-specified 2D coordinates into an one-demnsional index
        int rasterIndex = (y * _width) + x;
        // Return the item from the user-specified coordinates
        return oneDimensionArray[rasterIndex];
    }

    // This sets an item in the one-dimensional array, bassed on user-specified 2D coordinates
    public void SetItem(int x, int y, PDKLayer.PDKDehydratedObjectsHashSet ItemToSet)
    {
        // Convert the users-specified 2D coordinates into an one-demnsional index
        int rasterIndex = (y * _width) + x;
        // Set the item at the user-specified coordinates
        oneDimensionArray[rasterIndex] = ItemToSet;
    }
}
