using UnityEngine;

namespace Data
{
  [CreateAssetMenu(fileName = "Data", menuName = "Data/Monster")]
  public class MonsterData : ScriptableObject
  {
    public string monsterName;
    public int monsterHP = 3;
  }
}