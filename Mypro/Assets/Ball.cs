using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 dir, float spd)
    {
        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(dir.normalized * spd, ForceMode.VelocityChange);
        enabled = true; // このスクリプトを再有効化（必要なら）
    }

    public void Stop()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;        // 物理停止
        rb.detectCollisions = false;  // 以後の当たりを遮断
        rb.constraints = RigidbodyConstraints.FreezeAll;
        enabled = false;              // 念のため自分も止める
    }
}
