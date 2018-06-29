using UnityEngine.Networking;

public class MyNetManager : NetworkManager
{
    public NetworkDiscovery discovery;

    public void Start()
    {
        this.StartHost();
        discovery.Initialize();
        discovery.StartAsServer();
    }

    public override void OnStartHost()
    {

    }
}