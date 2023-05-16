# Introduction
This is a SRTP project in ZJU. A research on remote control technology based on mixed reality.

# Results
![unity](https://github.com/FelixChristian011226/RoboticArmsControlWithMixedReality/blob/main/Results/Unity.gif)  
                        *Unity Simulation*  
![Hololens1](https://github.com/FelixChristian011226/RoboticArmsControlWithMixedReality/blob/main/Results/Hololens1.gif)  
                        *Hololens2 Deployment 1*  
![Hololens2](https://github.com/FelixChristian011226/RoboticArmsControlWithMixedReality/blob/main/Results/Hololens2.gif)  
                        *Hololens2 Deployment 2*  

# How to Use?
## (0) Preparations
1. [Unity Hub](https://unity.com/cn/unity-hub) and Unity Editor (2019.4 recommended).
2. [VMware Workstation](https://www.vmware.com/cn/products/workstation-player.html) the virtual machine.
3. [URsim](https://www.universal-robots.com/download/software-e-series/simulator-non-linux/offline-simulator-e-series-ur-sim-for-non-linux-594/), you could follow the official website for installation help.
4. [Visual Studio](https://visualstudio.microsoft.com/zh-hans/) for deployment onto hololens2.
## (1) Deploy on Unity
1. Create a new 3D Core project.
2. Import MRTK package to your project. You can download the [Mixed Reality Feature Tool](https://www.microsoft.com/en-us/download/details.aspx?id=102778) here, then discover and import features as told in [learn.microsoft.com](https://learn.microsoft.com/zh-cn/windows/mixed-reality/mrtk-unity/mrtk2/configuration/usingupm?view=mrtkunity-2022-05) (you could just import the essential features).
3. Import [my](https://github.com/FelixChristian011226/RoboticArmsControlWithMixedReality/tree/main/my) into the Assets folder in your project.
4. Open scene [ur5control](https://github.com/FelixChristian011226/RoboticArmsControlWithMixedReality/blob/main/my/UR5/ur5control.unity).
5. Start your URsim virtual machine, open app 'URsim UR5', record the ip of your virtual machine ('关于' on the top right corner).
6. Open file [UR5Controller.cs](https://github.com/FelixChristian011226/RoboticArmsControlWithMixedReality/blob/main/my/UR5/UR5Controller.cs), set the ip (line 93) to your URsim.
7. Play the scene.
## (2) Deploy on Hololens2
