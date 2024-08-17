using UnityEngine;
using Unit;
using System;

  [RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
  public Player player;
  public float lerpSpeed = 3.0f;
  public float cameraShakeDampSpeed = 10.0f;
  public Vector2 cameraOffset = Vector2.zero;
  public bool fixXAxis = false;

  private float cameraShake = 0.0f;
  private Vector3 cameraPosition = Vector3.zero;

  private Camera playerCamera;

  private void Awake()
  {
    playerCamera = GetComponent<Camera>();
    cameraPosition = transform.position;
  }

  protected virtual void Update()
  {
    if (player == null) return;

    Vector3 targetPosition = player.transform.position + new Vector3(cameraOffset.x, cameraOffset.y);
    targetPosition.z = cameraPosition.z;
    if (fixXAxis) targetPosition.x = cameraPosition.x;

    cameraPosition = Vector3.Lerp(cameraPosition, targetPosition, lerpSpeed * Time.deltaTime);
    transform.position = cameraPosition + new Vector3(UnityEngine.Random.Range(0.0f, cameraShake), UnityEngine.Random.Range(0.0f, cameraShake));
    cameraShake = Mathf.Lerp(cameraShake, 0.0f, cameraShakeDampSpeed * Time.deltaTime);
  }

  public void ShakeCamera(float strength = 1.0f)
  {
    cameraShake = Math.Max(cameraShake, strength);
  }
}
