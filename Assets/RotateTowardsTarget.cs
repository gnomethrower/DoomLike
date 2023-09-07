using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RotateTowardsTarget : MonoBehaviour
{
    /* Sources
    https://www.youtube.com/watch?v=rKGsELBgpQY
    https://gamedevbeginner.com/how-to-rotate-in-unity-complete-beginners-guide
    https://gist.github.com/maxattack/4c7b4de00f5c1b95a33b
    */

    private RotationType rotationType;
    public enum RotationType { fixedRotation, linear, smooth }

    [SerializeField] private Transform lookTargetTransform;
    private Vector3 direction;

    private Vector3 newRotation;

    private float xVel;
    private float yVel;
    private float zVel;



    public void RotateTowardsConstantly(Transform target)
    {
        direction = target.position - transform.position;
        transform.eulerAngles = newRotation;

    }

    public void RotateTowardsLinearly(Transform target, RotationType type, float linearSpeed = 50f)
    {
        direction = target.position - transform.position;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), linearSpeed * Time.deltaTime);
        transform.eulerAngles = newRotation;
    }

    public void RotateTowardsSmoothly(Transform _target, float _smoothSpeed = .5f, bool _isLockedToYAxis = false)
    {
        direction = _target.position - transform.position;
        Vector3 goalRotation = Quaternion.LookRotation(direction).eulerAngles;

        if (!_isLockedToYAxis)
        {
            newRotation = new Vector3(
            Mathf.SmoothDampAngle(newRotation.x, goalRotation.x, ref xVel, _smoothSpeed),
            Mathf.SmoothDampAngle(newRotation.y, goalRotation.y, ref yVel, _smoothSpeed),
            Mathf.SmoothDampAngle(newRotation.z, goalRotation.z, ref zVel, _smoothSpeed)
            );
        }
        else
        {
            newRotation = new Vector3(
            transform.eulerAngles.x,
            Mathf.SmoothDampAngle(newRotation.y, goalRotation.y, ref yVel, _smoothSpeed),
            transform.eulerAngles.z
            );
        }
        transform.eulerAngles = newRotation;
    }
}
