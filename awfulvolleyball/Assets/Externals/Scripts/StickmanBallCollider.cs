using UnityEngine;
using Fusion;
public class StickmanBallCollider : MonoBehaviour
{
	[Header("Speed Traits")]
	public float minForce;
	public float maxForce;

	[Header("Dependencies")]
	public Rigidbody mainRB;
	public Transform headTransform;


	public void HandleBallBounce(Ball ball) {
		float minForce = 0.25f;
		float maxForce = 1.6f;
		
		float incMag = ball.rb.velocity.magnitude;
		float myMag = mainRB.velocity.magnitude;

		float combinedMag = incMag + myMag;
		float normalized = Mathf.Clamp01(combinedMag / 20f);
		float bounceForce = Mathf.Lerp(minForce, maxForce, normalized);

		Vector3 direction = (ball.transform.position - headTransform.position).normalized;
		ball.rb.velocity = Vector3.zero;
		ball.rb.AddForce(direction * bounceForce, ForceMode.Impulse);
	}

	void LateUpdate()
	{
		if (Camera.main != null)
		{
			Vector3 camForward = Camera.main.transform.forward;
			camForward.y = 0; // Ignore vertical tilt
			transform.forward = camForward.normalized;
		}
	}
}
