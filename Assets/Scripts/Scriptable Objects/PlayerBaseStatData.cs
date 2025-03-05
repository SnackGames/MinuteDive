using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
  [CreateAssetMenu(fileName = "Data", menuName = "Data/PlayerBaseStat")]
  public class PlayerBaseStatData : ScriptableObject
  {
    public List<StatModifier> statModifiers;
  }
}