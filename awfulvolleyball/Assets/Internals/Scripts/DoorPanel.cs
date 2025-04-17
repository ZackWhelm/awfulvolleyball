using System.Collections;
using UnityEngine;

public class DoorPanel : MonoBehaviour
{
    [Header("Traits")]
    public Vector3 endPos; 
    public float duration; 

    public void Open() {
       StartCoroutine(MoveToPosition());
    }

    private IEnumerator MoveToPosition() {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < duration) {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos; // snap to final position
    }
}
