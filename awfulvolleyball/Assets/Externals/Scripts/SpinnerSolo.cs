using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SpinnerSolo : MonoBehaviour
{
	public Rigidbody rb;
	public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
	public Vector3 axis = Vector3.forward;
	public float speedFactor = 1;
	[Range(0,1)] public float phaseOffset = 0;


    void Update()
    {
		rb.MoveRotation(transform.rotation * Quaternion.AngleAxis(speedFactor * Time.fixedDeltaTime, transform.TransformDirection(axis)));
    }

    private void OnValidate()
	{
		rb.transform.rotation = Quaternion.AngleAxis(
			curve.Evaluate(phaseOffset / curve.keys[curve.keys.Length - 1].time) * 360,
			transform.TransformDirection(axis));
	}
}
