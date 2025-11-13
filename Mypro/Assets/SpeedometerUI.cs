using UnityEngine;
using TMPro;

public class SpeedometerUI : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    public bool useKmh = true;     // km/h 表示
    public int decimals = 0;       // 小数桁
    public float smooth = 8f;      // 表示のならし係数

    private Rigidbody rb;
    private float smoothed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // m/s -> km/h or mph
        float ms = rb.linearVelocity.magnitude;
        float v = useKmh ? (ms * 3.6f) : (ms * 2.2369363f);

        // スムージング（指数移動平均）
        float k = 1f - Mathf.Exp(-smooth * Time.deltaTime);
        smoothed = Mathf.Lerp(smoothed, v, k);

        // 文字列生成（補間を使わず Format で安全に）
        string unit = useKmh ? "km/h" : "mph";
        string fmt = "F" + Mathf.Max(0, decimals).ToString();
        if (speedText != null) speedText.text = smoothed.ToString(fmt) + " " + unit;
    }
}
