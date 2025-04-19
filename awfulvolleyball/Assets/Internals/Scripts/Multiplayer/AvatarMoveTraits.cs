using UnityEngine;
using Fusion;
using Cinemachine;

public class AvatarMoveTraits: MonoBehaviour
{
	[Header("Speed Traits")]
	public float SprintSpeed;
	public float JogSpeed;
	public float CrouchSpeed;

	[Header("General Traits")]
    public float groundDrag;
	public float jumpForce;
	public float BaseHeight;
	public float CrouchHeight;
	public float SprintHeight;
}