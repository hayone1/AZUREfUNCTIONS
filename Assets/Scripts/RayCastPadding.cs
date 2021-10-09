using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RayCastPadding : MonoBehaviour
{
    [SerializeField] private Button[] allButtons;
    [SerializeField] private Text[] allTexts;
    private float _rayCastPad = 1;
    public float rayCastPad{
        get{return _rayCastPad;}
        set{
            rayCastPad = value;
            if (allButtons != null){
                foreach (Button buttonImage in allButtons)
                {
                    //allow only the transparent part of the image to be registered
                    buttonImage.image.alphaHitTestMinimumThreshold = value;
                }

            }
        }
    }
    // Button[] allButtons;
    // Start is called before the first frame update
    private void OnEnable() {
        allButtons = GameObject.FindObjectsOfType<Button>();    //find all buttons in scene
        allTexts = GameObject.FindObjectsOfType<Text>();
        //set the alpha needed for a successful raycas from the buttons
        if (allButtons != null){

            foreach (var _button in allButtons)
            {
                _button.image.alphaHitTestMinimumThreshold = _rayCastPad;
            }
        }
        if (allTexts != null){

            foreach (var _text in allButtons)
            {
                _text.interactable = false; //remove all tex as raycast target
            }
        }
    }

}
