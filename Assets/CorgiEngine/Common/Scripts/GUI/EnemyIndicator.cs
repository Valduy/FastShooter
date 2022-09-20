using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.CorgiEngine
{
    public class EnemyIndicator : MonoBehaviour
    {
        private Image _image;

        public string LeftSignal = "_IsLeftVisible";
        public string RightSignal = "_IsRightVisible";

        private void Start()
        {
            _image = GetComponent<Image>();
        }

        public void SetLeftVisibility(bool isVisible)
        {
            _image.material.SetFloat(LeftSignal, isVisible ? 1.0f : 0.0f);
        }

        public void SetRightVisibility(bool isVisible)
        {
            _image.material.SetFloat(RightSignal, isVisible ? 1.0f : 0.0f);
        }
    }
}
