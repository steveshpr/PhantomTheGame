using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWeapon : MonoBehaviour {

    [SerializeField]private GameObject hand;
    [SerializeField]private GameObject weapon;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(hand)) {
            weapon.SetActive(!weapon.activeSelf);
        }
    }
}