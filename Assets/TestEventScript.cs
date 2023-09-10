using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEventScript : MonoBehaviour
{
    /// <summary>
    /// This script only acts as the Publishing Event script.
    /// 
    /// All the logic that will happen when the event occurs will be contained within the publisher script.
    /// 
    /// The neat part: If we remove all the subscribers to an event, the Publisher will not put out an error. It's all one-way from Subscriber side.
    /// </summary>

    public event EventHandler<OnF5PressedEventArgs> OnF5Pressed; //This is how we define an event. EventHandler is part of the System namespace. It is the standard delegate used in System. The convention is "On" + "WhateverHappens".
    public class OnF5PressedEventArgs : EventArgs
    {
        public int F5Count;
    }

    public delegate void CustomEventDelegate(float f);
    public event CustomEventDelegate OnFloatEvent;

    public event Action<bool, int> OnTestActionEvent;

    private int incrementingF5Count;

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            incrementingF5Count++;
            //if (OnF5Pressed != null) { OnF5Pressed(this, EventArgs.Empty); }
            OnF5Pressed?.Invoke(this, new OnF5PressedEventArgs { F5Count = incrementingF5Count }); // 

            OnFloatEvent?.Invoke(5f);

            OnTestActionEvent?.Invoke(true, 2);
        }
    }

}
