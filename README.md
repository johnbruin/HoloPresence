# HoloPresence

HoloPresence is a very simple implementation of Microsoft's Holoportation. 
It takes the 3D pointcloud from the Kinect and sends this to the HoloLens client application in the same network.

HoloPresence uses:
- the particle system to render the pointcloud
- simple (and slow) Unet networking
- Unet network discovery
- Mixed Reality Toolkit App bar and Bounding box

The solution consists of 2 Unity projects that are pretty much similar except that the HoloPresenceHost is a only working on a Windows PC with a Kinect 2.0 attached. It is using a reference to the KinectUnityAddin.dll

The HoloPresenceClient is a mulit platform Unity application using the Mixed Reality Toolkit App bar and Bounding box and can be deployed to a HoloLens.

Developer hint: With Kinect Studio you can record the raw Kinect streams to a file and replay them anytime you want without connecting the Kinect.
