using UnityEngine;
using Fusion;

public class Ball : NetworkBehaviour 
{
    [Header("Dependencies")]
    public Rigidbody rb;

    [Header("Traits")]
    public float GravMultiply = 0.1f;

    void Start()
    {
        rb.useGravity = false;
    }

    public override void FixedUpdateNetwork()
	{
		if (Runner.IsForward)
		{
            rb.AddForce(Physics.gravity * GravMultiply, ForceMode.Acceleration);
        }
	}

    void FixedUpdate() {
        if (GameManager.Instance != null) {
            return;
        }
        rb.AddForce(Physics.gravity * GravMultiply, ForceMode.Acceleration);
    }

    public void HandleThroughRing(Ring ring) {
        Debug.Log("Ball Go Through: " + ring.name);
    }
}