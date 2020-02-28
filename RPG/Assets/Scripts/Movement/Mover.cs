using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        // We can remove the serializeField, its on just for debug purposes
        [SerializeField] private Transform target;

        // Cached References
        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private ActionScheduler actionScheduler;

        // Move to Cursor
        private Ray ray;
        private bool hasHit = false;
        private RaycastHit hit;

        // Update Animator
        private Vector3 globalVelocity;
        private Vector3 localVelocity;
        private float speed;


        // Start is called before the first frame update
        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateAnimator();
        }

        public bool MoveToCursor()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hasHit = Physics.Raycast(ray, out hit);
            if (hasHit)
            {
                //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
                if (Input.GetMouseButton(0))
                {
                    StartMovementAction(hit.point);
                }
                return true;
            }
            return false;
        }

        private void StartMovementAction(Vector3 destination)
        {
            actionScheduler.StartAction(this);
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            navMeshAgent.SetDestination(destination);
            navMeshAgent.isStopped = false;
        }

        private void UpdateAnimator()
        {
            globalVelocity = navMeshAgent.velocity;
            localVelocity = transform.InverseTransformDirection(globalVelocity);
            speed = localVelocity.z;
            animator.SetFloat("ForwardSpeed", speed);
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }
    }
}
