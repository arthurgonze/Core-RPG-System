using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _projectileSpeed = 1f;
        [SerializeField] private bool _isHoming = false;
        [SerializeField] private float _targetPositionOffset = 0.1f;
        [SerializeField] private float _lifetime = 1f;
        private float _damage = 0;
        private Health _target = null;
        private Vector3 _initialTargetPosition;
        private float _timeSinceLaunch = Mathf.Infinity;

        // Update is called once per frame
        void Update()
        {
            _timeSinceLaunch += Time.deltaTime;
            if (_target == null) return;
            if(IsInTargetsPosition() || (_timeSinceLaunch > _lifetime))
                Destroy(gameObject);
            ProjectileBehavior();
        }

        private bool IsInTargetsPosition()
        {
            return Vector3.Distance(transform.position, _target.transform.position) < _targetPositionOffset;
        }

        public void SetTarget(Health target, float damage)
        {
            this._target = target;
            this._damage = damage;
            _initialTargetPosition = GetAimLocation();
            _timeSinceLaunch = 0;
        }

        private void ProjectileBehavior()
        {
            if(_isHoming) transform.LookAt(GetAimLocation());
            else transform.LookAt(_initialTargetPosition);
            transform.Translate(Vector3.forward * Time.deltaTime * _projectileSpeed);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCollider = _target.GetComponent<CapsuleCollider>();
            if (targetCollider != null && targetCollider.enabled)
            {
                return _target.transform.position + (Vector3.up * (targetCollider.height / 2));
            }
           
            return _target.transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == _target.gameObject)
            {
                _target.TakeDamage(_damage);
            }

            Destroy(gameObject);
        }
    }
}
