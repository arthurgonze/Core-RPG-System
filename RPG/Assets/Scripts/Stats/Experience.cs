using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISavable
    {
        [SerializeField] float _experiencePoints = 0;

        public event Action onExperienceGained;

        public void GainExperience(float experience)
        {
            _experiencePoints += experience;
            onExperienceGained();
        }

        public float GetExperience()
        {
            return _experiencePoints;
        }

        public object CaptureState()
        {
             return _experiencePoints;
        }
        public void RestoreState(object state)
        {
            _experiencePoints = (float)state;
        }
    }
}
