using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("Dependencies")]
	public Checkpoint checkpoint = null;

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance != null) {
            if (!GameManager.Instance.Runner.IsServer) return;
        }
        Debug.Log("In Here");

        if (other.TryGetComponent(out StickmanRigidbody rb))
        {
            checkpoint.HandleEnter(rb.stickman);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (GameManager.Instance != null) {
            if (!GameManager.Instance.Runner.IsServer) return;
        }
        
        if (other.TryGetComponent(out StickmanRigidbody rb))
        {
            checkpoint.HandleExit(rb.stickman);
        }
    }
}
