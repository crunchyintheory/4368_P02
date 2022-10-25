using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public event Action PressedConfirm = delegate { };
    public event Action PressedCancel = delegate { };
    public event Action PressedLeft = delegate { };
    public event Action PressedRight = delegate { };

    // Update is called once per frame
    void Update()
    {
        DetectConfirm();
        DetectCancel();
        DetectLeft();
        DetectRight();
    }

    private void DetectRight()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            this.PressedRight?.Invoke();
        }
    }
    
    private void DetectLeft()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            this.PressedLeft?.Invoke();
        }
    }
    
    private void DetectCancel()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.PressedCancel?.Invoke();
        }
    }
    
    private void DetectConfirm()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.PressedConfirm?.Invoke();
        }
    }
}
