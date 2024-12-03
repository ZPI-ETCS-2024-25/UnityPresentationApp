using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ScrollButtonControl : MonoBehaviour
{
    public Slider slider;

    public void sliderChange(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        float value = context.ReadValue<float>();
        if (value == 1)
        {
            //Debug.Log(slider.value);
            slider.value = Mathf.Min(slider.value + 1,slider.maxValue);
            //Debug.Log(slider.value);
        }
        else if (value == -1)
        {
            //Debug.Log(slider.value);
            slider.value = Mathf.Max(slider.value -1,slider.minValue);
            //Debug.Log(slider.value);
        }
    }
}
