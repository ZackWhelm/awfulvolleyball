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

public void ResetPlayer(Stick stick)
{
    float angle = Random.Range(0f, Mathf.PI * 2);

    float xOffset = Mathf.Cos(angle) * CheckpointRadius;
    float zOffset = Mathf.Sin(angle) * CheckpointRadius;

    Vector3 centerPos = LastCheckpoint.transform.position;
    float yPos = centerPos.y + stickmanSingularRef.MoveHandler.baseHeight;

    Vector3 targPos = new Vector3(centerPos.x + xOffset, yPos, centerPos.z + zOffset);

    stickmanSingularRef.TeleportToAndFace(targPos, centerPos);
}
}
