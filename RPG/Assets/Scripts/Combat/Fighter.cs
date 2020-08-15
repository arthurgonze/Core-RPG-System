using UnityEngine;
using RPG.Movement;
using RPG.Core;


namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [SerializeField] private float _weaponDamage = 10f;
        [SerializeField] private GameObject _weaponPrefab = null;
        [SerializeField] private Transform _rightHandTransform = null;

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
            SpawnWeapon();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            MoveToAttack();
        }

        private void SpawnWeapon()
        {
            Instantiate(_weaponPrefab, _rightHandTransform);
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
            return Vector3.Distance(transform.position, _target.transform.position) < _weaponRange;
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
            if (_target == null) return;
            _target.TakeDamage(_weaponDamage);
            //Debug.Log("Just a scratch");
            if (_target.IsDead())
            {
                Cancel();
            }
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }
    }
}
