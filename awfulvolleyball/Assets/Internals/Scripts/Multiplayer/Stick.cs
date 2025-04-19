using UnityEngine;
using Fusion;
using Cinemachine;

public class Stick: NetworkBehaviour, ICanControlCamera
{
    [Header("Dependencies")]
	public StickMoveHandler MoveHandler;
	public PlayerObject PlayerObj { get; private set; }
	public Rigidbody bodyRigidBody;

	[Networked]
	public TickTimer JumpTimer { get; set; }
	public bool CanJump => JumpTimer.ExpiredOrNotRunning(Runner);

	[Networked]
	PlayerInput CurrInput { get; set; }


	void Update()
	{
		if (Object.HasInputAuthority)
		{
			MoveHandler.InputAuthorityUpdate(CurrInput);
		}
	}

	public override void Spawned()
	{
		PlayerObj = PlayerRegistry.GetPlayer(Object.InputAuthority);
		// PlayerObj.Controller = this;
		
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
	}

	public override void FixedUpdateNetwork() {
		if (GetInput(out PlayerInput input))
		{
			CurrInput = input;
		}
		if (Runner.IsForward)
		{
			MoveHandler.HandleInput(CurrInput.horDir, CurrInput.vertDir, CurrInput.jumpInput, CurrInput.crouchInput, CurrInput.sprintInput);
		}
	}


	public void SetLook(ref CinemachineFreeLook look)
	{
		if (Object.HasInputAuthority)
		{
			look.LookAt = bodyRigidBody.transform;
			look.Follow = bodyRigidBody.transform;
			MoveHandler.cam = Camera.main;
		}
	}

	public void TeleportToAndFace(Vector3 pos, Vector3 _lookAt) {
		bodyRigidBody.velocity = Vector3.zero;
		bodyRigidBody.angularVelocity = Vector3.zero;
		bodyRigidBody.gameObject.transform.position = pos;
	}
	
	public void HandleCritalHit() {
		// TODO(zack: add anim)
		OnDeath();
	}


	void OnDeath() {
		ProgressionManager.Instance.ResetPlayer(this);
	}
}