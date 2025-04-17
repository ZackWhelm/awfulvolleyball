using UnityEngine;
using Fusion;
using System.Collections;

public class Ball : NetworkBehaviour 
{
    [Header("Dependencies")]
    public Rigidbody rb;
    public MeshRenderer renderer;

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

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            StartCoroutine(HideAndDestroy());
        }

        Debug.Log(collision.gameObject.name);

        if (collision.gameObject.TryGetComponent(out StickmanBallCollider stickBallColider))
        {
			stickBallColider.HandleBallBounce(this);
		}
    }

    private IEnumerator HideAndDestroy()
    {
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        yield return new WaitForSeconds(1f);
        ProgressionManager.Instance.ResetPlayers();
        Destroy(this.gameObject);
    }




}