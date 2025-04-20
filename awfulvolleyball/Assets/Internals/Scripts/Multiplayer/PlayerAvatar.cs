using UnityEngine;
using Fusion;
using Cinemachine;

public class PlayerAvatar: NetworkBehaviour, ICanControlCamera
{
	public PlayerObject PlayerObj { get; private set; }
	[Header("Dependencies")]
	public Rigidbody rb;

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
	public float RideHeight = 1.2f;
	public float RideSpringDamper = 1.0f;
	public float RideSpringStrength = 1.0f;
    public float GroundedBuffer = 0.1f;
	public float PlayerSpeed = 1.2f;

	[Header("Spped Traits")]
	public float SprintSpeed;
	public float JogSpeed;
	public float CrouchSpeed;
	public float GroundDrag;
	public float JumpForce;
	public float BaseHeight;
	public float CrouchHeight;
	public float SprintHeight;

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
		MovePlayer(moveDir);
	}

	private void MovePlayer(Vector3 moveDirection)
    {
		rb.AddForce(moveDirection.normalized * 2f, ForceMode.Force);
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

	private void TryHoverPlayer() {
		RaycastHit hit;
		Vector3 rayOrigin = rb.transform.position;
		Vector3 rayDir = Vector3.down;
		float rayDistance = RideHeight + GroundedBuffer;

		bool _rayDidHit = Physics.Raycast(rayOrigin, rayDir, out hit, rayDistance, hoverableLayers);
				
		if (_rayDidHit) {
			Vector3 velocity = rb.velocity;
			velocity.y = 0f;
			rb.velocity = velocity;
			rb.transform.position = new Vector3(transform.position.x, RideHeight, transform.position.z);
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
		RideHeight = BaseHeight;
		IsCrouching = input.crouchInput;
		IsSprinting = input.sprintInput;

		if (IsCrouching) {
			RideHeight = CrouchHeight;
		}
		if (IsSprinting) {
			IsCrouching = false;
			RideHeight = SprintHeight;
			PlayerSpeed = SprintSpeed;
		}
		else {
			PlayerSpeed = JogSpeed;
		}
	}

	private bool IsGrounded()
	{
		RaycastHit hit;
		Vector3 rayOrigin = rb.transform.position;
		Vector3 rayDir = Vector3.down;
		float rayDistance = RideHeight + GroundedBuffer;
		bool rayDidHit = Physics.Raycast(rayOrigin, rayDir, out hit, rayDistance, hoverableLayers);
		Debug.Log("is grounded:" + rayDidHit);

		if (IsJumping) {
			if (rb.velocity.y < 0.0f ) {
				if (rayDidHit) {
					IsJumping = false;
				}
				return rayDidHit;
			} 
			return false;
		}
		return rayDidHit;
	}
}