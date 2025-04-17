using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [Header("Traits")]
    public bool IsMultiplayer = false;
    public int PlayerCount = 1;

    public static GlobalSettings Instance { get; private set; }

    void Awake()
    {
        Instance = this;   
    }
}
