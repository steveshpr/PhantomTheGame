using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phantom.Utility.MessageBus;
using UnityStandardAssets.CrossPlatformInput;

public class LHDrawWeapon : MonoBehaviour {

    [SerializeField]private GameObject hand;
    [SerializeField]private GameObject weapon;

    private bool drawing = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (drawing) {
            if (CrossPlatformInputManager.GetAxis("HoldL") > 0.4f){
                weapon.SetActive(true);
            }
            else{
                weapon.SetActive(false);
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(hand)) {
            MainBus.Instance.PublishEvent(new HUDSetText(weapon.gameObject.name));
            drawing = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.Equals(hand))
        {
            MainBus.Instance.PublishEvent(new HUDSetText(""));
            drawing = false;
        }
    }
}