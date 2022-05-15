using GameDevTV.Utils;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISavable
    {
        [SerializeField] TakeDamageEvent _takeDamage;
        [SerializeField] UnityEvent _onDie;   
        
        [System.Serializable] public class TakeDamageEvent : UnityEvent<float> {}

        private LazyValue<float> _healthPoints;
        private bool _die = false;

        private bool _isDead = false;
        BaseStats _baseStats = null;

        private void Awake()
        {
            _baseStats = GetComponent<BaseStats>();
            _healthPoints = new LazyValue<float>(getInitialHealth);
        }

        private void Start()
        {
            _healthPoints.ForceInit();
        }

        private void OnEnable()
        {
            if (_baseStats != null)
                _baseStats.onLevelUp += RegenFullLife;
        }

        private void OnDisable()
        {
            if (_baseStats != null)
                _baseStats.onLevelUp -= RegenFullLife;
        }

        public void Update()
        {
            if (_die) Die();
        }

        private float getInitialHealth()
        {
            return _baseStats.GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            _healthPoints.value = Mathf.Max(_healthPoints.value - damage, 0);
            _takeDamage.Invoke(damage);
            if (_healthPoints.value > 0) return;

            _onDie.Invoke();
            Die();
            GiveXP(instigator);
        }

        public void RegenFullLife()
        {
            _healthPoints.value = GetMaxHealthPoints();
        }

        public float GetCurrentHealthPoints()
        {
            return _healthPoints.value;
        }
        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return (_healthPoints.value / GetMaxHealthPoints());
        }

        private void Die()
        {
            if (_isDead) return;

            _isDead = true;
            this.GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<Collider>().enabled = false;
        }

        private void GiveXP(GameObject instigator)
        {
            instigator.GetComponent<Experience>()?.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public object CaptureState()
        {
            return this._healthPoints.value;
        }

        public void RestoreState(object state)
        {
            this._healthPoints.value = (float)state;
            if (this._healthPoints.value <= 0)
                Die();
        }
    }
}
