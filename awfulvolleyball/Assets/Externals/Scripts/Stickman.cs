using UnityEngine;
using Fusion;
public class Stickman : MonoBehaviour
{
	[Header("Dependencies")]
	public StickmanMovementHandler MoveHandler;
	public PlayerObject PlayerObj { get; private set; }
	public Rigidbody headRigidbody;

	[Header("Inputs")]
	public float horDir = 0;
	public float vertDir = 0;
	public bool jumpInput = false;
	public bool crouchInput = false;
	public bool sprintInput = false;

    bool readyToJump;


	[Header("Internals")]
	[SerializeField] private Vector3 prevVelocity = Vector3.zero;

	bool isFirstUpdate = true;

	private void OnCollisionEnter(Collision collision)
	{
        if (collision.gameObject.TryGetComponent(out Ball ball))
        {
            HandleBallBounce(ball);
        }
	}


	private void HandleBallBounce(Ball ball) {
		float minForce = 0.25f;
		float maxForce = 1.6f;
		float incMag = ball.rb.velocity.magnitude;
		float myMag = headRigidbody.velocity.magnitude;

		float combinedMag = incMag + myMag;
		float normalized = Mathf.Clamp01(combinedMag / 20f);
		float bounceForce = Mathf.Lerp(minForce, maxForce, normalized);

		Vector3 direction = (ball.transform.position - headRigidbody.transform.position).normalized;
		ball.rb.velocity = Vector3.zero;
		ball.rb.AddForce(direction * bounceForce, ForceMode.Impulse);
	}

    void Update()
    {
		horDir = Input.GetAxis("Horizontal");
		vertDir = Input.GetAxis("Vertical");
		jumpInput = Input.GetKey(KeyCode.Space);
		sprintInput = Input.GetKey(KeyCode.LeftShift);
		crouchInput = Input.GetKey(KeyCode.LeftControl);
    }


    void FixedUpdate()
	{
		MoveHandler.HandleInput(horDir, vertDir, jumpInput, crouchInput, sprintInput);
	}

}
