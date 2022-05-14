using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText _textPrefab = null;

        public void Spawn(float damage)
        {
            DamageText inGameText = Instantiate<DamageText>(_textPrefab, transform);
            inGameText.SetValue(damage);
        }
    }
}
