using UnityEngine;

public class PointCloudView : MonoBehaviour
{
    public ParticleSystem _particleSystem;
    public GameObject PointCloudSource;
    public float _particleSize;

    private PointCloudReceiver _PointCloudReceiver;

    void Start()
    {

    }

    void Update()
    {
        if (PointCloudSource == null)
        {
            return;
        }

        // Is the point cloud data coming from the PointCloudReceiver script?
        _PointCloudReceiver = PointCloudSource.GetComponent<PointCloudReceiver>();
        if (_PointCloudReceiver == null)
        {
            return;
        }

        var data = _PointCloudReceiver.GetData();
        if (data == null || data.Length < 1000)
        {
            return;
        }

        var particleCount = 0;
        var particles = new ParticleSystem.Particle[data.Length];
        foreach (var p in data)
        {
            var x = p.x / 100F;
            var y = p.y / 100F;
            var z = p.z / 100F;

            var color = new Color(p.r / 255F, p.g / 255F, p.b / 255F);

            particles[particleCount].position = new Vector3(x, y, z);
            particles[particleCount].startColor = color;
            particles[particleCount].startSize = _particleSize;
            particleCount++;
        }
        _particleSystem.SetParticles(particles, particles.Length);
    }
}