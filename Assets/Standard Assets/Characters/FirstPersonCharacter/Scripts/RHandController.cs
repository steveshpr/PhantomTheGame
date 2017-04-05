using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phantom.Utility;

public class RHandController : MonoBehaviour {

    [SerializeField]private OVRInput.Controller controller;
    [SerializeField]private float sensitivity;

    [SerializeField]private GameObject sword;

    // Update is called once per frame
    void Update () {
        Vector3 sourcePosition = OVRInput.GetLocalControllerPosition(controller);
        sourcePosition.Scale(new Vector3(sensitivity, sensitivity, sensitivity));
        transform.localPosition = sourcePosition;
        transform.localRotation = OVRInput.GetLocalControllerRotation(controller);
    }

    private void OnCollisionEnter(Collision col)
    {
        
        if (col.gameObject.layer != 8 && col.gameObject.layer != 9)
        {
            if (sword.activeSelf)
            {
                sword.GetComponent<Sword>().target = col.gameObject;
                return;
            }
            GetComponent<Renderer>().material.color = Color.red;
            GetComponent<RigidbodyDragger>().target = col.gameObject;
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.layer != 8 && col.gameObject.layer != 9)
        {
            if (sword.activeSelf)
            {
                sword.GetComponent<Sword>().target = null;
                return;
            }
            GetComponent<Renderer>().material.color = Color.white;
            GetComponent<RigidbodyDragger>().target = null;
        }
    }
}
