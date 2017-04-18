using System;
using System.Collections;
using UnityEngine;
using Phantom.Utility.MessageBus;
using UnityStandardAssets.CrossPlatformInput;

namespace Phantom.Utility
{
    public class RigidbodyDragger : MonoBehaviour
    {
        const float k_Spring = 500.0f;
        const float k_Damper = 1.0f;
        const float k_Drag = 10.0f;
        const float k_AngularDrag = 5.0f;
        const float k_Distance = 0.0002f;
        const float dragRange = 5f;
        const bool k_AttachToCenterOfMass = false;

        [HideInInspector]public SpringJoint m_SpringJoint;
        [HideInInspector]public GameObject target;

        private void Update()
        {
            // Make sure the user pressed "e" down
            if (CrossPlatformInputManager.GetAxis("InteractR") < 0.4f || m_SpringJoint)
            {
                return;
            }

            StartDragging();
        }

        private void StartDragging()
        {
            // We need to actually hit an object
            int layerMaskPlayer = 1 << 8;
            int layerMaskEnviroment = 1 << 9;
            int layerMask = layerMaskPlayer | layerMaskEnviroment;
            layerMask = ~layerMask;
            if (target == null)
            {
                //Debug.Log("not hit");
                return;
            }


            // We need to hit a rigidbody that is not kinematic
            if (!target.GetComponent<Rigidbody>() || target.GetComponent<Rigidbody>().isKinematic)
            {
                //check if target is arrow
                if (target.name == "arrow(Clone)")
                {
                    MainBus.Instance.PublishEvent(new PickArrowEvent(target));
                }
                return;
            }

            // check if the target is alive
            SphereCollider targetVision = target.GetComponent<SphereCollider>(); // for enemy with vision
            if (targetVision && targetVision.isTrigger)
            {
                //Debug.Log("alive?");
                //tell finding a alive thing
                MainBus.Instance.PublishEvent(new TryingToDragAlive(target));
                return;
            }

            if (!m_SpringJoint)
            {
                var go = new GameObject("Rigidbody dragger");
                Rigidbody body = go.AddComponent<Rigidbody>();
                m_SpringJoint = go.AddComponent<SpringJoint>();
                body.isKinematic = true;
            }

            m_SpringJoint.transform.position = target.transform.position;
            m_SpringJoint.anchor = Vector3.zero;

            m_SpringJoint.spring = k_Spring;
            m_SpringJoint.damper = k_Damper;
            m_SpringJoint.maxDistance = k_Distance;
            m_SpringJoint.connectedBody = target.GetComponent<Rigidbody>();

            StartCoroutine("DragObject");
        }

        private IEnumerator DragObject()
        {
            var oldDrag = m_SpringJoint.connectedBody.drag;
            var oldAngularDrag = m_SpringJoint.connectedBody.angularDrag;
            var oldMass = m_SpringJoint.connectedBody.mass;
            m_SpringJoint.connectedBody.mass = 20.0f;
            m_SpringJoint.connectedBody.drag = k_Drag;
            m_SpringJoint.connectedBody.angularDrag = k_AngularDrag;
            while (CrossPlatformInputManager.GetAxis("InteractR") >= 0.4f)
            {
                m_SpringJoint.transform.position = transform.position;
                yield return null;
            }
            if (m_SpringJoint.connectedBody)
            {
                m_SpringJoint.connectedBody.drag = oldDrag;
                m_SpringJoint.connectedBody.angularDrag = oldAngularDrag;
                m_SpringJoint.connectedBody.mass = oldMass;
                m_SpringJoint.connectedBody = null;
                Destroy(m_SpringJoint.gameObject);
            }
        }
    }
}