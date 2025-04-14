using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Adventurer : NetworkBehaviour, ICanControlCamera
{
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

    bool readyToJump;

	[Header("Dependencies")]
	public Transform interpolationTarget;
	public MeshRenderer ren;
	public Rigidbody rb;
	new public SphereCollider collider;
	public PlayerObject PlayerObj { get; private set; }

	[Networked]
	public TickTimer JumpTimer { get; set; }
	public bool CanJump => JumpTimer.ExpiredOrNotRunning(Runner);

	[Networked]
	public TickTimer DashTimer { get; set; }
	public bool CanDash => DashTimer.ExpiredOrNotRunning(Runner);

	[Networked]
	public float StillBounceStrength { get; set; }
	public float JumpBounceStrength { get; set; }
	public float BounceStrength { get; set; }


	[Networked]
	PlayerInput CurrInput { get; set; }
	PlayerInput prevInput = default;

	[Header("Internals")]
	[SerializeField] private Vector3 prevVelocity = Vector3.zero;

	bool isFirstUpdate = true;

	void Update()
	{
		if (Object.HasInputAuthority)
		{
			transform.rotation = CameraController.Instance.transform.rotation;
			if (CurrInput.crouchInput) {
				RideHeight = crouchHeight;
			}
			else {
				RideHeight = baseHeight;
			}

			if (CurrInput.sprintInput) {
				moveSpeed = sprintSpeed;
			}
			else {
				moveSpeed = baseSpeed;
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
        if (!GameManager.Instance.Runner.IsServer) return;

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

    		Vector3 direction = (ball.transform.position - transform.position).normalized;
			ball.rb.velocity = Vector3.zero;
			ball.rb.AddForce(direction * bounceForce, ForceMode.Impulse);
			Debug.Log("Hit Ball");
	}

	public override void Spawned()
	{
		PlayerObj = PlayerRegistry.GetPlayer(Object.InputAuthority);
		PlayerObj.Controller = this;
        rb.freezeRotation = true;
        readyToJump = true;

		ren.material.color = PlayerObj.Color;

		if (Object.HasInputAuthority)
		{
			CameraController.AssignControl(this);
		}
		else
		{
			Instantiate(ResourcesManager.Instance.worldNicknamePrefab, InterfaceManager.Instance.worldCanvas.transform).SetTarget(this);
		}
	}

	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		if (CameraController.HasControl(this))
		{
			CameraController.AssignControl(null);
		}

		if (!runner.IsShutdown)
		{
			if (PlayerObj.TimeTaken != PlayerObject.TIME_UNSET)
			{
				AudioManager.Play("ballInHoleSFX", AudioManager.MixerTarget.SFX, interpolationTarget.position);
			}
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (GetInput(out PlayerInput input))
		{
			CurrInput = input;
		}

		if (Runner.IsForward)
		{
			Vector3 flatForward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
			Vector3 flatRight = new Vector3(transform.right.x, 0f, transform.right.z).normalized;

			Vector3 moveDir = flatForward * input.vertDir + flatRight * input.horDir;
			if (CanJump) {
				MovePlayer(moveDir, input.jumpInput);
			}
			if (CanDash) {
				TryDash(moveDir, input.dashInput);
			}
			prevInput = CurrInput;
			SpeedControl();
			prevVelocity = rb.velocity;
			isFirstUpdate = false;
		}
	}

	private void TryDash(Vector3 moveDirection, bool tryingToDash) {
		if (tryingToDash && CanDash) {
			Debug.Log("Dashing");
			rb.AddForce(transform.forward  * dashForce, ForceMode.Impulse);
			DashTimer = TickTimer.CreateFromSeconds(Runner, dashCooldownTimer);
		}
	}

	private void MovePlayer(Vector3 moveDirection, bool tryingToJump)
    {
        // on ground
        if(IsGrounded()) {
			rb.drag = groundDrag;
			if (tryingToJump && CanJump) {
				rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
       		 	rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
				JumpTimer = TickTimer.CreateFromSeconds(Runner, jumpStartTimer);
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
		Vector3 rayOrigin = transform.position;
		Vector3 rayDir = Vector3.down;
		float rayDistance = (RideHeight + (GroundedBuffer*2))*2;

		bool _rayDidHit = Physics.Raycast(rayOrigin, rayDir, out hit, rayDistance, hoverableLayers);
		Debug.DrawLine(transform.position, transform.position + (rayDir * rayDistance));
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

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void Rpc_Respawn(bool effect)
	{
		if (effect) Instantiate(ResourcesManager.Instance.splashEffect, transform.position, ResourcesManager.Instance.splashEffect.transform.rotation);
		if (Object.HasInputAuthority) CameraController.Recenter();

		rb.velocity = rb.angularVelocity = Vector3.zero;
		rb.MovePosition(Level.Current.GetSpawnPosition(PlayerObj.Index));
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
	
	public Vector3 Position => interpolationTarget.position;
	
	public Quaternion Rotation
	{
		get => interpolationTarget.rotation;
		set => interpolationTarget.rotation = value;
	}

	public void ControlCamera(ref float pitch, ref float yaw)
	{
		if (Object.HasInputAuthority)
		{
			pitch -= Input.GetAxis("Mouse Y");
			yaw += Input.GetAxis("Mouse X");
		}
	}
}
