using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToogleSwitchButton : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Transform knob;
    [SerializeField] private float knobMovePositionX;
    [SerializeField] private float switchSpeed;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color disableColor;

    private bool currentState;
    public void OnButtonToogle(bool state)
    {
        var newKnobMovePositionX = state ? knobMovePositionX : -knobMovePositionX;
        knob.localPosition = Vector3.Lerp(knob.position, new Vector3(newKnobMovePositionX, 0f, 0f), switchSpeed);

        backgroundImage.color = state ? enableColor : disableColor;
        currentState = state;
    }
    public void OnButtonClick()
    {
        OnButtonToogle(!currentState);
    }
}
