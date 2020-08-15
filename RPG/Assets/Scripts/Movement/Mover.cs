using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISavable
    {
        // We can remove the serializeField, its on just for debug purposes
        [SerializeField] private Transform _target;
        [SerializeField] private float _maxSpeed = 6f;

        // Cached References
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private ActionScheduler _actionScheduler;

        // Move to Cursor
        private Ray _ray;
        private bool _hasHit = false;
        private RaycastHit _hit;

        // Update Animator
        private Vector3 _globalVelocity;
        private Vector3 _localVelocity;
        private float _speed;


        // Start is called before the first frame update
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateAnimator();
        }

        public bool MoveToCursor()
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _hasHit = Physics.Raycast(_ray, out _hit);
            if (_hasHit)
            {
                //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
                if (Input.GetMouseButton(0))
                {
                    StartMovementAction(_hit.point, 1f);
                }
                return true;
            }
            return false;
        }

        public void StartMovementAction(Vector3 destination, float speedFraction)
        {
            _actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            _navMeshAgent.SetDestination(destination);
            _navMeshAgent.speed = _maxSpeed * Mathf.Clamp01(speedFraction);
            _navMeshAgent.isStopped = false;
        }

        private void UpdateAnimator()
        {
            _globalVelocity = _navMeshAgent.velocity;
            _localVelocity = transform.InverseTransformDirection(_globalVelocity);
            _speed = _localVelocity.z;
            _animator.SetFloat("ForwardSpeed", _speed);
        }

        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }

        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)state;

            if (this.GetComponent<NavMeshAgent>() != null)
            {
                this.GetComponent<NavMeshAgent>().enabled = false;
                this.transform.position = ((SerializableVector3) data["position"]).ToVector();
                this.transform.eulerAngles = ((SerializableVector3) data["rotation"]).ToVector();
                this.GetComponent<NavMeshAgent>().enabled = true;
            }
            else
            {
                this.transform.position = ((SerializableVector3)data["position"]).ToVector();
                this.transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            }

            if (this.GetComponent<ActionScheduler>() != null)
            {
                this.GetComponent<ActionScheduler>().CancelCurrentAction();
            }
        }
    }
}
