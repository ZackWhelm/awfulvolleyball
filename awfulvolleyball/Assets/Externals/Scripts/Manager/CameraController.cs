using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Helpers.Math;
using Cinemachine;

public class CameraController : MonoBehaviour
{	
	ICanControlCamera con = null;
	public static ICanControlCamera Controller => Instance.con;

	public CinemachineVirtualCamera virtualCamera;

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
	}
}
