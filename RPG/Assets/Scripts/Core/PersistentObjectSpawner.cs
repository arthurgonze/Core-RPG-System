using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _persistentObjectPrefab;
        private static bool _hasSpawned = false;

        private void Awake()
        {
            if (_hasSpawned) return;
            
            SpawnPersistentObjects();

            _hasSpawned = true;
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = Instantiate(_persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}
