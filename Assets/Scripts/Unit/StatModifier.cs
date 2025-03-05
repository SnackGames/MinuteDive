using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum StatType
{
  None,

  // 공격력: 적에게 가하는 피해량
  Attack,

  // 방어력: 피해 입을 시 줄어드는 시간 감소
  Defense,
}

[Serializable]
public class StatModifier
{
  public StatType type = StatType.None;
  public float value = 0;
}
