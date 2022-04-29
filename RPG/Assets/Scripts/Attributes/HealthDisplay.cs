using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health _health;

        Text _healthValueText;

        private void Awake()
        {
            _healthValueText = GetComponent<Text>();
            _health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        // private void Start()
        // {
        //     _health = GameObject.FindWithTag("Player").GetComponent<Health>();
        // }

        private void Update()
        {
            _healthValueText.text = string.Format("{0:0} | {1:0}", _health.GetCurrentHealthPoints(), _health.GetMaxHealthPoints());
        }
    }
}