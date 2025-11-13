using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 子でも親でも Player タグを持っていれば反応
        if (other.CompareTag("Player") ||
            (other.attachedRigidbody != null && other.attachedRigidbody.transform.CompareTag("Player")) ||
            other.transform.root.CompareTag("Player"))
        {
            Debug.Log("ゴールしました！");
        }
    }
}
