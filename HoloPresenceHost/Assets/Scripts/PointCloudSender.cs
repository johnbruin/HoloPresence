using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PointCloudSender : MonoBehaviour
{
    public GameObject PointCloudSource;
    public int SendRate = 6;

    private const short pointCloudMessage = 136;
    private const short pointCloudMessageEnd = 137;
    private particle[] _pointCloudData;
    private int _rate;
    

    void Start()
    {

    }

    void Update()
    {
        if (PointCloudSource == null)
        {
            return;
        }

        // Get the parsed Kinect point cloud data
        var _KinectPointCloudData = PointCloudSource.GetComponent<KinectPointCloudData>();
        var pointCloudData = _KinectPointCloudData.GetData();
        if (pointCloudData == null || _pointCloudData == pointCloudData)
        {
            return;
        }
        _pointCloudData = pointCloudData;

        if (_rate == 0)
        {
            SendMessage(pointCloudData);
            _rate = SendRate;
        }
        else
        {
            _rate--;
        }
    }

    private void SendMessage(particle[] pointCloudData)
    {
        var pointCloudDataList = new List<particle>();
        var counter = 0;
        for (int i = 0; i < pointCloudData.Length; i++)
        {
            pointCloudDataList.Add(pointCloudData[i]);

            if (counter == 150 || i == pointCloudData.Length - 1)
            {
                var message = new ParticleMessage();
                message.data = pointCloudDataList.ToArray();

                //sending to server
                NetworkServer.SendToAll(pointCloudMessage, message);

                pointCloudDataList.Clear();
                counter = 0;
            }
            counter++;
        }
        NetworkServer.SendToAll(pointCloudMessageEnd, new ParticleMessage() { data = null });
    }
}