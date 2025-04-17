using UnityEngine;
using UnityEngine.Events;

public class Ring : MonoBehaviour
{
	[Header("Traits")]
	public float thresholdTime = 0.85f;
	public float currTime = 0.00f;
	public RingSide currentSideEntering = null;

    [Header("Dependencies")]
	public RingSide sideOne = null;
	public RingSide sideTwo = null;
	public UnityEvent onRingPassedThrough = null;

    void Start()
    {
        CheckDepenedencies();
    }

    void FixedUpdate()
    {
        if (currentSideEntering == null) {
			return;
		}
		currTime += Time.fixedDeltaTime;
		if (currTime > thresholdTime) {
			currentSideEntering = null;
		}
    }

    // TODO(zack): update to handle dual balls.
    public void HandleEnter(RingSide side) {
		currTime = 0.0f;
		currentSideEntering = side;
	}

	public bool CheckIsExit(RingSide side, Ball ball) {
		if (currentSideEntering == null) {
			return false;
		}
		if (side != currentSideEntering) {
			currentSideEntering = null;
			HandleBall(ball);
			return true;
		}
		return false;
	}

	private void CheckDepenedencies() {
		if (sideOne == null || sideTwo == null) {
            Debug.LogWarning("Dependencies fucked");
        }
	}

	private void HandleBall(Ball ball) {
		onRingPassedThrough.Invoke();
		/*
		player.PlayerObj.TimeTaken = (GameManager.Instance.Runner.Tick - GameManager.Instance.TickStarted) * GameManager.Instance.Runner.DeltaTime;
		GameManager.Instance.Runner.Despawn(player.Object);

		if (PlayerRegistry.All(p => p.HasFinished))
		{
			GameManager.State.Server_SetState(GameState.EGameState.Outro);
		}
		*/
	}
}
