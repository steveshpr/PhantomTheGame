using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phantom.Utility.MessageBus;

public class FlyingArrow : MonoBehaviour {

    private int frameDelay = 2;
    private bool active = false;
    
	void Start () {
	}
	
	void FixedUpdate () {
        if (frameDelay > 0) {
            frameDelay--;
            return;
        }
        if (!active)
        {
            active = true;
            gameObject.GetComponent<Collider>().isTrigger = false;
            gameObject.layer = 13;
        }
        if (transform.position.y >= 200 || transform.position.y <= -100) {
            Destroy(gameObject);
        }

    }

    private void OnCollisionStay(Collision col)
    {
        if (col.gameObject.layer != 12 && col.gameObject.layer !=8) {
            if (active)
            {
                var rigidBody = gameObject.GetComponent<Rigidbody>();
                active = false;
                rigidBody.angularVelocity = Vector3.zero;
                rigidBody.velocity = Vector3.zero;
                rigidBody.isKinematic = true;
                if (col.gameObject.layer == 10)
                {
                    MainBus.Instance.PublishEvent(new KillEnemy(col.gameObject, transform));
                }
                if (col.gameObject.layer == 9) {
                    Debug.Log("wall");
                }
                enabled = false;
            }
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer != 12 && col.gameObject.layer != 8)
        {
            if (active)
            {
                var rigidBody = gameObject.GetComponent<Rigidbody>();
                active = false;
                rigidBody.angularVelocity = Vector3.zero;
                rigidBody.velocity = Vector3.zero;
                rigidBody.isKinematic = true;
                if (col.gameObject.layer == 10)
                {
                    MainBus.Instance.PublishEvent(new KillEnemy(col.gameObject, transform));
                }
                if (col.gameObject.layer == 9)
                {
                    Debug.Log("wall");
                }
                enabled = false;
            }
        }
    }
}
