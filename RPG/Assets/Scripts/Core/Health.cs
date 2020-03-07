using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float healthPoints = 100f;

        private Animator animator;

        bool isDead = false;

        private void Start()
        {
            animator = this.GetComponent<Animator>();
        }
        public void TakeDamage(float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            Debug.Log(healthPoints);
            if (healthPoints <= 0 && !isDead)
            {
                Die();
            }
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            animator.SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            if (this.GetComponent<NavMeshAgent>()) { this.GetComponent<NavMeshAgent>().enabled = false; }
            //if (this.GetComponent<Collider>()) { this.GetComponent<Collider>().enabled = false; }
        }

        public bool IsDead()
        {
            return isDead;
        }
    }
}
