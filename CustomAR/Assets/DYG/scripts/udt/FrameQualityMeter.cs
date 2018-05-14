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
            button = GetComponent<Button>();
            ColorBlock buttonColors = button.colors;
            defaultButtonColor = buttonColors.normalColor;
            buttonText = GetComponentInChildren<Text>();
            defaultButtonTextColor = buttonText.color;
        }

        public bool IsRetry
        {
            get { return isRetry; }
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

        public void ToggleToRetry(bool toRetry = true)
        {
            setButtonQuality(ButtonQuality.Bad);

            buttonText.text = toRetry ? retryMsg : captureMsg;

            isRetry = toRetry;
        }
        
        private void setButtonQuality(ButtonQuality quality)
        {
            ColorBlock buttonColors = button.colors;
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
            
            button.colors = buttonColors;
            //GetComponentInChildren<Text>().color = textColor;
        }

        private Button button;
        private Text buttonText;
        
        private bool isRetry = false;
        
        private Color defaultButtonColor;
        private Color defaultButtonTextColor;
        
        private Color buttonColor = Color.green;
        private Color buttonTextColor = Color.white;

        private const string captureMsg = "Capture";
        private const string retryMsg = "Retry";

    }
}