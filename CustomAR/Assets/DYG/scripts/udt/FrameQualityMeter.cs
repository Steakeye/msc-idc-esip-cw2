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
            defaultButtonHoverColor = buttonColors.highlightedColor;
            buttonText = GetComponentInChildren<Text>();
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
            if (button == null || button.IsDestroyed())
            {
                return;
            }
            
            ColorBlock buttonColors = button.colors;
            Color textColor;
            
            Debug.Log("calling setButtonQuality");
            if (quality == ButtonQuality.Good)
            {
                buttonColors.normalColor = buttonColor;
                buttonColors.highlightedColor = buttonColor;
            }
            else
            {
                buttonColors.normalColor = defaultButtonColor;
                buttonColors.highlightedColor = defaultButtonHoverColor;
            }
            
            button.colors = buttonColors;
        }

        private Button button;
        private Text buttonText;
        
        private bool isRetry = false;
        
        private Color defaultButtonColor;
        private Color defaultButtonHoverColor;
        
        private Color buttonColor = Color.green;

        private const string captureMsg = "Capture";
        private const string retryMsg = "Retry";

    }
}