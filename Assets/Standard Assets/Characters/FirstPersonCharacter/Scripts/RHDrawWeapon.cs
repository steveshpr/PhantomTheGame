using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phantom.Utility.MessageBus;
using UnityStandardAssets.CrossPlatformInput;

public class RHDrawWeapon : MonoBehaviour
{

    [SerializeField]private GameObject hand;
    [SerializeField]private GameObject weapon;
    [SerializeField]private GameObject counterWeapon;
    [SerializeField]private int count;

    private bool drawing = false;
    private bool holding = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (drawing)
        {
            if (CrossPlatformInputManager.GetAxis("HoldR") > 0.4f)
            {
                if (!holding && count > 0){
                    holding = true;
                    weapon.SetActive(true);
                    count--;
                }
            }
            else
            {
                if (holding){
                    holding = false;
                    weapon.SetActive(false);
                    count++;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(hand) && !counterWeapon.activeSelf)
        {
            MainBus.Instance.PublishEvent(new HUDSetText(weapon.gameObject.name));
            drawing = true;
            if (weapon.activeSelf) {
                holding = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(hand) && !counterWeapon.activeSelf)
        {
            MainBus.Instance.PublishEvent(new HUDSetText(""));
            drawing = false;
            holding = false;
        }
    }
}