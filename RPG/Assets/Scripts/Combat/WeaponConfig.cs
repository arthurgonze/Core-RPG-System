using RPG.Core;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private float _weaponDamage = 10f;
        [SerializeField] private float _weaponPercentageDamage = 10;
        [SerializeField] private bool _isRightHand = true;
        [SerializeField] private Weapon _equippedWeaponPrefab = null;
        [SerializeField] private Projectile _projectile = null;
        [SerializeField] private AnimatorOverrideController _animatorOverride = null;
        private const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            Debug.Log("Spawnando arma");
            Weapon weapon = null;
            if (_equippedWeaponPrefab != null)
            {
                weapon = Instantiate(_equippedWeaponPrefab, GetHandTransform(rightHand, leftHand));
                weapon.gameObject.name = weaponName;
                Debug.Log("Arma Instanciada");
            }

            var overrideController =  animator.runtimeAnimatorController as AnimatorOverrideController;
            if (_animatorOverride != null)
                animator.runtimeAnimatorController = _animatorOverride;
            else if(overrideController != null)
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
                oldWeapon = leftHand.Find(weaponName);
            
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            return _isRightHand ? rightHand : leftHand;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float damage)
        {
            Projectile projectileInstance = Instantiate(_projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            // Launch Projectile Audio
            projectileInstance.SetTarget(target, instigator, damage);
        }
        public bool HasProjectile() { return _projectile != null; }
        public float GetWeaponRange() { return _weaponRange; }
        public float GetWeaponDamage() { return _weaponDamage; }
        public float GetWeaponPercentageDamageBonus() { return _weaponPercentageDamage; }
    }
}
