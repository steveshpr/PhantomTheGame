using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LHandController : MonoBehaviour {

    [SerializeField]private OVRInput.Controller controller;
    [SerializeField]private float sensitivity;

    // Update is called once per frame
    void Update () {
        Vector3 sourcePosition = OVRInput.GetLocalControllerPosition(controller);
        sourcePosition.Scale(new Vector3(sensitivity, sensitivity, sensitivity));
        transform.localPosition = sourcePosition;
        transform.localRotation = OVRInput.GetLocalControllerRotation(controller);
    }
}
