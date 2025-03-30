using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

[Serializable]
public enum StatType
{
  None,

  // 적에게 가하는 피해량
  Attack,

  // 피해 입을 시 줄어드는 시간 감소
  Defense,

  // 게임 시작 시 주어지는 시간
  InitialRemainTime,
}

[Serializable]
public class StatModifier
{
  public StatType type = StatType.None;
  public float value = 0;
  private static Dictionary<StatType, string> statTypeToString = new Dictionary<StatType, string>()
  {
    {StatType.Attack,             "공격력"},
    {StatType.Defense,            "방어력"},
    {StatType.InitialRemainTime,  "추가 시간"},
  };

  public string GetModifierString()
  {
    return statTypeToString[type] + " + " + value;
  }
}

public class PlayerStat : MonoBehaviour
{
  public Data.PlayerBaseStatData baseStat;
  private Dictionary<StatType, float> currentStat = new Dictionary<StatType, float>();

  void Start()
  {
    InitializeStat();
  }

  public void InitializeStat()
  {
    foreach (StatType type in Enum.GetValues(typeof(StatType)))
    {
      currentStat[type] = 0;
    }

    if (baseStat != null)
    {
      foreach (StatModifier modifier in baseStat.statModifiers)
      {
        currentStat[modifier.type] += modifier.value;
      }
    }
  }

  public void ModifyStatValue(StatModifier modifier, bool Adding)
  {
    if (Adding)
      currentStat[modifier.type] += modifier.value;
    else
      currentStat[modifier.type] -= modifier.value;
  }

  public void ModifyStatValues(List<StatModifier> modifiers, bool Adding)
  {
    foreach (StatModifier modifier in modifiers)
    {
      ModifyStatValue(modifier, Adding);
    }
  }

  public float GetStatValue(StatType type)
  {
    if(currentStat.ContainsKey(type))
      return currentStat[type];
    return 0.0f;
  }
}
