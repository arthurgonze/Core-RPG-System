using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private float _timeToFadeIn = .5f; 

        private const string DefaultSaveFile = "save";
        private SavingSystem _savingSystem;

        private void Awake()
        {
            _savingSystem = GetComponent<SavingSystem>();
        }

        private IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            // fade out
            fader.FadeOutImmediate();
            yield return _savingSystem.LoadLastScene(DefaultSaveFile);
            // fade in
            yield return fader.FadeIn(_timeToFadeIn);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
        }

        public void Load()
        {
            _savingSystem.Load(DefaultSaveFile);
        }

        public void Save()
        {
            _savingSystem.Save(DefaultSaveFile);
        }
    }
}
