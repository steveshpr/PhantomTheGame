using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Arrow : MonoBehaviour {

    [SerializeField]private GameObject LHand;
    [SerializeField]private GameObject refGroup;
    [SerializeField]private GameObject bow;

    private bool aimable;
    private bool aimming;
    private float strength;

    void Start () {
        aimable = false;
        aimming = false;
        strength = -1f;
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
                GameObject arrow = Instantiate(gameObject);
                arrow.GetComponent<Arrow>().fire(gameObject, strength);
                gameObject.SetActive(false);
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

    public void fire(GameObject origin, float strength) {
        transform.position = origin.transform.position;
        transform.forward = origin.transform.forward;
        Physics.IgnoreCollision(GetComponent<Collider>(), origin.GetComponent<Collider>());
        transform.localScale = origin.transform.lossyScale;
        GetComponent<Rigidbody>().isKinematic = false;
        //GetComponent<Collider>().isTrigger = false;
        GetComponent<Rigidbody>().AddForce(origin.GetComponent<Arrow>().refGroup.transform.forward * strength * 3000);
        GetComponent<FlyingArrow>().enabled = true;
        enabled = false;
    }
}
