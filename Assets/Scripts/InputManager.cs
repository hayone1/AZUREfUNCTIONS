using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class InputManager : MonoBehaviour
{   
    public InputAction ActuateAction;
    public event EventHandler<Vector2> OnMove;  //this event can easily be passed around
    //the vector2 would be the current position of the pointer
    
    // Start is called before the first frame update
    void Start()
    {
        ActuateAction.performed += (context) => {
            if (Touchscreen.current.primaryTouch.isInProgress){
                OnContactDelta(Touchscreen.current.primaryTouch.position.ReadValue());
                }
        };
        ActuateAction.Enable();
        
    }

    void OnContactDelta(Vector2 _contactPosition)
    {
        OnMove?.Invoke(this, _contactPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
