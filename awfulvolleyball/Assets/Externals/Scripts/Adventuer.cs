using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;
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

	public override void Spawned()
	{
		PlayerObj = PlayerRegistry.GetPlayer(Object.InputAuthority);
		//PlayerObj.Controller = this;
        rb.freezeRotation = true;
        readyToJump = true;

		ren.material.color = PlayerObj.Color;

		if (Object.HasInputAuthority)
		{
			CameraController.AssignControl(this);
		}
		else
		{
			// Instantiate(ResourcesManager.Instance.worldNicknamePrefab, InterfaceManager.Instance.worldCanvas.transform).SetTarget(this);
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

	/*
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
	*/


	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void Rpc_Respawn(bool effect)
	{
		if (effect) Instantiate(ResourcesManager.Instance.splashEffect, transform.position, ResourcesManager.Instance.splashEffect.transform.rotation);
		// if (Object.HasInputAuthority) CameraController.Recenter();

		rb.velocity = rb.angularVelocity = Vector3.zero;
		rb.MovePosition(Level.Current.GetSpawnPosition(PlayerObj.Index));
	}

	public void SetLook(ref CinemachineFreeLook look)
	{
		if (Object.HasInputAuthority)
		{
			
		}
	}
}
