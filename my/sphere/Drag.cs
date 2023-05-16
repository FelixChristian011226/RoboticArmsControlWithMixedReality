using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class Drag : MonoBehaviour, IMixedRealityPointerHandler
{
    public UR5Controller arm;
    private bool is_dragging = false;
    private Vector3 lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = arm.TCP_Pose();
        transform.localPosition = lastPosition + arm.transform.localPosition;
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        is_dragging = true;
        lastPosition = eventData.Pointer.Position;
    }
    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        is_dragging= false;
    }
    public void OnPointerDragged(MixedRealityPointerEventData eventData){
        if (is_dragging)
        {
            Vector3 delta = eventData.Pointer.Position - lastPosition;
            //
            lastPosition = eventData.Pointer.Position;

        }
    }
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = lastPosition;
        //
        //transform.localPosition = lastPosition + arm.transform.localPosition;
        arm.TCP_Move(transform.localPosition - arm.transform.localPosition);
    }
}
