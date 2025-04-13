using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingSide : MonoBehaviour
{
    [Header("Dependencies")]
	public Ring parentRing = null;

    void Start()
    {
        CheckDepenedencies();
    }

	private void CheckDepenedencies() {
		if (parentRing == null) {
            Debug.LogWarning("Dependencies fucked");
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.Instance.Runner.IsServer) return;

        if (other.TryGetComponent(out Ball ball))
        {
            if (!parentRing.CheckIsExit(this, ball)) {
                parentRing.HandleEnter(this);
            }

        }
    }

}
