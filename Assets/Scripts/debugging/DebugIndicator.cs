using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugIndicator : MonoBehaviour
{
   public static void DrawDebugIndicator(Vector3 position, Color color, float duration = 10f, float size = 1f)
    {
        float halfSize = size * 0.5f;

        Vector3 xStart = position + Vector3.left * halfSize;
        Vector3 xEnd = position + Vector3.right * halfSize;

        Vector3 yStart = position + Vector3.forward * halfSize;
        Vector3 yEnd = position + Vector3.back * halfSize;

        Vector3 zStart = position + Vector3.down * halfSize;
        Vector3 zEnd = position + Vector3.up * halfSize;

        Debug.DrawLine(xStart, xEnd, color, duration);
        Debug.DrawLine(yStart, yEnd, color, duration);
        Debug.DrawLine(zStart, zEnd, color, duration);
    }
}

