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

        short xOne = findXCoord(pixels, width, height, numOfPixels, cropColor);
        short yOne = findYCoord(pixels, width, numOfPixels, cropColor);
        short xTwo = findXCoord(pixels, width, height, numOfPixels, cropColor, Coords.CoordsTwo);
        short yTwo = findYCoord(pixels, width, numOfPixels, cropColor, Coords.CoordsTwo);
        
        return new [] { new TextureExtension.Point(xOne, yOne), new TextureExtension.Point(xTwo, yTwo) };
    }

    private static short findXCoord(Color[] pixels, int width, int height, int pixelCount, Color cropColor, Coords side = Coords.CoordsOne)
    {
        int rowCountVal = side == Coords.CoordsOne ? width : -width;
        short xCountVal = (short)(side == Coords.CoordsOne ? 1 : -1);
        int startIdx = width;
        int lastLineIdx = (height - 1) * width;
        int idx = side == Coords.CoordsOne ? startIdx : pixelCount - 1; // We're getting a 1 pixel black line at the top
        short xCoord = (short)(side == Coords.CoordsOne ? 0 : width - 1);

        Func<bool> predicateUp = () => idx < pixelCount;

        Func<bool> predicateDown = () => idx > -1;

        Func<bool> predicate = side == Coords.CoordsOne ? predicateUp: predicateDown;
        
        for (; predicate(); idx += rowCountVal)
        {
            Color pixel = pixels[idx];

            if (!pixel.Equals(cropColor))
            {
                break;
            }

            if ((lastLineIdx + xCoord) == idx)
            {
                xCoord += xCountVal;
                idx = xCoord;
            }
        }

        return xCoord;
    }

    private static short findYCoord(Color[] pixels, int width, int pixelCount, Color cropColor, Coords side = Coords.CoordsOne)
    {
        int countVal = side == Coords.CoordsOne ? 1 : -1;
        int idx = side == Coords.CoordsOne ? 0 : pixelCount - 1;

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

        return (short)(idx / width);
    }
}
