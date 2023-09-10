using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TestEventScript;

public class TestEventSubscriberScript : MonoBehaviour
{

    /// <summary>
    /// This script acts as the subscriber to the event of TestEventScript.
    /// Subscriber's logic can subscribe and unsubscribe to an event.
    /// </summary>

    TestEventScript testEventScript; // reference to the publisher script


    private void Start()
    {
        testEventScript = this.GetComponent<TestEventScript>(); // reference to the publisher script
        testEventScript.OnF5Pressed += Testing_OnSpacePressed; // Subscribing to the publisher script's Event.
        testEventScript.OnFloatEvent += TestEventScript_OnFloatEvent; ;
        testEventScript.OnTestActionEvent += TestEventScript_OnTestActionEvent;
    }

    private void TestEventScript_OnTestActionEvent(bool arg1, int arg2)
    {
        throw new NotImplementedException();
    }

    private void TestEventScript_OnFloatEvent(float f)
    {
        Debug.Log("Float: " + f);
    }

    private void Testing_OnSpacePressed(object sender, OnF5PressedEventArgs e) // defining the event function - whatever is supposed to happen when said event is called.
    {
        Debug.Log("F5 pressed " + e.F5Count + " times");
        if (e.F5Count >= 5)
        {
            Debug.Log("You've pressed F5 five times. No F5's left!");
            testEventScript.OnF5Pressed -= Testing_OnSpacePressed; // Unsubscribing to the publisher script's event. F5 should only work five times!
        }

    }
}
