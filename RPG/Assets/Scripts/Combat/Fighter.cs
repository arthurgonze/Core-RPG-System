using UnityEngine;
using RPG.Movement;
using RPG.Core;


namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private float weaponDamage = 10f;

        private float timeSinceLastAttack = Mathf.Infinity;

        private Health target;

        private ActionScheduler actionScheduler;
        private Animator animator;
        private Mover mover;
        void Start()
        {
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            mover = GetComponent<Mover>();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            MoveToAttack();
        }

        private void MoveToAttack()
        {
            if (target == null) return;
            
            if (!GetIsInRange())
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();

                if (timeSinceLastAttack > timeBetweenAttacks)
                {
                    AttackBehaviour();
                    timeSinceLastAttack = 0;
                }
            }
        }

        private void AttackBehaviour()
        {
            this.transform.LookAt(target.transform);
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
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
            actionScheduler.StartAction(this);
            this.target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
            target = null;
        }

        // Animation Event
        void Hit()
        {
            if (target == null) return;
            target.TakeDamage(weaponDamage);
            Debug.Log("Just a scratch");
            if (target.IsDead())
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
