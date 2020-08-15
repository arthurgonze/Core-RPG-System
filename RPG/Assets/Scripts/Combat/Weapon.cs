using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private float _weaponDamage = 10f;
        [SerializeField] private GameObject _equippedWeaponPrefab = null;
        [SerializeField] private AnimatorOverrideController _animatorOverride = null;

        public void Spawn(Transform handTransform, Animator animator)
        {
            if (_equippedWeaponPrefab != null) 
                Instantiate(_equippedWeaponPrefab, handTransform);
            if (_animatorOverride != null)
                animator.runtimeAnimatorController = _animatorOverride;
        }

        public float GetWeaponRange() { return _weaponRange; }
        public float GetWeaponDamage() { return _weaponDamage; }
    }
}
