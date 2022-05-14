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
        [SerializeField] private Transform _rightHandTransform = null;
        [SerializeField] private Transform _leftHandTransform = null;
        [SerializeField] private Weapon _defaultWeapon = null;
        

        private float _timeSinceLastAttack = Mathf.Infinity;
        private Health _target;
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
            _currentWeapon = new LazyValue<Weapon>(GetInitialWeapon);
        }

        private void Start()
        {
            _currentWeapon.ForceInit();
            // if(_currentWeapon.value == null)
            //     EquipWeapon(_defaultWeapon);
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            MoveToAttack();
        }

        private Weapon GetInitialWeapon()
        {
            AttachWeapon(_defaultWeapon);
            return _defaultWeapon;
        }

        public void EquipWeapon(Weapon weapon)
        {
            _currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(_rightHandTransform, _leftHandTransform, animator);
        }

        private void MoveToAttack()
        {
            if (_target == null) return;

            if (!GetIsInRange())
                _mover.MoveTo(_target.transform.position, 1f);
            else
            {
                _mover.Cancel();

                if (_timeSinceLastAttack > _timeBetweenAttacks)
                {
                    AttackBehaviour();
                    _timeSinceLastAttack = 0;
                }
            }
        }

        private void AttackBehaviour()
        {
            this.transform.LookAt(_target.transform);
            _animator.ResetTrigger("stopAttack");
            _animator.SetTrigger("attack");
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) < _currentWeapon.value.GetWeaponRange();
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
                yield return _currentWeapon.value.GetWeaponDamage();
        }

        public IEnumerable<float> GetPercentModifier(Stat stat)
        {
            if(stat == Stat.Damage)
                yield return _currentWeapon.value.GetWeaponPercentageDamageBonus();
        }

        // Animation Event
        private void Hit()
        {
            if (_target.IsDead())
                Cancel();

            if (_target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (_currentWeapon.value.HasProjectile())
                _currentWeapon.value.LaunchProjectile(_rightHandTransform, _leftHandTransform, _target, gameObject, damage);
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
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public Health GetTarget()
        {
            return _target;
        }

        public object CaptureState() 
        {
            if(_currentWeapon.value == null) return "Unarmed";
            return _currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
