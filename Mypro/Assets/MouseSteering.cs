using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;  // 新Input Systemにも対応
#endif

public class MouseDriveFree : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider backLeftWheel;
    public WheelCollider backRightWheel;

    [Header("Steering")]
    public float maxSteerAngle = 30f;     // 最大舵角
    public float steerSensitivity = 10f;  // 1秒間にどれだけ舵角を変えるか（横スライド感度）
    public float steerReturn = 2f;        // 入力が止まった時に0へ戻る速さ

    [Header("Throttle / Brake")]
    public float motorPower = 300f;       // 駆動力（必要に応じて調整）
    public float throttleSensitivity = 5f;// 1秒間にどれだけスロットルを変えるか（縦スライド感度）
    public float throttleReturn = 2f;     // 入力が止まった時に0へ戻る速さ
    public bool invertY = false;          // 上で前進が良ければ false、逆が良ければ true

    float steerTarget = 0f;   // 目標舵角（-max~+max）
    float currentSteer = 0f;  // 実際に適用する舵角
    float throttle = 0f;      // -1..+1（後退..前進）

    void Update()
    {
        // 1フレームのポインタ移動量（タッチパッドもここに入ります）
        float dx = 0f, dy = 0f;

        #if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            Vector2 d = Mouse.current.delta.ReadValue();
            dx = d.x;
            dy = d.y;
        }
        #else
        dx = Input.GetAxis("Mouse X") * 10f; // 旧InputManagerのスケール合わせ
        dy = Input.GetAxis("Mouse Y") * 10f;
        #endif

        // スライドした分だけ増減（クリック不要）
        steerTarget += dx * steerSensitivity * Time.deltaTime;

        float sign = invertY ? +1f : -1f; // 上に動かすと前進にしたければ sign=-1
        throttle   += sign * dy * throttleSensitivity * Time.deltaTime;

        // 入力が止まったら自然に戻す
        if (Mathf.Abs(dx) < 0.0001f)
            steerTarget = Mathf.Lerp(steerTarget, 0f, steerReturn * Time.deltaTime);
        if (Mathf.Abs(dy) < 0.0001f)
            throttle   = Mathf.Lerp(throttle,    0f, throttleReturn * Time.deltaTime);

        // 範囲制限
        steerTarget = Mathf.Clamp(steerTarget, -maxSteerAngle, maxSteerAngle);
        throttle    = Mathf.Clamp(throttle,    -1f,           +1f);
    }

    void FixedUpdate()
    {
        // 物理更新タイミングで適用（他スクリプトに上書きされにくい）
        currentSteer = Mathf.MoveTowards(currentSteer, steerTarget, 200f * Time.fixedDeltaTime);

        if (frontLeftWheel)  frontLeftWheel.steerAngle  = currentSteer;
        if (frontRightWheel) frontRightWheel.steerAngle = currentSteer;

        float torque = throttle * motorPower;
        if (backLeftWheel)  backLeftWheel.motorTorque  = torque;
        if (backRightWheel) backRightWheel.motorTorque = torque;
    }
}
