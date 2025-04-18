using UnityEngine;

public class DevTester : MonoBehaviour
{
    public static DevTester Instance { get; private set; }

    [Header("Dependencies")]
    public GameObject ballPrefab;
    public Transform playerTransform;

    [Header("Traits")]
    public float pushForce = 10f;
    /*
    // how to find ball: GameObject.FindWithTag("Ball")
Press E to move ball forward from direction of player. 
Press Q to spawn ball on top of player. 
    */


    void Update() {
        if (!GlobalSettings.Instance.IsMultiplayer) {
            HandleTestInput();  
        }

    }

    private void HandleTestInput() {
        if (Input.GetKeyDown(KeyCode.E)) {
            GameObject ball = GameObject.FindWithTag("Ball");
            if (ball != null) {
                Rigidbody rb = ball.GetComponent<Rigidbody>();
                if (rb != null) {
                    rb.velocity = transform.forward * pushForce;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            if (!ProgressionManager.Instance.CheckIfBallExists()) {
                Vector3 spawnPos = playerTransform.position + Vector3.up * 2f;
                Instantiate(ballPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
