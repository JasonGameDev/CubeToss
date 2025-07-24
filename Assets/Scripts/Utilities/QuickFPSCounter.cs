using UnityEngine;

namespace CubeToss.Utilities
{
    public class QuickFPSCounter : MonoBehaviour
    {
        float deltaTime;

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle
            {
                alignment = TextAnchor.UpperRight,
                fontSize = h / 40,
                normal = { textColor = Color.white }
            };
            Rect rect = new Rect(0, 10, w - 10, h / 10);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            GUI.Label(rect, $"{msec:0.0} ms ({fps:0.} fps)", style);
        }
    }
}