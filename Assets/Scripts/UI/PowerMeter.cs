using CubeToss.Events;
using UnityEngine;
using UnityEngine.UI;

namespace CubeToss.UI
{
    public class PowerMeter : MonoBehaviour
    {
        // TODO: Normally I would  implement a UI controller module and use that to update UI views but keeping it simple for now.
        [SerializeField] private GrabberEventChannel grabberEventChannel;
        [SerializeField] private Slider slider;

        private void OnEnable()
        {
            grabberEventChannel.UpdateFlickPower.AddListener(OnPowerUpdated);
        }

        private void OnDisable()
        {
            grabberEventChannel.UpdateFlickPower.RemoveListener(OnPowerUpdated);
        }

        private void SetPower(float power)
        {
            if (slider != null)
            {
                slider.value = Mathf.Clamp01(power);

                var color = power < 0.5f
                    ? Color.Lerp(Color.green, Color.yellow, power * 2f)
                    : Color.Lerp(Color.yellow, Color.red, (power - 0.5f) * 2f);
                slider.fillRect.GetComponent<Image>().color = color;
            }
        }

        private void OnPowerUpdated(float power)
        {
            SetPower(power);
        }
    }
}