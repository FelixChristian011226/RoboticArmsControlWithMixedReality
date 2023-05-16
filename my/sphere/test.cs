using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class test : MonoBehaviour
{
    public UR5Controller arm;
    private Vector3 lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = arm.TCP_Pose();
        transform.localPosition = lastPosition + arm.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        lastPosition = arm.TCP_Pose();

        transform.localPosition = lastPosition + arm.transform.localPosition;
    }
}
