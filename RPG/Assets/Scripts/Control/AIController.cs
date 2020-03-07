using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 2f;

        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;

        Vector3 guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;

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
                timeSinceLastSawPlayer = 0;
                fighter.Attack(player);
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
            else
            {
                mover.MoveTo(guardPosition);
            }

            timeSinceLastSawPlayer += Time.deltaTime;
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
