using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using RPG.Core;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 1;
        [SerializeField] private bool _isHoming = true;
        [SerializeField] private GameObject _hitEffect = null;
        [SerializeField] private float _maxLifetime = 5f;
        [SerializeField] private float _lifeAfterImpact = 2f;
        [SerializeField] private GameObject[] _destroyOnHit = null;

        private Health _target = null;
        private GameObject _instigator = null;
        private float _damage = 0;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (_target == null) return;
            if (_isHoming && !_target.IsDead())
                transform.LookAt(GetAimLocation());
            
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this._target = target;
            this._damage = damage;
            this._instigator = instigator;

            Destroy(gameObject, _maxLifetime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return _target.transform.position;
            }
            return _target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != _target) return;
            if (_target.IsDead()) return;
            _target.TakeDamage(_instigator, _damage);

            _speed = 0;

            if (_hitEffect != null)
            {
                Instantiate(_hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in _destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, _lifeAfterImpact);
        }
    }
}
