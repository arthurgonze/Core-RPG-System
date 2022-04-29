using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience _experience;

        Text _experienceValueText;

        private void Awake()
        {
            _experienceValueText = GetComponent<Text>();
            _experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        // private void Start()
        // {
        //     _experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        // }

        private void Update()
        {
            _experienceValueText.text = _experience.GetExperience().ToString();
        }
    }
}