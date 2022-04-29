using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject 
    {
        [SerializeField] ProgressionCharacterClass[] _characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> _lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();
            float[] levels = _lookupTable[characterClass][stat];   
            if(levels.Length < level) 
                return levels[levels.Length];
            return levels[level-1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();
            float[] levels = _lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookup()
        {
            if(_lookupTable != null) return;

            _lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
            foreach (ProgressionCharacterClass character in _characterClasses)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();

                foreach(ProgressionStat char_stat in character.stats)
                    statLookupTable[char_stat.stat] = char_stat.levels;
                _lookupTable[character.characterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}
