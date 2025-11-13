using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class CarAgent : Agent
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque = 1500f;
    public float maxSteeringAngle = 30f;

    public int max_step = 300;

    public int current_step = 0;

    public Transform target;
    private Rigidbody rBody;

    private float steering;
    private float motor;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    public override void OnEpisodeBegin()
    {
        rBody.linearVelocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
        transform.localPosition = startPosition;
        transform.localRotation = startRotation;

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = 0;
                axleInfo.rightWheel.steerAngle = 0;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = 0;
                axleInfo.rightWheel.motorTorque = 0;
            }
        }
        // ゴール位置は固定なので、何もしない
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // ゴールまでの相対位置
        sensor.AddObservation(target.localPosition - transform.localPosition);

        // 車の速度
        sensor.AddObservation(rBody.linearVelocity);

        // 車の進行方向
        sensor.AddObservation(transform.forward);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        steering = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f) * maxSteeringAngle;
        motor = Mathf.Clamp(actionBuffers.ContinuousActions[1], 0, 1f) * maxMotorTorque;

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }

        // ゴールとの距離に応じて小さな負報酬（遠いほどマイナス）
        //float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);
        //AddReward(-0.001f * distanceToTarget);

        // コースアウトした場合のペナルティ
        if (transform.localPosition.y < -0.5f)
        {
            AddReward(-1.0f);
            EndEpisode();
        }
        current_step = StepCount;
        if (current_step >= max_step)
        {
            AddReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    // ゴール衝突判定
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            AddReward(+1.0f);   // 成功報酬
            EndEpisode();
        }
        else if (other.CompareTag("Checkpoint"))
        {
            AddReward(+1.0f);
        }
        else if (other.CompareTag("Obstacle"))
        {
            AddReward(-1.0f);   // 障害物に衝突したらペナルティ
            EndEpisode();
        }
    }
}
