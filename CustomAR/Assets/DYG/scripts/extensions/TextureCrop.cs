using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureCrop
{

    private enum Coords {
        CoordsOne,
        CoordsTwo
    }

    public static TextureExtension.Point[] FindCropCoordinates(this Texture2D aTex, Color cropColor)
    {
        Color[] pixels = aTex.GetPixels();
        int numOfPixels = pixels.Length; 
        int width = aTex.width;
        int height = aTex.height;

        int xOne = findXCoord(pixels, width, height, numOfPixels, cropColor);
        int yOne = findYCoord(pixels, width, numOfPixels, cropColor);
        int xTwo = findXCoord(pixels, width, height, numOfPixels, cropColor, Coords.CoordsTwo);
        int yTwo = findYCoord(pixels, width, numOfPixels, cropColor, Coords.CoordsTwo);
        
        return new [] { new TextureExtension.Point(xOne, yOne), new TextureExtension.Point(xTwo, yTwo) };
    }

    private static int findXCoord(Color[] pixels, int width, int height, int pixelCount, Color cropColor, Coords side = Coords.CoordsOne)
    {
        bool isSideOne = side == Coords.CoordsOne;
        int rowCountVal = isSideOne ? width : -width;
        short xCountVal = (short)(isSideOne ? 1 : -1);
        int startIdx = width; // We're getting a 1 pixel black line at the top
        int lastIdx = pixelCount - 1;
        int lastLineIdx = isSideOne ? (height - 1) * width : width;
        int idx = isSideOne ? startIdx : pixelCount - 1; 
        int xCoord = (short)(isSideOne ? 0 : width - 1);

        Func<bool> predicateUp = () => idx < pixelCount;

        Func<bool> predicateDown = () => idx > startIdx;

        Func<bool> predicate = side == Coords.CoordsOne ? predicateUp: predicateDown;
        
        Action assignIdxUp = () =>
        {
            xCoord += xCountVal;
            idx = xCoord;            
        };
        
        Action assignIdxDown = () =>
        {
            idx = lastIdx + xCoord;
            xCoord += xCountVal;
        };

        Action assignIdx = side == Coords.CoordsOne ? assignIdxUp: assignIdxDown;
        
        for (; predicate(); idx += rowCountVal)
        {
            Color pixel = pixels[idx];

            if (!pixel.Equals(cropColor))
            {
                break;
            }

            if ((lastLineIdx + xCoord) == idx)
            {
                assignIdx();
            }
        }

        return xCoord;
    }

    private static int findYCoord(Color[] pixels, int width, int pixelCount, Color cropColor, Coords side = Coords.CoordsOne)
    {
        int countVal = side == Coords.CoordsOne ? 1 : -1;
        int startIdx = width; // We're getting a 1 pixel black line at the top
        int idx = side == Coords.CoordsOne ? startIdx : pixelCount - 1;

        Func<bool> predicateUp = () => idx < pixelCount;

        Func<bool> predicateDown = () => idx > -1;

        Func<bool> predicate = side == Coords.CoordsOne ? predicateUp: predicateDown;
        
        for (; predicate(); idx += countVal)
        {
            Color pixel = pixels[idx];

            if (!pixel.Equals(cropColor))
            {
                break;
            }
        }

        return (idx / width);
    }
}
