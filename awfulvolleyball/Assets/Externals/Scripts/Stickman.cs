using UnityEngine;
using Fusion;
public class Stickman : MonoBehaviour
{
	public float horDir = 0;
	public float vertDir = 0;
	public bool jumpInput = false;

	[Header("Traits")]
	public float moveSpeed;
    public float groundDrag;
	public float jumpForce;
	public float dashForce;
	public float dashCooldownTimer = 2.5f;
    public float airMultiplier;
	public float baseSpeed;
	public float sprintSpeed;
	public float baseHeight;
	public float crouchHeight;

	[Header("Hover Traits")]
	public LayerMask hoverableLayers;
	public float RideHeight = 1.2f;
	public float RideSpringDamper = 1.0f;
	public float RideSpringStrength = 1.0f;
    public float GroundedBuffer = 0.1f;
    public float jumpStartTimer = 0.2f;
	public bool CanJump = true;

    bool readyToJump;

	[Header("Dependencies")]
	public Rigidbody rb;
	public PlayerObject PlayerObj { get; private set; }


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
			float myMag = rb.velocity.magnitude;

			float combinedMag = incMag + myMag;
			float normalized = Mathf.Clamp01(combinedMag / 20f);
			float bounceForce = Mathf.Lerp(minForce, maxForce, normalized);

    		Vector3 direction = (ball.transform.position - rb.transform.position).normalized;
			ball.rb.velocity = Vector3.zero;
			ball.rb.AddForce(direction * bounceForce, ForceMode.Impulse);
			Debug.Log("Hit Ball");
	}

    void Update()
    {
		horDir = Input.GetAxis("Horizontal");
		vertDir = Input.GetAxis("Vertical");
		jumpInput = Input.GetKey(KeyCode.Space);
		RideHeight = baseHeight;
    }


    void FixedUpdate()
	{
		Vector3 flatForward = new Vector3(rb.transform.forward.x, 0f, rb.transform.forward.z).normalized;
		Vector3 flatRight = new Vector3(rb.transform.right.x, 0f, rb.transform.right.z).normalized;

		Vector3 moveDir = flatForward * vertDir + flatRight * horDir;
		if (CanJump) {
			MovePlayer(moveDir, jumpInput);
		}
		SpeedControl();
		prevVelocity = rb.velocity;
		isFirstUpdate = false;
	}

	private void MovePlayer(Vector3 moveDirection, bool tryingToJump)
    {
        // on ground
        if(IsGrounded()) {
			rb.drag = groundDrag;
			if (tryingToJump && CanJump) {
				rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
       		 	rb.AddForce(rb.transform.up * jumpForce, ForceMode.Impulse);
				// JumpTimer = TickTimer.CreateFromSeconds(Runner, jumpStartTimer);
			}
			else {
				rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
				HoverPlayer();
			}
		}
        // in air
        else {
			rb.drag = 0;
            rb.AddForce(moveDirection.normalized  * moveSpeed * 10f * airMultiplier, ForceMode.Force);
			HoverPlayer();
		}

	}


	private void HoverPlayer() {
		RaycastHit hit;
		Vector3 rayOrigin = rb.transform.position;
		Vector3 rayDir = Vector3.down;
		float rayDistance = (RideHeight + (GroundedBuffer*2))*2;

		bool _rayDidHit = Physics.Raycast(rayOrigin, rayDir, out hit, rayDistance, hoverableLayers);
		Debug.DrawLine(rb.transform.position, rb.transform.position + (rayDir * rayDistance));
		if (_rayDidHit) {
			Vector3 vel = rb.velocity;
			Vector3 otherVel = Vector3.zero;

			float rayDirVel = Vector3.Dot(rayDir, vel);
			// TODO(zack): implement
			float otherDirVel = Vector3.Dot(rayDir, otherVel);


			float relVel = rayDirVel - otherDirVel;

			float x = hit.distance - RideHeight;

			float springForce = (x* RideSpringStrength) - (relVel * RideSpringDamper);
			rb.AddForce(rayDir * springForce);

		}
	}


	private void SpeedControl() {
		Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
	}

	bool IsGrounded()
	{
		RaycastHit hit;
		Vector3 rayOrigin = transform.position;
		Vector3 rayDir = Vector3.down;
		float rayDistance = RideHeight + GroundedBuffer;

		bool rayDidHit = Physics.Raycast(rayOrigin, rayDir, out hit, rayDistance, hoverableLayers);
		return rayDidHit;
	}

}
