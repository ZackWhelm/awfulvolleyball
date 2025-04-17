using UnityEngine;

public class StickmanAnimationHandler : MonoBehaviour
{
	[Header("Traits")]
	public float rotationSpeed = 1f;

	[Header("Dependencies")]
	public Transform parentTransform;
	public Animator animator;
	public StickmanMovementHandler stickmanMovementHandler;


	public void UpdateAnimationDirection(Vector3 movementDirection) {
		if (movementDirection != Vector3.zero) {
			Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
			Vector3 targetEuler = new Vector3(0, targetRotation.eulerAngles.y, 0);

			Quaternion smoothRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetEuler), Time.deltaTime * rotationSpeed);
			parentTransform.rotation = smoothRotation;
			animator.SetBool("IsIdle", false);
			ComputeState();
		}
		else {
			animator.SetBool("IsJogging", false);
			animator.SetBool("IsSprinting", false);
			animator.SetBool("IsIdle", true);
		}
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
