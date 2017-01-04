using System;
using UnityEngine;
using Phantom.Utility.MessageBus;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]

    public class AICharacterControl : MonoBehaviour, ISubscriber<SpottedEvent>
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        
        private float visionDistance;
        private float visionHeight;
        private Transform target;
        private CapsuleCollider mainCollider;
        private SphereCollider mainVision;
        private Array ragdoll;


        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

	        agent.updateRotation = false;
	        agent.updatePosition = true;

            
            mainCollider = GetComponent<CapsuleCollider>();
            mainVision = GetComponent<SphereCollider>();
            visionDistance = mainVision.radius;
            visionHeight = mainVision.center.y;

            ragdoll = GetComponentsInChildren<Collider>();
            foreach (Collider collider in ragdoll)
            {
                collider.enabled = false;
                collider.gameObject.GetComponent<Rigidbody>().useGravity = false;
            }

            mainCollider.enabled = true;
            mainVision.enabled = true;

            MainBus.Instance.Subscribe(this);
        }

        private void die() {

            

            Destroy(GetComponent("AICharacterControl"));
            Destroy(GetComponent("ThirdPersonCharacter"));
            foreach (var comp in gameObject.GetComponents<Component>())
            {
                if (!(comp is Transform))
                {
                    Destroy(comp);
                }
            }

            foreach (Collider collider in ragdoll)
            {
                collider.enabled = true;
                collider.gameObject.GetComponent<Rigidbody>().useGravity = true;
            }

        }

        
        private void Update()
        {
            
            if (target != null)
            {
                agent.SetDestination(target.position);
            }
            else
            {
                agent.SetDestination(transform.position);
            }

            if (agent.remainingDistance > agent.stoppingDistance)
                character.Move(agent.desiredVelocity, false, false);
            else
                character.Move(Vector3.zero, false, false);

            if (Input.GetKeyUp(KeyCode.End))
            {
                die();
            }
        }


        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void OnTriggerStay(Collider coll) {
            if (coll.gameObject.layer == 8) {

                Vector3 eyePosition = transform.position + Vector3.up * visionHeight;
                Vector3 rayDirection = (coll.transform.position - eyePosition);
                rayDirection.y = 0;
                rayDirection.Normalize();
                
                Vector3 leftLimit = (transform.forward - transform.right).normalized;
                Vector3 rightLimit = (transform.forward + transform.right).normalized;


                float angleLeft = Vector3.Dot(leftLimit.normalized, rayDirection.normalized);
                float angleRight = Vector3.Dot(rightLimit.normalized, rayDirection.normalized);

                bool inFov = angleLeft >= 0 && angleRight >= 0;

                //test rays
                /*
                Debug.DrawRay(eyePosition, leftLimit * visionDistance, Color.green);
                Debug.DrawRay(eyePosition, rightLimit * visionDistance, Color.green);
                if (inFov)
                {
                    Debug.DrawRay(eyePosition, rayDirection * visionDistance, Color.red);
                }
                else {
                    Debug.DrawRay(eyePosition, rayDirection * visionDistance, Color.blue);
                }*/
                //Debug.Log((transform.forward - rayDirection).normalized);
                //Debug.Log((transform.forward + transform.right).normalized);

                RaycastHit hit;
                int layerMask = 1 << 10;
                layerMask = ~layerMask;
                if (Physics.Raycast(eyePosition, rayDirection, out hit, visionDistance, layerMask)) {
                    if (inFov && hit.collider.gameObject.layer == 8) {
                        MainBus.Instance.PublishEvent(new SpottedEvent(coll.transform));
                    }
                    else{
                        MainBus.Instance.PublishEvent(new SpottedEvent(null));
                    }
                }
            }
        }

        public void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.layer == 8)
            {
                MainBus.Instance.PublishEvent(new SpottedEvent(null));
            }
        }

        public void OnEvent(SpottedEvent evt)
        {
            target = evt.targetTransform;
        }
    }
}
