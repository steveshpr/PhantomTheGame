using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phantom.Utility.MessageBus;
using UnityStandardAssets.CrossPlatformInput;
using Phantom.Utility;

public class RHDrawWeapon : MonoBehaviour
{

    [SerializeField]private GameObject hand;
    [SerializeField]private GameObject weapon;
    [SerializeField]private GameObject counterWeapon;
    [SerializeField]public int count;

    private bool drawing = false;
    private bool holding = false;
    private SpringJoint dragging;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (drawing)
        {
            MainBus.Instance.PublishEvent(new HUDSetIcon(weapon.gameObject.name, "green"));
            if (CrossPlatformInputManager.GetAxis("HoldR") > 0.4f)
            {
                if (!holding && count > 0)
                {
                    holding = true;
                    weapon.SetActive(true);
                    count--;
                }
            }
            else if(CrossPlatformInputManager.GetButton("RCancel"))
            {
                if (holding)
                {
                    holding = false;
                    weapon.SetActive(false);
                    count++;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(hand))
        {
            var dragging = hand.GetComponent<RigidbodyDragger>().m_SpringJoint;
            if (!counterWeapon.activeSelf && !dragging)
            {
                drawing = true;
                if (weapon.activeSelf)
                {
                    holding = true;
                }
            }
            else
            {
                MainBus.Instance.PublishEvent(new HUDSetIcon(weapon.gameObject.name, "red"));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(hand))
        {
            if (!counterWeapon.activeSelf)
            {
                drawing = false;
                holding = false;
            }
            MainBus.Instance.PublishEvent(new HUDSetIcon(weapon.gameObject.name, "org"));
        }
    }
}