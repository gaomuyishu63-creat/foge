using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] string ballTag = "Ball";

    bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag(ballTag)) return;
        triggered = true;

        // ボールを完全停止
        other.GetComponent<Ball>()?.Stop();

        Debug.Log("GOAL!");

        // ここで必要ならリスポーンなど
        // StartCoroutine(Respawn(other.transform));
    }
}
