using UnityEngine;

namespace Data
{
  [CreateAssetMenu(fileName = "Data", menuName = "Data/Monster")]
  public class MonsterData : ScriptableObject
  {
    public string monsterName;
    public int monsterHP = 3;
    public float monsterMoveSpeed = 10.0f;
  }
}