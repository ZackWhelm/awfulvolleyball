using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class PlayerInputBehaviour : Fusion.Behaviour, INetworkRunnerCallbacks
{
	float xInput = 0;
	float yInput = 0;
	bool spaceInput = false;
	bool zInput = false;
	bool shiftInput = false;
	bool controlInput = false;

	private void Update()
	{
		xInput = Input.GetAxis("Horizontal");
		yInput = Input.GetAxis("Vertical");
		spaceInput = Input.GetKey(KeyCode.Space);
		zInput = Input.GetKey(KeyCode.Z);
		shiftInput = Input.GetKey(KeyCode.LeftShift);
		controlInput = Input.GetKey(KeyCode.LeftControl);
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		if (PlayerObject.Local == null || PlayerObject.Local.Controller == null) return;
		if (UIScreen.activeScreen != InterfaceManager.Instance.hud) return;
		if (GameManager.State.Current != GameState.EGameState.Intro
			&& GameManager.State.Current != GameState.EGameState.Game) return;

		PlayerInput fwInput = new PlayerInput();

		fwInput.horDir = xInput;
		fwInput.vertDir = yInput;
		fwInput.jumpInput = spaceInput;
		fwInput.dashInput = zInput;
		fwInput.sprintInput = shiftInput;
		fwInput.crouchInput = controlInput;
		input.Set(fwInput);
	}

	#region INetworkRunnerCallbacks
	public void OnConnectedToServer(NetworkRunner runner) { }
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
	public void OnDisconnectedFromServer(NetworkRunner runner) { }
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
	public void OnSceneLoadDone(NetworkRunner runner) { }
	public void OnSceneLoadStart(NetworkRunner runner) { }
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    #endregion
}

public struct PlayerInput : INetworkInput
{
	public bool jumpInput;
	public bool dashInput;
	public bool sprintInput;
	public bool crouchInput;
	public float horDir;
	public float vertDir;
}