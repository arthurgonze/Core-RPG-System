using UnityEngine;
using RPG.Movement;
using RPG.Core;


namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float timeBetweenAttacks = 1f;

        private float timeSinceLastAttack = 0;

        private Transform target;
        private ActionScheduler actionScheduler;
        private Animator animator;
        void Start()
        {
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
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
                GetComponent<Mover>().MoveTo(target.position);
            }
            else
            {
                GetComponent<Mover>().Cancel();

                if (timeSinceLastAttack > timeBetweenAttacks)
                {
                    AttackBehaviour();
                    timeSinceLastAttack = 0;
                }
            }
        }

        private void AttackBehaviour()
        {
            animator.SetTrigger("attack");
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.position) < weaponRange;
        }

        public bool PlayerAttack()
        {
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;

                if (Input.GetMouseButtonDown(0))
                {
                    Attack(target);
                }
                return true;
            }
            return false;
        }

        private void Attack(CombatTarget combatTarget)
        {
            actionScheduler.StartAction(this);
            this.target = combatTarget.transform;
        }

        public void Cancel()
        {
            target = null;
        }

        // Animation Event
        void Hit()
        {
            Debug.Log("Just a scratch");
        }
    }
}
