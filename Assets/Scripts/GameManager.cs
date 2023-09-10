using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager current;

    public static object OnPlayerDeath { get; internal set; }

    private void Awake()
    {
        current = this;
    }



}
