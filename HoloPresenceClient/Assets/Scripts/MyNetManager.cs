using UnityEngine.Networking;

public class MyNetManager : NetworkManager
{
    public NetworkDiscovery discovery;

    public void Start()
    {
        discovery.Initialize();
        discovery.StartAsClient();
    }   
}