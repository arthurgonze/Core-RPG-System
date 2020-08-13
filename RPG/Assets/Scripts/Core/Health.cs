using RPG.Saving;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISavable
    {
        [SerializeField] private float _healthPoints = 100f;

        private bool _isDead = false;

        public void TakeDamage(float damage)
        {
            _healthPoints = Mathf.Max(_healthPoints - damage, 0);
            if (_healthPoints <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (_isDead) return;

            _isDead = true;
            this.GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public object CaptureState()
        {
            return this._healthPoints;
        }

        public void RestoreState(object state)
        {
            this._healthPoints = (float) state;
            if (this._healthPoints <= 0)
            {
                Die();
            }
        }
    }
}
