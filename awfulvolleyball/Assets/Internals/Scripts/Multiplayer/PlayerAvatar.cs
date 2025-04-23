using UnityEngine;
using Fusion;
using Cinemachine;

public class PlayerAvatar: NetworkBehaviour, ICanControlCamera
{
	public PlayerObject PlayerObj { get; private set; }
	[Header("Dependencies")]
	public Rigidbody rb;
	public CapsuleCollider capsule;
	public Transform sphereTransform;

	[Networked]
	public TickTimer JumpTimer { get; set; }
	public bool CanJump => JumpTimer.ExpiredOrNotRunning(Runner);

	[Networked]
	PlayerInput CurrInput { get; set; }
	public bool IsJumping = false;

	public bool IsCrouching = false;
	public bool IsSprinting = false;


	[Header("Hover Variables")]
	public LayerMask hoverableLayers;
    public float GroundedBuffer = 0.1f;
	public float PlayerSpeed = 1.2f;

	[Header("Spped Traits")]
	public float SprintSpeed;
	public float JogSpeed;
	public float CrouchSpeed;
	public float GroundDrag;
	public float JumpForce;
	public float JumpTimerStartTime = 0.25f;
	public float BaseHeight;
	public float CrouchHeight;
	public float SprintHeight;


	private float _avatarHeight;
	public float AvatarHeight
	{
		get { return _avatarHeight; }
		set { 
			_avatarHeight = value; 
			SetHeight();
		}
	}

	void Update()
	{
		if (Object.HasInputAuthority)
		{
			InputAuthorityUpdate(CurrInput);
		}
	}

	public override void Spawned()
	{
		PlayerObj = PlayerRegistry.GetPlayer(Object.InputAuthority);
		PlayerObj.Controller = this;
		
		if (Object.HasInputAuthority)
		{
			Debug.Log("HasInputAuthority" + gameObject.name  + PlayerObj.Index);
			CameraController.AssignControl(this);
		}
		else
		{
			Debug.Log("Does not have InputAuthority" + gameObject.name + PlayerObj.Index);
			Instantiate(ResourcesManager.Instance.worldNicknamePrefab, InterfaceManager.Instance.worldCanvas.transform).SetTarget(this);
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
		if (Runner.IsForward) {
			HandleInput(CurrInput);
		}
	}

	public void HandleInput(PlayerInput input) {
		Vector3 newForward = new Vector3(input.forwardDir.x, 0.0f, input.forwardDir.y);
		Vector3 newRight = new Vector3(input.rightDir.x, 0.0f, input.rightDir.y);
		Vector3 moveDir = newForward * input.vertDir + newRight * input.horDir;
		HeightAndSpeedUpdates();
		MovePlayer(moveDir, input.jumpInput);
	}

	private void MovePlayer(Vector3 moveDirection, bool hasJumpInput)
    {
		rb.AddForce(moveDirection.normalized * 2f, ForceMode.Force);
		if (hasJumpInput) {
			if (CanJump && IsGrounded()) {
				Debug.Log("Tried Jump and succeeded");
				rb.AddForce(Vector3.up * JumpForce * 50f, ForceMode.Force);
				JumpTimer = TickTimer.CreateFromSeconds(Runner, JumpTimerStartTime);
			}
			else {
				Debug.Log("Tried Jump and failed");
			}
		}
		
		LimitSpeed();
	}

	public void SetLook(ref CinemachineFreeLook look)
	{
		if (Object.HasInputAuthority)
		{
			look.LookAt = rb.transform;
			look.Follow = rb.transform;
		}
	}

	private void LimitSpeed() {
		Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(flatVel.magnitude > PlayerSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * PlayerSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
	}

    private void InputAuthorityUpdate(PlayerInput input)
	{
		IsCrouching = input.crouchInput;
		IsSprinting = input.sprintInput;


	}

	private bool IsGrounded()
	{
		RaycastHit hit;
		Vector3 rayOrigin = rb.transform.position;
		Vector3 rayDir = Vector3.down;
		float rayDistance = AvatarHeight + GroundedBuffer;
		bool rayDidHit = Physics.Raycast(rayOrigin, rayDir, out hit, rayDistance, hoverableLayers);
		Debug.Log("Grounded" + rayDidHit);
		return rayDidHit;
	}

	private void HeightAndSpeedUpdates() {
		if (IsCrouching) {
			AvatarHeight = CrouchHeight;
			if (!IsSprinting) {
				return;
			}
		}
		if (IsSprinting) {
			AvatarHeight = SprintHeight;
			PlayerSpeed = SprintSpeed;
			return;
		}
		else {
			PlayerSpeed = JogSpeed;
		}
	}

	private void SetHeight()
	{
		capsule.height = AvatarHeight;
		if (sphereTransform == null) {
			return;
		}
		
		float capsuleHeightWithoutRounds = capsule.height - (2*capsule.radius);
		float circleHeight = capsuleHeightWithoutRounds / 2;
		sphereTransform.localPosition = new Vector3(0.0f, circleHeight, 0.0f);
	}
}