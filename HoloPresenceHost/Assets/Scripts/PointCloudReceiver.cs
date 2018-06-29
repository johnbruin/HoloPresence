using System.Collections.Generic;
using UnityEngine.Networking;

public class PointCloudReceiver : NetworkBehaviour
{
    private List<particle> _PointCloudList = new List<particle>();
    private particle[] _PointCloud;

    private const short pointCloudMessage = 136;
    private const short pointCloudMessageEnd = 137;

    public particle[] GetData()
    {
        return _PointCloud;
    }

    void Start()
    {
        //registering the client handlers
        NetworkManager.singleton.client.RegisterHandler(pointCloudMessage, ReceiveMessage);
        NetworkManager.singleton.client.RegisterHandler(pointCloudMessageEnd, ReceiveMessageEnd);
    }

    private void ReceiveMessage(NetworkMessage message)
    {
        var particleMessage = message.ReadMessage<ParticleMessage>();
        for (int i = 0; i < particleMessage.data.Length; i++)
        {
            _PointCloudList.Add(particleMessage.data[i]);
        }
    }

    private void ReceiveMessageEnd(NetworkMessage message)
    {
        message.ReadMessage<ParticleMessage>();
        _PointCloud = _PointCloudList.ToArray();
        _PointCloudList.Clear();
    }
}