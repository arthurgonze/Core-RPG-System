using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Health _health;

        Text _healthValueText;
        Fighter _player;

        private void Awake()
        {
            _healthValueText = GetComponent<Text>();
            _player = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }
        
        // private void Start()
        // {
        //     _player = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        // }

        private void Update()
        {
            _health = _player.GetTarget();
            if (_health != null)
                _healthValueText.text = string.Format("{0:0} | {1:0}", _health.GetCurrentHealthPoints(), _health.GetMaxHealthPoints()); // _healthValueText.text = string.Format("{0:0}%", _health.GetPercentage());
            else
                _healthValueText.text = " ";
        }
    }
}