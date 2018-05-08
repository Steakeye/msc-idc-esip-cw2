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
        short yOne = findYCoord(pixels, width, numOfPixels, cropColor, Coords.CoordsTwo);
        short xTwo = findXCoord(pixels, width, height, numOfPixels, cropColor);
        short yTwo = findYCoord(pixels, width, numOfPixels, cropColor, Coords.CoordsTwo);
        
        return new [] { new TextureExtension.Point(xOne, yOne), new TextureExtension.Point(xTwo, yTwo) };
    }

    private static short findXCoord(Color[] pixels, int width, int height, int pixelCount, Color cropColor, Coords side = Coords.CoordsOne)
    {
        int countVal = side == Coords.CoordsOne ? height : -height;
        int yCountVal = side == Coords.CoordsOne ? 1 : -1;
        int idx = side == Coords.CoordsOne ? 0 : pixelCount;
        short xCoord = (short)(side == Coords.CoordsOne ? 0 : height - 1);
        short yCoord = (short)(side == Coords.CoordsOne ? 0 : pixelCount);

        Func<bool> predicateUp = () => idx < pixelCount;

        Func<bool> predicateDown = () => idx > -1;

        Func<bool> predicate = side == Coords.CoordsOne ? predicateUp: predicateDown;
        
        for (; predicate(); idx += countVal)
        {
            Color pixel = pixels[idx];

            if (pixel.Equals(cropColor))
            {
                break;
            }

            if (idx % height == 0)
            {
                idx -= pixelCount;
            }
        }

        return (short)(idx / height);
        
    }

    private static short findYCoord(Color[] pixels, int width, int pixelCount, Color cropColor, Coords side = Coords.CoordsOne)
    {
        int countVal = side == Coords.CoordsOne ? 1 : -1;
        int idx = side == Coords.CoordsOne ? 0 : pixelCount;

        Func<bool> predicateUp = () => idx < pixelCount;

        Func<bool> predicateDown = () => idx > -1;

        Func<bool> predicate = side == Coords.CoordsOne ? predicateUp: predicateDown;
        
        for (; predicate(); idx += countVal)
        {
            Color pixel = pixels[idx];

            if (pixel.Equals(cropColor))
            {
                break;
            }
        }

        /*short xCoord = (short)(idx / width);
        
        return xCoord*/

        return (short)(idx / width);
    }
}
