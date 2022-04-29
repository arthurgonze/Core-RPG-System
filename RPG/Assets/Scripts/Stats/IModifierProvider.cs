using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetAddModifier(Stat stat);
        IEnumerable<float> GetPercentModifier(Stat stat);
    }
}
