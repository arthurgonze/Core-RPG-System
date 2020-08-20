using UnityEngine;
using RPG.Movement;
using RPG.Core;


namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [SerializeField] private Transform _rightHandTransform = null;
        [SerializeField] private Transform _leftHandTransform = null;
        [SerializeField] private Weapon _defaultWeapon = null;
        [SerializeField] private Weapon _currentWeapon = null;

        private float _timeSinceLastAttack = Mathf.Infinity;
        private Health _target;


        // Cached reference
        private ActionScheduler _actionScheduler;
        private Animator _animator;
        private Mover _mover;

        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();
            _mover = GetComponent<Mover>();
        }

        private void Start()
        {
            EquipWeapon(_defaultWeapon);
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            MoveToAttack();
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (weapon == null) return;
            _currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(_rightHandTransform, _leftHandTransform, animator);
        }

        private void MoveToAttack()
        {
            if (_target == null) return;

            if (!GetIsInRange())
            {
                _mover.MoveTo(_target.transform.position, 1f);
            }
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
            return Vector3.Distance(transform.position, _target.transform.position) < _currentWeapon.GetWeaponRange();
        }

        public bool PlayerAttack()
        {
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
            foreach (RaycastHit hit in hits)
            {
                CombatTarget combatTarget = hit.transform.GetComponent<CombatTarget>();
                if (combatTarget == null) continue;

                if (!CanAttack(combatTarget.gameObject)) continue;

                if (Input.GetMouseButton(0))
                {
                    Attack(combatTarget.gameObject);
                }
                return true;
            }
            return false;
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

        // Animation Event
        private void Hit()
        {
            if (_target.IsDead())
                Cancel();

            if (_target == null) return;

            if (_currentWeapon.HasProjectile())
                _currentWeapon.LaunchProjectile(_rightHandTransform, _leftHandTransform, _target);
            else
                _target.TakeDamage(_currentWeapon.GetWeaponDamage());
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
    }
}
