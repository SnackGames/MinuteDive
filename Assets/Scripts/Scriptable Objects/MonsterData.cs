using UnityEngine;
using System.Collections.Generic;

namespace Data
{
  [CreateAssetMenu(fileName = "Data", menuName = "Data/Monster")]
  public class MonsterData : ScriptableObject
  {
    public string monsterName;
    public int monsterHP = 3;
    public int monsterDamage = 5;
    public float monsterMoveSpeed = 10.0f;
    public float monsterMoveAcceleration = 10.0f;
    public float monsterWaitTime = 1.0f;
    public List<DropData> monsterDropData;
  }
}