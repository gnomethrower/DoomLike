using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceNotes : MonoBehaviour
{

    /* 
    Another way on how to use Interfaces in unity is to define an Abstract Scriptable Object that implements the functionality of the Interface.
    The clases that implement the interface then have a Serialize field where you can drag in the concrete implementation of the scriptable object. 
    And you implement the interface by calling the function on the scriptable object.
    I even got so far to make a propertyAttribute that uses reflection to show all classes that inherit from the abstract class in a dropdown and automatically creates a scriptable object for me.
    You can go really complex with that and you can easily switch between different behaviours just by using a dropdown menu.

    And the best? if you want to add a new function, all you have to do is to create a class that inherits from the abstract scriptable object.Done.
    */

}
