// Author: Long Qian
// Email: lqian8@jhu.edu

// Network supplemantation by Zain
// Email : zainmehdi31@gmail.com
// Special thanks to Bonghan Kim for help in command mapping

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;



public class UR5Controller : MonoBehaviour {


    /// <summary> 	
    /// TCPListener to listen for incomming TCP connection 	
    /// requests. 	
    /// </summary> 	
    private TcpListener tcpListener;
    /// <summary> 
    /// Background thread for TcpServer workload. 	
    /// </summary> 	
    private Thread tcpListenerThread;

    /// <summary> 	
    /// Create handle to connected tcp client. 	
    /// </summary> 	
    private TcpClient connectedTcpClient;
    private NetworkStream stream;



    public GameObject RobotBase;
    public ArmJoint[] armjoint;


    int Port = 0;
    private string[] buffer;
    public float[] jointValues = new float[6];
    public double[] tcp_pose = new double[3];

    private GameObject[] jointList = new GameObject[6];
    private float[] upperLimit = { 180f, 180f, 180f, 180f, 180f, 180f };
    private float[] lowerLimit = { -180f, -180f, -180f, -180f, -180f, -180f };

    bool run = false;

    private Vector3 lastPosition;

    DateTime start;



    // Use this for initialization
    void Start () {
        initializeJoints();
        tcp_pose[0] = 0;
        tcp_pose[1] = 0;
        tcp_pose[2] = 0;

        // var z= port.GetComponent<InputField>();
        // Port = int.Parse(z.text);

        // Start TcpServer background thread 		
        //tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        //tcpListenerThread.IsBackground = true;
        //tcpListenerThread.Start();
        Debug.Log("Connecting!");
        Connect();
        Debug.Log("Connecting Completed!");
        tcpListenerThread = new Thread(ListenForIncommingRequests);
        tcpListenerThread.Start();

        string command = "play\n";
        byte[] data = Encoding.ASCII.GetBytes(command);
        stream.Write(data, 0, data.Length);

        start = DateTime.Now;



    }
    void Connect()
    {
        connectedTcpClient = new TcpClient("192.168.137.1", 30003);
        stream = connectedTcpClient.GetStream();
    }

    // Update is called once per frame
    void LateUpdate () {

        //for (int i = 0; i < 6; i++)
        for ( int i = 0; i < 6; i ++) {
            Vector3 currentRotation = jointList[i].transform.localEulerAngles;
          //  Debug.Log(currentRotation);
            currentRotation.z = jointValues[i];
            jointList[i].transform.localEulerAngles = currentRotation;
           

        }
    }

    void OnGUI() {
        int boundary = 20;


#if UNITY_EDITOR
        int labelHeight = 25;
        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 18;
#else
                int labelHeight = 40;
                GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 40;
#endif

        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        for (int i = 0; i < 6; i++)
        {
            GUI.Label(new Rect(50, boundary + (i * 2 + 1) * labelHeight+250, labelHeight * 7, labelHeight), "Joint " + i +" = "+jointValues[i].ToString("F1")+" "+ " ");
           // jointValues[i] = GUI.HorizontalSlider(new Rect(boundary+50 + labelHeight * 4, boundary + (i * 2 + 1) * labelHeight + labelHeight / 4, labelHeight * 5, labelHeight), jointValues[i], lowerLimit[i], upperLimit[i]);
        }
    }


    // Create the list of GameObjects that represent each joint of the robot
    void initializeJoints() {
        var RobotChildren = RobotBase.GetComponentsInChildren<Transform>();
        for (int i = 0; i < RobotChildren.Length; i++) {
            if (RobotChildren[i].name == "control0") {
                jointList[0] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control1") {
                jointList[1] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control2") {
                jointList[2] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control3") {
                jointList[3] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control4") {
                jointList[4] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control5") {
                jointList[5] = RobotChildren[i].gameObject;
            }
        }
    }

    /// <summary> 	
    /// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
    /// </summary> 	
    /// 
    void OnApplicationQuit()
    {
        tcpListenerThread.Abort();
    }
    private void ListenForIncommingRequests()
    {
        while (true)
        {
            Byte[] bytes = new Byte[2048];
            int length;
            // Read incomming stream into byte arrary. 						
            length = stream.Read(bytes, 0, bytes.Length);
            var incommingData = new byte[length];
            Array.Copy(bytes, 0, incommingData, 0, length);
            // Convert byte array to string message. 							
            double[] joints_pos = new double[6];
            double[] tcp_pos = new double[6];
            byte[] temp_8 = new byte[8];
            byte[] temp_4 = new byte[4];
            Array.Copy(incommingData, 0, temp_4, 0, 4);
            Array.Reverse(temp_4);
            int numbers = BitConverter.ToInt32(temp_4, 0);
            if (numbers != length)
                continue ;
            for (int i = 0; i < 6; i++)
            {
                Array.Copy(incommingData, 252 + 8 * i, temp_8, 0, 8);
                Array.Reverse(temp_8);
                joints_pos[i] = BitConverter.ToDouble(temp_8,0);
            }


            jointValues[0] = (float)((joints_pos[0] * (-1)) * 180 / Math.PI);
            jointValues[1] = (float)((joints_pos[1]) * 180 / Math.PI + 90f);    //jointValues[1] = (float)(((Convert.ToDouble(buffer[1]))*(-1)) * 180 / Math.PI - 90f);
            jointValues[2] = (float)((joints_pos[2]) * 180 / Math.PI);          //jointValues[2] = -1f*(float)((Convert.ToDouble(buffer[2])) * 180 / Math.PI);
            jointValues[3] = (float)((joints_pos[3]) * 180 / Math.PI) + 90f;    //jointValues[3] = (float)((Convert.ToDouble(buffer[3])) * 180 / Math.PI) * (-1) - 90f;
            jointValues[4] = -1f * (float)((joints_pos[4]) * 180 / Math.PI);
            jointValues[5] = (float)((joints_pos[5]) * 180 / Math.PI);

            for (int i = 0; i < 6; i++)
            {
                Array.Copy(incommingData, 444 + 8 * i, temp_8, 0, 8);
                Array.Reverse(temp_8);
                tcp_pos[i] = BitConverter.ToDouble(temp_8,0);
            }

            // x+ = x+; y+ = z+ ;z+ = y+;
            tcp_pose[0] = (tcp_pos[0]) * 9.2f;
            tcp_pose[1] = (tcp_pos[2]) * 9.2f;
            tcp_pose[2] = (tcp_pos[1]) * 9.2f;

            //Debug.Log("Angle 3" + jointValues[2]);
        }

    }
    /// <summary> 	
    /// Send message to client using socket connection. 	just a test
    /// </summary> 	
    private void SendMessage()
    {
        // Some code here...
        DateTime end = DateTime.Now;

        TimeSpan duration = end - start;
        double milliseconds = duration.TotalMilliseconds;
        if (milliseconds < 50) // 2000
            return;
        

        while (connectedTcpClient == null)
            Connect();

        //byte[] message;
        //// Convert a string message to a byte array
        //string command = String.Format("var p = [{0},{1},{2}]\n", lastPosition.x, lastPosition.y, lastPosition.z);

        //message = Encoding.UTF8.GetBytes(command);
        //    // Send the byte array over the socket
        //    //connectedTcpClient.Send(message);
        //stream.Write(message, 0, message.Length);
        //string command = String.Format("servoj(get_inverse_kin(p[{0},{1},{2},{3},{4},{5}]),t=0.5,lookahead_time=0.1,gain=200)\n",lastPosition.x, lastPosition.y, lastPosition.z, 0, 0.5, 0);
        string command = String.Format("servoj(get_inverse_kin(p[{0},{1},{2},{3},{4},{5}]),t=0.1,lookahead_time=0.05)\n", lastPosition.x, lastPosition.y, lastPosition.z, 0, 0.5, 0);
        //string command = String.Format("movej(get_inverse_kin(p[{0},{1},{2},{3},{4},{5}]),a=1.5,v=2.5)\n", lastPosition.x, lastPosition.y, lastPosition.z, 0, 0.5, 0);
        byte[] message = Encoding.UTF8.GetBytes(command);
        // Send the byte array over the socket
        //connectedTcpClient.Send(message);
        stream.Write(message, 0, message.Length);

        start = DateTime.Now;

    }

    public Vector3 ForwardKinematics()
    {
        return armjoint[armjoint.Length-1].transform.localPosition;
        Vector3 prevPoint = armjoint[0].transform.position;
        Quaternion rotation = Quaternion.identity;
        for (int i = 1; i < armjoint.Length; i++)
        {
            // Rotates around a new axis
            rotation *= Quaternion.AngleAxis(jointValues[i - 1], armjoint[i - 1].RotationAxis);
            Vector3 nextPoint = prevPoint + 2 * (Vector3)(rotation * armjoint[i].transform.localPosition);

            prevPoint = nextPoint;
        }
        return prevPoint;
    }
    public Vector3 TCP_Pose()
    {
        Vector3 res = new Vector3((float)tcp_pose[0], (float)tcp_pose[1], (float)tcp_pose[2]);
        return res;
    }

    public void TCP_Move(Vector3 _lastPosition )
    {
        
        this.lastPosition[0] = _lastPosition[0] / 9.2f;
        this.lastPosition[1] = _lastPosition[2] / 9.2f;
        this.lastPosition[2] = _lastPosition[1] / 9.2f;
    }
    void Update()
    {
        SendMessage();
        //ListenForIncommingRequests();

        //byte[] message = Encoding.UTF8.GetBytes(String.Format("servoj(get_inverse_kin(p[{0},{1},{2},{3},{4},{5}]),0.01,lookahead.time=0.1,gain=200)", lastPosition.x, lastPosition.y, lastPosition.z, 0, 0.5, 0));
        //// Send the byte array over the socket
        //stream.Write(message, 0, message.Length);
    }
}
