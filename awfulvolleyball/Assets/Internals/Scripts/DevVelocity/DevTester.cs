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
                    Vector3 directionTrue = (ball.transform.position - playerTransform.position);
                    directionTrue = new Vector3(directionTrue.x, 0.0f, directionTrue.z);
                    Vector3 direction = directionTrue.normalized;
                    direction += new Vector3(0.0f, 0.1f, 0.0f);
                    direction.Normalize();
                    rb.velocity = direction * pushForce;
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
