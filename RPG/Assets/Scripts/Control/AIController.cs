using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float dwellTime = 2f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1;
        [Range(0,1)][SerializeField] float patrolSpeedFraction = 0.2f;
        int currentWaypointIndex = 0;

        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;

        Vector3 guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceEnteringWaypoint = Mathf.Infinity;

        private void Start()
        {
            fighter = this.GetComponent<Fighter>();
            health = this.GetComponent<Health>();
            mover = this.GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
            guardPosition = this.transform.position;
        }

        private void Update()
        {
            if (health.IsDead()) return;

            if (DistanceToPlayer() < chaseDistance && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceEnteringWaypoint += Time.deltaTime;
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition;

            if(patrolPath != null)
            {
                if(AtWaypoint())
                {
                    CycleWaypoint();
                    timeSinceEnteringWaypoint = 0;
                }
                nextPosition = GetCurrentWaypoint();
            }
            if(timeSinceEnteringWaypoint > dwellTime)
            {
                mover.StartMovementAction(nextPosition, patrolSpeedFraction);
            }
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(this.transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private float DistanceToPlayer()
        {
            float distance = Vector3.Distance(this.transform.position, player.transform.position);
            return distance;
        }

        // Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, chaseDistance);
        }
    }
}
