using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phantom.Utility.MessageBus;

public class Sword : MonoBehaviour {

    private Vector3 lastPos;

    private float speed = 0f;
    private int coolDown = 0;

    [SerializeField]private GameObject hand;

    [HideInInspector]public GameObject target = null;

    // Use this for initialization
    private void OnEnable()
    {
        StartCoroutine(CalcVelocity());
    }

    void FixedUpdate()
    {
        if (coolDown > 0) {
            coolDown--;
        }
        if (target != null && speed >= 1f && coolDown <= 0) {
            coolDown = 20;
            MainBus.Instance.PublishEvent(new KillEnemy(target));
            target = null;
        }
    }


    IEnumerator CalcVelocity()
    {
        while (Application.isPlaying)
        {
            // Position at frame start
            lastPos = hand.transform.localPosition;
            // Wait till it the end of the frame
            yield return new WaitForEndOfFrame();
            // Calculate velocity: Velocity = DeltaPosition / DeltaTime
            speed = (lastPos - hand.transform.localPosition).magnitude / Time.deltaTime;
        }
    }
}
