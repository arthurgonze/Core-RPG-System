using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using System;
using GameDevTV.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspicionTime = 3f;
        [SerializeField] private float _dwellTime = 2f;
        [SerializeField] private PatrolPath _patrolPath;
        [SerializeField] private float _waypointTolerance = 1;
        [Range(0,1)][SerializeField] private float _patrolSpeedFraction = 0.2f;
        private int _currentWaypointIndex = 0;

        // Cached reference
        private Fighter _fighter;
        private GameObject _player;
        private Health _health;
        private Mover _mover;

        // Patrol Behavior
        private LazyValue<Vector3> _guardPosition;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceEnteringWaypoint = Mathf.Infinity;


        private void Awake()
        {
            _fighter = this.GetComponent<Fighter>();
            _health = this.GetComponent<Health>();
            _mover = this.GetComponent<Mover>();
            _player = GameObject.FindWithTag("Player");
            _guardPosition = new LazyValue<Vector3>(GetInitialGuardPosition);
        }

        private void Start()
        {
            _guardPosition.ForceInit();
        }

        private void Update()
        {
            if (_health.IsDead()) return;

            if (DistanceToPlayer() < _chaseDistance && _fighter.CanAttack(_player))
                AttackBehaviour();
            else if (_timeSinceLastSawPlayer < _suspicionTime)
                SuspicionBehaviour();
            else
                PatrolBehaviour();
            
            UpdateTimers();
        }

        private Vector3 GetInitialGuardPosition()
        {
            return this.transform.position;
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player);
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceEnteringWaypoint += Time.deltaTime;
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardPosition.value;

            if(_patrolPath != null)
            {
                if(AtWaypoint())
                {
                    CycleWaypoint();
                    _timeSinceEnteringWaypoint = 0;
                }
                nextPosition = GetCurrentWaypoint();
            }
            if(_timeSinceEnteringWaypoint > _dwellTime)
            {
                _mover.StartMovementAction(nextPosition, _patrolSpeedFraction);
            }
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(this.transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < _waypointTolerance;
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = _patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return _patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        private float DistanceToPlayer()
        {
            float distance = Vector3.Distance(this.transform.position, _player.transform.position);
            return distance;
        }

        // Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, _chaseDistance);
        }
    }
}
