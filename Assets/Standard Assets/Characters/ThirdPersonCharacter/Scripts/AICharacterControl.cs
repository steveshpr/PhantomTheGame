using System;
using UnityEngine;
using Phantom.Utility.MessageBus;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]

    public class AICharacterControl : MonoBehaviour, ISubscriber<SpottedEvent>, ISubscriber<ChokeEnemy>, ISubscriber<LostSightEvent>, ISubscriber<KillEnemy>
    {
        public NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public float walkRadius;
        public bool isStatic;


        private float visionDistance;
        private float visionHeight;
        private Transform target;
        private CapsuleCollider mainCollider;
        private SphereCollider mainVision;
        private Array ragdoll;

        private bool spotted = false;


        private void Start()
        {

            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<NavMeshAgent>();
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
            MainBus.Instance.PublishEvent(new LostSightEvent());

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
                collider.gameObject.layer = 11;
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
                if(!isStatic)
                if (agent.remainingDistance < 1f)
                {
                    //Debug.Log("paling");
                    Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;
                    randomDirection += transform.position;
                    NavMeshHit hit;
                    NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
                    Vector3 finalPosition = hit.position;
                    agent.SetDestination(finalPosition);
                }
            }

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                if (target != null)
                {
                    character.Move(agent.desiredVelocity, true, false, false);
                }
                else {
                    character.Move(agent.desiredVelocity, false, false, false);
                }
            }
            else{
                character.Move(Vector3.zero,false, false, false);
            }
            
                

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
            if (coll.gameObject.layer == 8 || coll.gameObject.layer == 11) {

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
                
                Debug.DrawRay(eyePosition, leftLimit * visionDistance, Color.green);
                Debug.DrawRay(eyePosition, rightLimit * visionDistance, Color.green);
                if (inFov)
                {
                    Debug.DrawRay(eyePosition, rayDirection * visionDistance, Color.red);
                }
                else {
                    Debug.DrawRay(eyePosition, rayDirection * visionDistance, Color.blue);
                }

                if (inFov) {
                    RaycastHit hit;
                    int layerMask = 1 << 10;
                    layerMask = ~layerMask;
                    if (Physics.Raycast(eyePosition, rayDirection, out hit, visionDistance, layerMask))
                    {
                        if (hit.collider.gameObject.layer == 8)
                        {
                            spotted = true;
                            MainBus.Instance.PublishEvent(new SpottedEvent(coll.transform));
                        }
                    }
                    else if (!spotted){
                        spotted = true;
                        MainBus.Instance.PublishEvent(new SpottedEvent(coll.transform));
                    }
                }
                else
                {
                    spotted = false;
                    MainBus.Instance.PublishEvent(new LostSightEvent());
                }
                //}
            }
        }

        public void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.layer == 8)
            {
                spotted = false;
                MainBus.Instance.PublishEvent(new LostSightEvent());
            }
        }

        public void OnEvent(SpottedEvent evt)
        {
            target = evt.targetTransform;
        }

        public void OnEvent(ChokeEnemy evt)
        {
            if (evt.target.name == name) {
                if (!spotted)
                {
                    die();
                }
            }
        }

        public void OnEvent(KillEnemy evt)
        {
            if (evt.target.name == name)
            {
                if (!spotted)
                {
                    die();
                }
            }
        }

        public void OnEvent(LostSightEvent evt)
        {
            if (spotted)
            {
                MainBus.Instance.PublishEvent(new SpottedEvent(target));
            }
            else
            {
                target = null;
            }
        }
    }
}
