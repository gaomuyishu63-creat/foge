using UnityEngine;
using TMPro;

public class CollisionCounter : MonoBehaviour
{
    public TextMeshProUGUI collisionText; // 表示用のUI
    private int collisionCount = 0;

    private void OnCollisionEnter(Collision collision)
    {
        // 「Wall」と衝突したらカウントアップ
        if (collision.gameObject.CompareTag("wall"))
        {
            collisionCount++;
            collisionText.text = "Collisions: " + collisionCount;
        }
    }
}
