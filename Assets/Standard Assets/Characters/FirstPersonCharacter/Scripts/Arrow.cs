using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Arrow : MonoBehaviour {

    [SerializeField]private GameObject LHand;
    [SerializeField]private GameObject refGroup;
    [SerializeField]private GameObject bow;

    private bool aimable = false;
    private bool aimming = false;
    private float strength = -1f;

    void Start () {
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        aimming = CrossPlatformInputManager.GetAxis("InteractR") > 0.4f;
        if (aimable && aimming && bow.activeSelf)
        {
            Vector3 dir = (LHand.transform.position - refGroup.transform.position).normalized;
            refGroup.transform.forward = dir;
            strength = (LHand.transform.position - refGroup.transform.position).magnitude;
        }

        if (aimable && !aimming && bow.activeSelf) {
            if (strength >= 0.3f) {
                Debug.Log("fire! stength: " + strength);
            }
            strength = -1f;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(LHand)) {
            aimable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(LHand))
        {
            aimable = false;
        }
    }
}
