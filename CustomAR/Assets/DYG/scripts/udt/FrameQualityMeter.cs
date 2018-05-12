/*===============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using UnityEngine.UI;

namespace DYG.udt
{
    enum ButtonQuality
    {
        Bad,
        Good
    }

    public class FrameQualityMeter : MonoBehaviour
    {
        /*void SetMeter(Color low, Color med, Color high)
        {
            if (LowMedHigh.Length == 3)
            {
                if (LowMedHigh[0])
                    LowMedHigh[0].color = low;
                if (LowMedHigh[1])
                    LowMedHigh[1].color = med;
                if (LowMedHigh[2])
                    LowMedHigh[2].color = high;
            }
        }*/

        public void SetQuality(Vuforia.ImageTargetBuilder.FrameQuality quality)
        {
            switch (quality)
            {
                case (Vuforia.ImageTargetBuilder.FrameQuality.FRAME_QUALITY_NONE):
                case (Vuforia.ImageTargetBuilder.FrameQuality.FRAME_QUALITY_LOW):
                    setButtonQuality(ButtonQuality.Bad);
                    break;
                case (Vuforia.ImageTargetBuilder.FrameQuality.FRAME_QUALITY_MEDIUM):
                case (Vuforia.ImageTargetBuilder.FrameQuality.FRAME_QUALITY_HIGH):
                    setButtonQuality(ButtonQuality.Good);
                    break;
            }
        }

        private void setButtonQuality(ButtonQuality quality)
        {
            Debug.Log("calling setButtonQuality");
        }

        private void OnClick()
        {
            Debug.Log("calling OnClick");
        }
    }
}