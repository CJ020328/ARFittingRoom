using UnityEngine;
using Windows.Kinect;

public class KinectManager : MonoBehaviour
{
    private KinectSensor sensor;
    private BodyFrameReader bodyFrameReader;
    private Body[] bodies;

    void Start()
    {
        InitializeKinect();
    }

    void Update()
    {
        if (bodyFrameReader != null)
        {
            var frame = bodyFrameReader.AcquireLatestFrame();
            if (frame != null)
            {
                if (bodies == null)
                {
                    bodies = new Body[sensor.BodyFrameSource.BodyCount];
                }

                frame.GetAndRefreshBodyData(bodies);
                frame.Dispose();
            }
        }
    }

    private void InitializeKinect()
    {
        sensor = KinectSensor.GetDefault();

        if (sensor != null)
        {
            bodyFrameReader = sensor.BodyFrameSource.OpenReader();
            sensor.Open();
        }
        else
        {
            Debug.LogError("No Kinect sensor found.");
        }
    }

    void OnApplicationQuit()
    {
        if (bodyFrameReader != null)
        {
            bodyFrameReader.Dispose();
            bodyFrameReader = null;
        }

        if (sensor != null && sensor.IsOpen)
        {
            sensor.Close();
            sensor = null;
        }
    }

    public Body[] GetData()
    {
        return bodies;
    }
}
