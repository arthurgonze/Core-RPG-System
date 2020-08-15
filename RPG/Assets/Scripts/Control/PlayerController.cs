using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        // Cached Reference
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;

        // Start is called before the first frame update
        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (_health.IsDead()) return;
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
        }

        private bool InteractWithCombat()
        {
            return _fighter.PlayerAttack();
        }

        private bool InteractWithMovement()
        {
            return _mover.MoveToCursor();
        }
    }
}
