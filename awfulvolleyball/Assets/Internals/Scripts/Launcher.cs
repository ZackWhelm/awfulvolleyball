using UnityEngine;

public class Launcher : MonoBehaviour
{
    [Header("Traits")]
	public float launchForce = 1.75f;
    public Vector3 launchDirection = Vector3.up;

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance != null) {
            if (!GameManager.Instance.Runner.IsServer) return;
        }
        if (other.TryGetComponent(out Ball ball))
        {

            ball.rb.AddForce(launchDirection.normalized * launchForce, ForceMode.Impulse);
        }
    }

}
