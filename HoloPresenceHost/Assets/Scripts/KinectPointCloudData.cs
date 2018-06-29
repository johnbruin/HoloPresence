using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class KinectPointCloudData : MonoBehaviour
{
    public int scale = 10;
    public int skip = 1;

    private CoordinateMapper _Mapper;
    private FrameDescription colorFrameDesc;
    private FrameDescription depthFrameDesc;
    private FrameDescription bodyIndexFrameDesc;
    private MultiSourceFrameReader multiFrameSourceReader;

    private ushort[] depthFrameData;
    private byte[] colorFrameData;
    private byte[] bodyIndexFrameData;

    private CameraSpacePoint[] cameraSpacePoints;
    private ColorSpacePoint[] colorSpacePoints;

    private int bytesPerPixel = 4;
    private KinectSensor _Sensor;

    private particle[] _particles;

    // Public function so other scripts can grab the Kinect Data
    public particle[] GetData()
    {
        return _particles;
    }

    // Use this for initialization
    void Start()
    {
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            multiFrameSourceReader = _Sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth | FrameSourceTypes.Color | FrameSourceTypes.BodyIndex);

            _Mapper = _Sensor.CoordinateMapper;

            depthFrameDesc = _Sensor.DepthFrameSource.FrameDescription;
            var depthWidth = depthFrameDesc.Width;
            var depthHeight = depthFrameDesc.Height;
            depthFrameData = new ushort[depthWidth * depthHeight];
            colorSpacePoints = new ColorSpacePoint[depthWidth * depthHeight];
            cameraSpacePoints = new CameraSpacePoint[depthWidth * depthHeight];

            colorFrameDesc = _Sensor.ColorFrameSource.FrameDescription;
            var colorWidth = colorFrameDesc.Width;
            var colorHeight = colorFrameDesc.Height;
            colorFrameData = new byte[colorWidth * colorHeight * bytesPerPixel];

            bodyIndexFrameDesc = _Sensor.BodyIndexFrameSource.FrameDescription;
            var bodyIndexFrameWidth = bodyIndexFrameDesc.Width;
            var bodyIndexFrameHeight = bodyIndexFrameDesc.Height;
            bodyIndexFrameData = new byte[bodyIndexFrameWidth * bodyIndexFrameHeight];

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int depthWidth = 0;
        int depthHeight = 0;
        int colorWidth = 0;
        int colorHeight = 0;

        var multiSourceFrameProcessed = false;
        var colorFrameProcessed = false;
        var depthFrameProcessed = false;
        var bodyIndexFrameProcessed = false;

        if (_Sensor == null) return;

        var multiSourceFrame = multiFrameSourceReader.AcquireLatestFrame();

        if (multiSourceFrame != null)
        {
            using (var depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
            {
                using (var colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame())
                {
                    using (var bodyIndexFrame = multiSourceFrame.BodyIndexFrameReference.AcquireFrame())
                    {
                        if (depthFrame != null)
                        {
                            var depthFrameDescription = depthFrame.FrameDescription;
                            depthWidth = depthFrameDescription.Width;
                            depthHeight = depthFrameDescription.Height;
                            depthFrame.CopyFrameDataToArray(depthFrameData);
                            depthFrameProcessed = true;
                        }

                        if (colorFrame != null)
                        {
                            var colorFrameDescription = colorFrame.FrameDescription;
                            colorWidth = colorFrameDescription.Width;
                            colorHeight = colorFrameDescription.Height;
                            colorFrame.CopyConvertedFrameDataToArray(colorFrameData, ColorImageFormat.Bgra);
                            colorFrameProcessed = true;
                        }

                        if (bodyIndexFrame != null)
                        {
                            bodyIndexFrame.CopyFrameDataToArray(bodyIndexFrameData);
                            bodyIndexFrameProcessed = true;
                        }

                        multiSourceFrameProcessed = true;

                        if (multiSourceFrameProcessed && depthFrameProcessed && colorFrameProcessed && bodyIndexFrameProcessed)
                        {
                            var particles = new List<particle>();

                            _Mapper.MapDepthFrameToColorSpace(depthFrameData, colorSpacePoints);
                            _Mapper.MapDepthFrameToCameraSpace(depthFrameData, cameraSpacePoints);

                            for (int y = 0; y < depthHeight; y += skip)
                            {
                                for (int x = 0; x < depthWidth; x += skip)
                                {
                                    var depthIndex = (y * depthWidth) + x;
                                    var p = cameraSpacePoints[depthIndex];
                                    var colorPoint = colorSpacePoints[depthIndex];
                                    var player = bodyIndexFrameData[depthIndex];

                                    byte r = 0;
                                    byte g = 0;
                                    byte b = 0;

                                    var colorX = (int)System.Math.Floor(colorPoint.X + 0.5);
                                    var colorY = (int)System.Math.Floor(colorPoint.Y + 0.5);

                                    if ((colorX >= 0) && (colorX < colorWidth) && (colorY >= 0) && (colorY < colorHeight))
                                    {
                                        int colorIndex = ((colorY * colorWidth) + colorX) * bytesPerPixel;
                                        b = colorFrameData[colorIndex++];
                                        g = colorFrameData[colorIndex++];
                                        r = colorFrameData[colorIndex++];
                                    }

                                    if (!(double.IsInfinity(p.X)) && !(double.IsInfinity(p.Y)) && !(double.IsInfinity(p.Z)))
                                    {
                                        //if (p.X < 1.5 && p.Y < 1.5 && p.Z < 1.5)
                                        if (player != 0xff)
                                        {
                                            particles.Add(new particle
                                            {
                                                x = (short)(p.X * scale * 100),
                                                y = (short)(p.Y * scale * 100),
                                                z = (short)(p.Z * scale * 100),
                                                r = r,
                                                g = g,
                                                b = b,
                                            });
                                        }
                                    }
                                }
                            }
                            _particles = particles.ToArray();
                        }
                    }
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        multiFrameSourceReader.Dispose();
        multiFrameSourceReader = null;

        if (_Mapper != null)
        {
            _Mapper = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
}