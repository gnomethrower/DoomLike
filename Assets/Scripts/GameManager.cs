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
