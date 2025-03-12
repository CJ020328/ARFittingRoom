using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class SwipeGestureDetector : MonoBehaviour
{
    public float swipeSpeedThreshold = 0.8f; // Reduced speed threshold
    public float xSwipeThreshold = 0.15f; // Reduced distance threshold
    public float ySwipeThreshold = 0.1f; // Reduced height threshold
    private float cooldownDuration = 0.5f; // Reduced cooldown duration
    private float cooldownTimer = 0.0f; // Benchmark
    private bool isSwipeDetected = false;

    private Vector3 previousRightHandPosition = Vector3.zero;
    private Vector3 currentRightHandPosition;
    private Vector3 previousLeftHandPosition = Vector3.zero;
    private Vector3 currentLeftHandPosition;

    public BodySourceManager bodySourceManager;

    public delegate void SwipeEventHandler(int direction);
    public event SwipeEventHandler OnSwipe;

    void Update()
    {
        if (cooldownTimer > 0.0f)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        Body body = GetFirstTrackedBody();
        if (body == null) return;

        currentRightHandPosition = GetHandPosition(body, JointType.WristRight);
        currentLeftHandPosition = GetHandPosition(body, JointType.WristLeft);

        if (previousRightHandPosition == Vector3.zero)
        {
            previousRightHandPosition = currentRightHandPosition;
        }

        if (previousLeftHandPosition == Vector3.zero)
        {
            previousLeftHandPosition = currentLeftHandPosition;
        }

        float distanceRightX = Mathf.Abs(currentRightHandPosition.x - previousRightHandPosition.x);
        float distanceLeftX = Mathf.Abs(currentLeftHandPosition.x - previousLeftHandPosition.x);
        float distanceRightY = Mathf.Abs(currentRightHandPosition.y - previousRightHandPosition.y);
        float distanceLeftY = Mathf.Abs(currentLeftHandPosition.y - previousLeftHandPosition.y);
        float speedRight = CalculateSpeed(previousRightHandPosition, currentRightHandPosition);
        float speedLeft = CalculateSpeed(previousLeftHandPosition, currentLeftHandPosition);

        if (!isSwipeDetected)
        {
            if (distanceRightX > xSwipeThreshold && speedRight > swipeSpeedThreshold && distanceRightY < ySwipeThreshold)
            {
                if (currentRightHandPosition.x < previousRightHandPosition.x)
                {
                    OnSwipe?.Invoke(1);
                    Debug.Log("Right hand swipe from right to left detected.");
                    isSwipeDetected = true;
                    cooldownTimer = cooldownDuration;
                }
            }
            else if (distanceLeftX > xSwipeThreshold && speedLeft > swipeSpeedThreshold && distanceLeftY < ySwipeThreshold)
            {
                if (currentLeftHandPosition.x > previousLeftHandPosition.x)
                {
                    OnSwipe?.Invoke(-1);
                    Debug.Log("Left hand swipe from left to right detected.");
                    isSwipeDetected = true;
                    cooldownTimer = cooldownDuration;
                }
            }
        }
        else
        {
            isSwipeDetected = false;
        }

        previousRightHandPosition = currentRightHandPosition;
        previousLeftHandPosition = currentLeftHandPosition;
    }

    private Vector3 GetHandPosition(Body body, JointType jointType)
    {
        CameraSpacePoint handPosition = body.Joints[jointType].Position;
        return new Vector3(handPosition.X, handPosition.Y, handPosition.Z);
    }

    private float CalculateSpeed(Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        float time = Time.deltaTime;
        return distance / time;
    }

    private Body GetFirstTrackedBody()
    {
        Body[] bodyData = bodySourceManager.GetData();
        if (bodyData == null) return null;

        foreach (var body in bodyData)
        {
            if (body.IsTracked) return body;
        }

        return null;
    }
}
