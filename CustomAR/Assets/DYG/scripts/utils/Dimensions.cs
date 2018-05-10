using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DYG.utils
{
    public enum AspectRatio
    {
        LandScape,
        Portrait,
        Square
    }
    
    public static class Dimensions {

        public static AspectRatio GetAspectRatio(int width, int height)
        {
            AspectRatio aspectR; // = AspectRatio.Unknown;
            
            if (width == height)
            {
                aspectR = AspectRatio.Square;
            } else if (width > height)
            {
                aspectR = AspectRatio.LandScape;
            }
            else
            {
                aspectR = AspectRatio.Portrait;
            }

            return aspectR;
        }
        
        public static float GetAspectRatioFloat(int width, int height)
        {
            return GetAspectRatioFloat((float)width, (float)height);
        }
        public static float GetAspectRatioFloat(float width, float height)
        {
            return (float)width/(float)height;
        }
    }
}
