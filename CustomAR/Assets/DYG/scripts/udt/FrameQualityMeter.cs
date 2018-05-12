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

        private void Start()
        {
            ColorBlock buttonColors = GetComponent<Button>().colors;
            defaultButtonColor = buttonColors.normalColor;
            defaultButtonTextColor = GetComponentInChildren<Text>().color;
        }

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
            ColorBlock buttonColors = GetComponent<Button>().colors;
            Color textColor;
            
            Debug.Log("calling setButtonQuality");
            if (quality == ButtonQuality.Good)
            {
                buttonColors.normalColor = buttonColor;
                textColor = buttonTextColor;
            }
            else
            {
                buttonColors.normalColor = defaultButtonColor;
                textColor = defaultButtonTextColor;
            }
            
            GetComponent<Button>().colors = buttonColors;
            //GetComponentInChildren<Text>().color = textColor;
        }

        private Color defaultButtonColor;
        private Color defaultButtonTextColor;
        
        private Color buttonColor = Color.green;
        private Color buttonTextColor = Color.white;

    }
}