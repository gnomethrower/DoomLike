using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface Manager taken from 'Learning C# by Developing Games with Unity' p.295.

// 1
public interface IManager
{
    // 2 String Variable named State with accessors named get and set
    string State { get; set; }

    // 3
    public void Initialize();

}
