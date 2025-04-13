using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Helpers.Math;

public class CameraController : MonoBehaviour
{
	[field: SerializeField] public ShakeBehaviour Shake { get; private set; }
	
	ICanControlCamera con = null;
	public static ICanControlCamera Controller => Instance.con;

	public float lookHeightOffset = 0.2f;

	public float defaultPitch = 20;
	public float maxPitch = 90;

	[SerializeField, ReadOnly] float pitch = 0;
	[SerializeField, ReadOnly] float yaw = 0;

	Vector3 cachedPosition;

	public static CameraController Instance { get; private set; }
	private void Awake()
	{
		Instance = this;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void OnValidate()
	{
		pitch = defaultPitch;
		transform.localEulerAngles = new Vector3(pitch, yaw, 0);
	}

	private void OnDestroy()
	{
		con = null;
		Instance = null;
		Cursor.lockState = CursorLockMode.None;
	}

	public static void AssignControl(ICanControlCamera controller)
	{
		Instance.con = controller;
		if (controller == null)
		{
			HUD.Instance.SpectatingObj.SetActive(false);
		}
		else if (controller is NetworkBehaviour nb)
		{
			HUD.Instance.SpectatingObj.SetActive(!nb.Object.HasInputAuthority);
		}
	}

	public static bool HasControl(ICanControlCamera controller)
	{
		return Controller?.Equals(controller) == true;
	}

	private void LateUpdate()
	{
		if (!(GameManager.Instance?.Runner?.IsRunning == true) || PlayerRegistry.CountPlayers == 0) return;

		if (con == null && PlayerRegistry.CountWhere(p => p.Controller) > 0 && (GameManager.State.Current == GameState.EGameState.Intro || GameManager.State.Current == GameState.EGameState.Game))
		{
			AssignControl(PlayerRegistry.First(p => p.Controller != null).Controller);
		}

		if (con == null) return;

		if (con is NetworkBehaviour nb)
		{
			if (nb.Object.HasInputAuthority == false)
			{
				if (Input.GetMouseButtonDown(0))
				{
					AssignControl(PlayerRegistry.NextWhere(PlayerRegistry.First(p => p.Controller == nb), p => p.Controller).Controller);
					if (con == null) return;
				}
			}
		}

		con.ControlCamera(ref pitch, ref yaw);
		yaw = Mathf.Repeat(yaw + 180, 360) - 180;
		pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

		Quaternion orientation = Quaternion.Euler(pitch, yaw, 0);

		// Set the camera at the player's position with a height offset
		transform.position = con.Position;

		// Set the camera's rotation directly
		transform.rotation = orientation;
		con.Rotation = orientation;

		// Change to orientation 

		cachedPosition = con.Position;
	}

	public static void Recenter()
	{
		Instance.pitch = Instance.defaultPitch;
		Instance.yaw = 0;
	}
}
