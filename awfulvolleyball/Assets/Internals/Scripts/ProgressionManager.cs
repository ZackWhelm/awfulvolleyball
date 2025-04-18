using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }

    [Header("GameSetup")]
    public float ballSpawnOffset; 
    public Stickman stickmanSingularRef; 

    [Header("Dependencies")]
    public GameObject ballPrefabs; 
    public DoorPanel doorPanel; 

    [Header("Traits")]
    public Checkpoint LastCheckpoint;
    public float CheckpointRadius;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void SetLastCheckPoint(Checkpoint checkpoint) {
        LastCheckpoint = checkpoint;
    }

    public bool CheckIfBallExists()
    {
        return GameObject.FindWithTag("Ball") != null;
    }

    public void SpawnBallAboveThis(Vector3 pos) {
        if (!GlobalSettings.Instance.IsMultiplayer) {
            if (ballPrefabs != null) {
                Vector3 spawnPosition = pos + Vector3.up * ballSpawnOffset;
                Instantiate(ballPrefabs, spawnPosition, Quaternion.identity);
            }
        }
    }

    public void ResetPlayers() {
        if (!GlobalSettings.Instance.IsMultiplayer) {
            Vector3 targPos = new Vector3(LastCheckpoint.transform.position.x, LastCheckpoint.transform.position.y + stickmanSingularRef.MoveHandler.baseHeight, LastCheckpoint.transform.position.z - CheckpointRadius);
            stickmanSingularRef.TeleportToAndFace(targPos, LastCheckpoint.transform.position);
        }
    }
}
