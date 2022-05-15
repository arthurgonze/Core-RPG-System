using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private float _timeToFadeIn = .5f;

        private const string _defaultSaveFile = "save";

        // Cached reference
        private SavingSystem _savingSystem;

        private void Awake()
        {
            _savingSystem = GetComponent<SavingSystem>();
            // StartCoroutine(LoadLastScene());
        }

        private void Start()
        {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return _savingSystem.LoadLastScene(_defaultSaveFile);
            
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.Fade(0, _timeToFadeIn);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                Save();
            if (Input.GetKeyDown(KeyCode.L))
                Load();
            if (Input.GetKeyDown(KeyCode.Delete))
                DeleteSave();
        }

        public void Load()
        {
            _savingSystem.Load(_defaultSaveFile);
        }

        public void Save()
        {
            _savingSystem.Save(_defaultSaveFile);
        }

        public void DeleteSave()
        {
            _savingSystem.Delete(_defaultSaveFile);
        }
    }
}
