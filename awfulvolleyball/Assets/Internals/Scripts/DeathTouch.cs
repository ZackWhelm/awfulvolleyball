using UnityEngine;

public class DeathTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance != null) {
            if (!GameManager.Instance.Runner.IsServer) return;
        }
        
        if (other.TryGetComponent(out StickmanRigidbody rb))
        {
            rb.stickman.HandleCritalHit();
        }
    }
}
