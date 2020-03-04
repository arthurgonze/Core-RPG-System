using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
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
            if(healthPoints <= 0 && !isDead)
            {
                animator.SetTrigger("die");

                Collider collider = this.GetComponent<Collider>();
                if (collider)
                {
                    collider.enabled = false;
                }

                isDead = true;
            }
        }
        public bool IsDead()
        {
            return isDead;
        }
    }
}
