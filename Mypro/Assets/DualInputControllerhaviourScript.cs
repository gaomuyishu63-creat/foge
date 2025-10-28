using UnityEngine;

/// <summary>
/// 矢印キー（↑←→）とドラッグ（マウス/タッチ）を両立する簡易コントローラである。
/// ・前進は「前方向のみ」（allowReverse=falseなら後退しない）
/// ・前進している間だけ矢印（LineRendererまたはDebug.DrawRay）を表示する。
/// </summary>
public class DualInputController : MonoBehaviour
{
    [Header("移動・回転")]
    [SerializeField] float moveSpeed = 5f;      // 前進速度[m/s]
    [SerializeField] float turnSpeed = 120f;    // 旋回速度[deg/s]

    [Header("ドラッグ入力")]
    [SerializeField] float dragSensitivityX = 0.005f; // 横ドラッグ→ステア
    [SerializeField] float dragSensitivityY = 0.005f; // 縦ドラッグ→前進
    [SerializeField] float dragDeadZone   = 6f;       // ピクセル閾値
    [SerializeField] bool  prioritizeDrag = true;     // ドラッグ中はKBより優先する

    [Header("挙動オプション")]
    [SerializeField] bool  allowReverse = false;  // 後退を許可するか
    [SerializeField] float inputSmooth  = 12f;    // 入力のなめらかさ（Lerp係数）

    [Header("矢印表示（任意）")]
    [SerializeField] LineRenderer arrowLine;      // ゲーム画面に表示したいとき割当
    [SerializeField] float arrowLength = 2.5f;    // 矢印の長さ

    // 内部状態
    Vector2 dragStart;
    bool    dragging;
    float   targetForward;   // 目標前進入力（0..1 or -1..1）
    float   targetSteer;     // 目標ステア（-1..1）
    float   forward;         // 平滑化後の前進入力
    float   steer;           // 平滑化後のステア入力

    void Update()
    {
        // 1) キーボード（矢印キー）入力
        float kVertical = 0f;
        if (Input.GetKey(KeyCode.UpArrow))  kVertical = 1f;
        if (allowReverse && Input.GetKey(KeyCode.DownArrow)) kVertical = -1f;

        float kHorizontal = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))  kHorizontal = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) kHorizontal =  1f;

        // 2) ドラッグ入力（タッチ優先）
        bool dragActive = false;
        float dForward = 0f;
        float dSteer   = 0f;

        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                dragStart = touch.position;
                dragging = true;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if (!dragging) { dragStart = touch.position; dragging = true; }

                Vector2 delta = touch.position - dragStart;
                dragActive = delta.magnitude > dragDeadZone;

                if (dragActive)
                {
                    float rawForward = Mathf.Clamp(delta.y * dragSensitivityY, -1f, 1f);
                    dForward = allowReverse ? rawForward : Mathf.Max(0f, rawForward); // 下方向は0扱い
                    dSteer   = Mathf.Clamp(delta.x * dragSensitivityX, -1f, 1f);
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                dragging = false;
            }
        }
        else
        {
            // マウス（タッチパッド含む）
            if (Input.GetMouseButtonDown(0))
            {
                dragStart = Input.mousePosition;
                dragging = true;
            }
            if (Input.GetMouseButton(0) && dragging)
            {
                Vector2 delta = (Vector2)Input.mousePosition - dragStart;
                dragActive = delta.magnitude > dragDeadZone;

                if (dragActive)
                {
                    float rawForward = Mathf.Clamp(delta.y * dragSensitivityY, -1f, 1f);
                    dForward = allowReverse ? rawForward : Mathf.Max(0f, rawForward);
                    dSteer   = Mathf.Clamp(delta.x * dragSensitivityX, -1f, 1f);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
            }
        }

        // 3) 入力統合
        if (prioritizeDrag && dragActive)
        {
            targetForward = dForward;
            targetSteer   = dSteer;
        }
        else
        {
            float sumF = dForward + kVertical;
            float sumS = dSteer   + kHorizontal;
            targetForward = Mathf.Clamp(sumF, allowReverse ? -1f : 0f, 1f);
            targetSteer   = Mathf.Clamp(sumS, -1f, 1f);
        }

        // 4) 入力を平滑化（CS0136対策：変数名を lerpT に変更）
        float lerpT = 1f - Mathf.Exp(-inputSmooth * Time.deltaTime);
        forward = Mathf.Lerp(forward, targetForward, lerpT);
        steer   = Mathf.Lerp(steer,   targetSteer,   lerpT);

        // 5) 移動・回転（Transform直操作）
        if (Mathf.Abs(steer) > 1e-3f)
        {
            float yaw = steer * turnSpeed * Time.deltaTime;
            transform.Rotate(0f, yaw, 0f, Space.Self);
        }
        if (Mathf.Abs(forward) > 1e-3f)
        {
            Vector3 move = transform.forward * (forward * moveSpeed * Time.deltaTime);
            transform.Translate(move, Space.World);
        }

        // 6) 矢印表示（前進中のみ表示）
        bool showArrow = forward > 0.01f;
        DrawArrow(showArrow);
    }

    void DrawArrow(bool visible)
    {
        if (arrowLine != null)
        {
            arrowLine.enabled = visible;
            if (visible)
            {
                Vector3 p0 = transform.position + Vector3.up * 0.2f;
                Vector3 p1 = p0 + transform.forward * arrowLength;
                arrowLine.positionCount = 2;
                arrowLine.SetPosition(0, p0);
                arrowLine.SetPosition(1, p1);
            }
        }
        else
        {
            if (visible)
            {
                Debug.DrawRay(transform.position + Vector3.up * 0.2f,
                              transform.forward * arrowLength, Color.green, 0f, false);
            }
        }
    }
}
