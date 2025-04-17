using UnityEngine;
using Fusion;
public class StickmanMovementHandler : MonoBehaviour
{
	[Header("Speed Traits")]
	public float SprintSpeed;
	public float JogSpeed;

	[Header("Traits")]
	public float playerSpeed;
    public float groundDrag;
	public float jumpForce;
    public float airMultiplier;
	public float baseHeight;
	public float crouchHeight;
	public bool IsCrouching = true;
	public bool IsSprinting = true;
	private bool _isJumping = false;

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
	public Camera cam;
	public StickmanAnimationHandler AnimHandler;
	public Transform AnimTransform;

	bool isFirstUpdate = true;

	public bool IsJumping
	{
		get { return _isJumping; }
		set { 
			if (value) {
				AnimHandler.StartJump();
				CanJump = false;
			}
			_isJumping = value; 
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
	}


	public void HandleInput(float horizontal, float vertical, bool spaceInput, bool crouchInput, bool sprintInput) {
		Vector3 camForward = cam.transform.forward;
		Vector3 camRight = cam.transform.right;

		Vector3 flatForward = new Vector3(camForward.x, 0f, camForward.z).normalized;
		Vector3 flatRight = new Vector3(camRight.x, 0f, camRight.z).normalized;

		Vector3 moveDir = flatForward * vertical + flatRight * horizontal;
		MovePlayer(moveDir, spaceInput);
		SpeedControl();
		IsSprinting = sprintInput;
		AnimHandler.UpdateAnimationDirection(moveDir);
		AnimHandler.UpdateAnimPosition(rb.transform.position);
	}

	private void MovePlayer(Vector3 moveDirection, bool tryingToJump)
    {
        // on ground
        if(IsGrounded()) {
			rb.drag = groundDrag;
			if (tryingToJump && CanJump) {
				IsJumping = true;
				rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
       		 	rb.AddForce(rb.transform.up * jumpForce, ForceMode.Impulse);
				// JumpTimer = TickTimer.CreateFromSeconds(Runner, jumpStartTimer);
			}
			else {
				rb.AddForce(moveDirection.normalized * playerSpeed * 10f, ForceMode.Force);
				HoverPlayer();
			}
		}
        // in air
        else {
			rb.drag = 0;
            rb.AddForce(moveDirection.normalized  * playerSpeed * 10f * airMultiplier, ForceMode.Force);
		}

	}

    void Update()
    {
		RideHeight = baseHeight;
        if (IsCrouching) {
			RideHeight = crouchHeight;
		}
		if (IsSprinting) {
			RideHeight = crouchHeight;
			playerSpeed = SprintSpeed;
		}
		else {
			playerSpeed = JogSpeed;
		}
		if (AnimTransform == null) {
			return;
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
        if(flatVel.magnitude > playerSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * playerSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
	}

	bool IsGrounded()
	{
		RaycastHit hit;
		Vector3 rayOrigin = rb.transform.position;
		Vector3 rayDir = Vector3.down;
		float rayDistance = RideHeight + GroundedBuffer;
		bool rayDidHit = Physics.Raycast(rayOrigin, rayDir, out hit, rayDistance, hoverableLayers);

		if (IsJumping) {
			if ( rb.velocity.y < 0.0f ) {
				if (rayDidHit) {
					IsJumping = false;
					CanJump = true;
				}
				return rayDidHit;
			}
			return false;
		}
		return rayDidHit;
	}

}
