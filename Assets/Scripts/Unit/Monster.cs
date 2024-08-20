using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Monster : MonoBehaviour
{
  public MonsterData monsterData;

  public GameObject hitParticlePrefab;

  public void OnHit()
  {
    // �ӽ÷� ������� ��ƼŬ ��ȯ
    Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
    Destroy(gameObject);
  }
}