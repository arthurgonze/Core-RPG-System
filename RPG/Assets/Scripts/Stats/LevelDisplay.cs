using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats _baseStats;

        Text _levelValueText;

        private void Awake()
        {

            _levelValueText = GetComponent<Text>();
            _baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        // private void Start()
        // {
        //     _baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        // }

        private void Update()
        {
            _levelValueText.text = _baseStats.GetLevel().ToString();
        }
    }
}