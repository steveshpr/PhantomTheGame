using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phantom.Utility.MessageBus;

public class FlyingArrow : MonoBehaviour {

    private int frameDelay = 2;
    private bool active = true;
    
	void Start () {
	}
	
	void FixedUpdate () {
        if (frameDelay > 0) {
            frameDelay--;
            return;
        }
        gameObject.GetComponent<Collider>().isTrigger = false;
        gameObject.layer = 9;
	}

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer != 8 && col.gameObject.layer !=9) {
            if (active)
            {
                active = false;
                MainBus.Instance.PublishEvent(new KillEnemy(col.gameObject));
                enabled = false;
            }
        }
    }
}
