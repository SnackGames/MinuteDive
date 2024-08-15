using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

namespace UI
{
  [DisallowMultipleComponent]
  [ExecuteAlways]
  [RequireComponent(typeof(TextMeshProUGUI))]
  [AddComponentMenu("UI/FPS Text")]
  public class UI_FPSText : MonoBehaviour
  {
    protected const int averageCount = 16;
    protected float[] framerates = new float[averageCount];
    protected int currentFramerateIndex = 0;

    protected TextMeshProUGUI text;

    private void Awake()
    {
      // #TODO 프레임 제어를 다른 곳에서 총괄하도록 하자
      Application.targetFrameRate = 60;

      text = GetComponent<TextMeshProUGUI>();
    }

    protected virtual void Update()
    {
      framerates[currentFramerateIndex++ % averageCount] = Time.deltaTime;

      text.SetText($"{(int)(averageCount / framerates.Sum())} FPS");
    }
  }
}