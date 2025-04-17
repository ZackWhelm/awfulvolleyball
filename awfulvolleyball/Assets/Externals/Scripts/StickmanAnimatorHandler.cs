using UnityEngine;
using UnityEngine.UIElements;

public class StickmanAnimationHandler : MonoBehaviour
{
	[Header("Traits")]
	public float rotationSpeed = 1f;
	public float offsetFromRB = -0.2625f;

	[Header("Dependencies")]
	public Animator animator;
	public StickmanMovementHandler stickmanMovementHandler;

	public void UpdateAnimPosition(Vector3 pos) {
		// transform.position = new Vector3(pos.x, pos.y + offsetFromRB, pos.z);
		transform.position = pos;
		transform.position += new Vector3(0.0f, offsetFromRB, 0.0f);
	}

	public void UpdateAnimationDirection(Vector3 movementDirection) {
		if (movementDirection != Vector3.zero) {
			Debug.Log("In Here");
			Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
			Vector3 targetEuler = new Vector3(0, targetRotation.eulerAngles.y, 0);
			Debug.Log("targetEuler: " + targetEuler);

			Quaternion smoothRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetEuler), Time.deltaTime * rotationSpeed);
			transform.rotation = smoothRotation;
			animator.SetBool("IsIdle", false);
			ComputeState();
		}
		else {
			animator.SetBool("IsJogging", false);
			animator.SetBool("IsSprinting", false);
			animator.SetBool("IsIdle", true);
		}

		// Rotate slowly to face movement direction
	}

	public void StartJump() {
		animator.SetTrigger("Jump");
	}

	private void ComputeState() {
		if (stickmanMovementHandler.IsSprinting) {
			animator.SetBool("IsSprinting", true);
			animator.SetBool("IsJogging", false);
		} else {
			animator.SetBool("IsJogging", true);
			animator.SetBool("IsSprinting", false);
		}
	}

}
