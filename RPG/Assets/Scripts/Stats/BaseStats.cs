using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)][SerializeField] private int _startingLevel = 1;
        [SerializeField] private CharacterClass _characterClass;
        [SerializeField] private Progression _progression = null;
        [SerializeField] private GameObject _levelUpParticleEffect = null;
        [SerializeField] bool _useModifiers = false;

        private LazyValue<int> _currentLevel;
        private Experience _experience;


        public event Action onLevelUp;


        private void Awake()
        {
            _experience = GetComponent<Experience>();
            _currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            _currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (_experience != null)
                _experience.onExperienceGained += UpdateLevel;
        }

        private void OnDisable()
        {
            if (_experience != null)
                _experience.onExperienceGained -= UpdateLevel;
        }

        public float GetStat(Stat stat)
        {
            float baseStat = _progression.GetStat(stat, _characterClass, GetLevel());
            return (baseStat + GetAddMod(stat)) * (1 + GetPercentMod(stat) / 100);
        }

        private float GetAddMod(Stat stat)
        {
            if (!_useModifiers) return 0;

            float sum = 0;
            foreach (var provider in GetComponents<IModifierProvider>())
                foreach (var modifier in provider.GetAddModifier(stat))
                    sum += modifier;
            return sum;
        }

        private float GetPercentMod(Stat stat)
        {
            if (!_useModifiers) return 0;

            float sum = 0;
            foreach (var provider in GetComponents<IModifierProvider>())
                foreach (var modifier in provider.GetPercentModifier(stat))
                    sum += modifier;
            return sum;
        }

        public int GetLevel()
        {
            return _currentLevel.value;
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > _currentLevel.value)
            {
                _currentLevel.value = newLevel;
                onLevelUp();
                LevelUpEffect();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(_levelUpParticleEffect, transform);
        }

        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();

            if (experience == null) return _startingLevel;

            float currentXP = experience.GetExperience();
            float xpToLevelUp;
            int penultimateLevel = _progression.GetLevels(Stat.ExperienceToLevelUp, _characterClass);

            for (int level = 1; level <= penultimateLevel; level++)
            {
                xpToLevelUp = _progression.GetStat(Stat.ExperienceToLevelUp, _characterClass, level);
                if (xpToLevelUp > currentXP)
                    return level;
            }

            return penultimateLevel + 1;
        }
    }
}
