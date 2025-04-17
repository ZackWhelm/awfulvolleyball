using UnityEngine;

public class ProgressionManagerRepo : MonoBehaviour
{
    public static ProgressionManagerRepo Instance { get; private set; }

    [Header("Dependencies")]
    public DoorPanel tutorialDoorPanel; 
    public bool[] ringsPassedThrough; 

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void PassedRingOne() {
        ringsPassedThrough[0] = true;
        tutorialDoorPanel.Open();   
    }
}
