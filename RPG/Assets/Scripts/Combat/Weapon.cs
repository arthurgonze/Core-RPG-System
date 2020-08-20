using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private float _weaponDamage = 10f;
        [SerializeField] private bool _isRightHand = true;
        [SerializeField] private GameObject _equippedWeaponPrefab = null;
        [SerializeField] private Projectile _projectile = null;
        [SerializeField] private AnimatorOverrideController _animatorOverride = null;
        private const string weaponName = "Weapon"; 

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            if (_equippedWeaponPrefab != null)
            {
                GameObject weapon = Instantiate(_equippedWeaponPrefab, GetHandTransform(rightHand, leftHand));
                weapon.name = weaponName;
            }
            
            if (_animatorOverride != null)
                animator.runtimeAnimatorController = _animatorOverride;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            return _isRightHand ? rightHand : leftHand;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            Projectile projectileInstance = Instantiate(_projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, _weaponDamage);
        }
        public bool HasProjectile() { return _projectile != null; }
        public float GetWeaponRange() { return _weaponRange; }
        public float GetWeaponDamage() { return _weaponDamage; }
    }
}
