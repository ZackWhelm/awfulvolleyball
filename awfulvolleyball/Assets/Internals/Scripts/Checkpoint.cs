using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Float")]
    [SerializeField] private bool unlocked = false;
    [SerializeField] private float _playersIn; 
    private Dictionary<Stickman, bool> stickmenIn = new Dictionary<Stickman, bool>();

    public float PlayersIn
    {
        get { return _playersIn; }
        set { 
            _playersIn = value;
        }
    }

    public void HandleEnter(Stickman stick) {
        if (!unlocked) {
            Unlock();
        }
        if (!stickmenIn.ContainsKey(stick)) {
            stickmenIn[stick] = true;
            PlayersIn = stickmenIn.Count;
            if (!ProgressionManager.Instance.CheckIfBallExists()) {
                Debug.Log(GlobalSettings.Instance.PlayerCount + " vs " + PlayersIn);
                if (GlobalSettings.Instance.PlayerCount == PlayersIn) {
                    ProgressionManager.Instance.SpawnBallAboveThis(transform.position);
                }
            }
        }
    }

    public void HandleExit(Stickman stick) {
        if (stickmenIn.ContainsKey(stick)) {
            stickmenIn.Remove(stick);
            PlayersIn = stickmenIn.Count;
        }
    }

    public void Unlock() {
        unlocked = true;
        ProgressionManager.Instance.SetLastCheckPoint(this);
    }
}
