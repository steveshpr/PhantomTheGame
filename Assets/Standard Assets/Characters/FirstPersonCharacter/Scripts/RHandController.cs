using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phantom.Utility;
using Phantom.Utility.MessageBus;
using System;

public class RHandController : MonoBehaviour, ISubscriber<PickArrowEvent>{

    [SerializeField]private OVRInput.Controller controller;
    [SerializeField]private float sensitivity;

    [SerializeField]private GameObject sword;
    [SerializeField]private GameObject arrow;

    private void Start()
    {
        MainBus.Instance.Subscribe(this);
    }

    // Update is called once per frame
    void Update () {
        Vector3 sourcePosition = OVRInput.GetLocalControllerPosition(controller);
        sourcePosition.Scale(new Vector3(sensitivity, sensitivity, sensitivity));
        transform.localPosition = sourcePosition;
        transform.localRotation = OVRInput.GetLocalControllerRotation(controller);
    }

    private void OnCollisionEnter(Collision col)
    {
        //enemy or dead body or arrow on ground
        if (col.gameObject.layer == 10 || col.gameObject.layer == 11 || col.gameObject.layer == 13)
        {
            if (sword.activeSelf)
            {
                sword.GetComponent<Sword>().target = col.gameObject;
                return;
            }

            if (arrow.activeSelf) {
                return;
            }
            GetComponent<Renderer>().material.color = Color.red;
            GetComponent<RigidbodyDragger>().target = col.gameObject;
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.layer == 10 || col.gameObject.layer == 11 || col.gameObject.layer == 13)
        {
            if (sword.activeSelf)
            {
                sword.GetComponent<Sword>().target = null;
            }
            GetComponent<Renderer>().material.color = Color.white;
            GetComponent<RigidbodyDragger>().target = null;
        }
    }

    public void OnEvent(PickArrowEvent evt)
    {
        if (!arrow.activeSelf) {
            Destroy(evt.arrow);
            arrow.SetActive(true);
        }
    }
}
