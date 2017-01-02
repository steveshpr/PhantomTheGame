using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]

    public class AICharacterControl : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Transform target;                                    // target to aim for

        private Collider mainCollider;
        private Array ragdoll;


        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

	        agent.updateRotation = false;
	        agent.updatePosition = true;

            
            mainCollider = GetComponent<Collider>();

            ragdoll = GetComponentsInChildren<Collider>();
            foreach (Collider collider in ragdoll)
            {
                collider.enabled = false;
            }

            mainCollider.enabled = true;
            
        }

        private void die() {

            foreach (Collider collider in ragdoll)
            {
                collider.enabled = true;
            }
            mainCollider.enabled = false;

            Destroy(GetComponent("AICharacterControl"));
            Destroy(GetComponent("ThirdPersonCharacter"));
            foreach (var comp in gameObject.GetComponents<Component>())
            {
                if (!(comp is Transform))
                {
                    Destroy(comp);
                }
            }
        }


        private void Update()
        {
            if (target != null)
                agent.SetDestination(target.position);

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
    }
}
