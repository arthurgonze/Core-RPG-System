using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISavable, IModifierProvider
    {
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [SerializeField] private float _autoAttackRange = 3f;
        [SerializeField] private Transform _rightHandTransform = null;
        [SerializeField] private Transform _leftHandTransform = null;
        [SerializeField] private WeaponConfig _defaultWeaponConfig = null;
        

        private float _timeSinceLastAttack = Mathf.Infinity;
        private Health _target;
        private WeaponConfig _currentWeaponConfig;
        private LazyValue<Weapon> _currentWeapon;

        // Cached reference
        private ActionScheduler _actionScheduler;
        private Animator _animator;
        private Mover _mover;

        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();
            _mover = GetComponent<Mover>();

            _currentWeaponConfig = _defaultWeaponConfig;
            _currentWeapon = new LazyValue<Weapon>(GetInitialWeapon);
        }

        private void Start()
        {
            _currentWeapon.ForceInit();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            MoveToAttack();
        }

        private Weapon GetInitialWeapon()
        {
            return AttachWeapon(_defaultWeaponConfig);
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            Debug.Log("Equip Weapon");
            _currentWeaponConfig = weapon;
            _currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Debug.Log("Attach Weapon");
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(_rightHandTransform, _leftHandTransform, animator);
        }

        private void MoveToAttack()
        {
            if (_target == null) return;
            if (_target.IsDead()) 
            {
                _target = FindNewTargetInRange();
                if (_target == null) return;
            }


            if (!GetIsInRange(_target.transform))
                _mover.MoveTo(_target.transform.position, 1f);
            else
            {
                _mover.Cancel();
                AttackBehavior();
            }
        }

        private Health FindNewTargetInRange()
        {
            Health best = null;
            float bestDistance = Mathf.Infinity;
            foreach (var candidate in FindAllTargetsInRange())
            {
                float candidateDistance = Vector3.Distance(
                    transform.position, candidate.transform.position);
                if (candidateDistance < bestDistance)
                {
                    best = candidate;
                    bestDistance = candidateDistance;
                }
            }
            return best;
        }

        private IEnumerable<Health> FindAllTargetsInRange()
        {
            RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, _autoAttackRange, Vector3.up);
            foreach (var hit in raycastHits)
            {
                Health health = hit.transform.GetComponent<Health>();
                if (health == null) continue;
                if (health.IsDead()) continue;
                if (health.gameObject == gameObject) continue;
                yield return health;
            }
        }

        private void AttackBehavior()
        {
            transform.LookAt(_target.transform);
            if (_timeSinceLastAttack > _timeBetweenAttacks)
            {
                AttackBehaviour();
                _timeSinceLastAttack = 0;
            }
        }

        private void AttackBehaviour()
        {
            this.transform.LookAt(_target.transform);
            _animator.ResetTrigger("stopAttack");
            _animator.SetTrigger("attack");
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < _currentWeaponConfig.GetWeaponRange();
        }

        public void Attack(GameObject combatTarget)
        {
            if (combatTarget == null) return;
            _actionScheduler.StartAction(this);
            this._target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            _animator.ResetTrigger("attack");
            _animator.SetTrigger("stopAttack");
            _target = null;
            _mover.Cancel();
        }

        public IEnumerable<float> GetAddModifier(Stat stat)
        {
            if(stat == Stat.Damage)
                yield return _currentWeaponConfig.GetWeaponDamage();
        }

        public IEnumerable<float> GetPercentModifier(Stat stat)
        {
            if(stat == Stat.Damage)
                yield return _currentWeaponConfig.GetWeaponPercentageDamageBonus();
        }

        // Animation Event
        private void Hit()
        {
            if (_target == null) return;
            if (_target.IsDead())
                Cancel();

            if(_currentWeapon.value != null)
                _currentWeapon.value.OnHit();

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (_currentWeaponConfig.HasProjectile())
                _currentWeaponConfig.LaunchProjectile(_rightHandTransform, _leftHandTransform, _target, gameObject, damage);
            else
                _target.TakeDamage(gameObject, damage);
        }

        void Shoot()
        {
            Hit();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            if((!_mover.CanMoveTo(combatTarget.transform.position)) && (!GetIsInRange(combatTarget.transform))) return false;

            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public Health GetTarget()
        {
            return _target;
        }

        public object CaptureState() 
        {
            if(_currentWeaponConfig == null) return "Unarmed";
            return _currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
