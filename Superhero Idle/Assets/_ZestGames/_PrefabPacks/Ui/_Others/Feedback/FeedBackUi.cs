using UnityEngine;
using TMPro;

namespace ZestGames
{
    public class FeedBackUi : MonoBehaviour
    {
        public enum Colors
        {
            Cyan,
            Magenta,
            Green
        }

        private TextMeshProUGUI _feedbackText;
        private Color _cyan = Color.cyan;
        private Color _magenta = Color.magenta;
        private Color _green = Color.green;

        private void Awake()
        {
            _feedbackText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            _feedbackText.transform.parent.gameObject.SetActive(false);
        }

        private void Start()
        {
            UiEvents.OnGiveFeedBack += GiveFeedBack;
        }

        private void OnDisable()
        {
            UiEvents.OnGiveFeedBack -= GiveFeedBack;
        }

        private void GiveFeedBack(string message, Colors color)
        {
            if (color == Colors.Cyan)
                _feedbackText.color = _cyan;
            else if (color == Colors.Magenta)
                _feedbackText.color = _magenta;
            else if (color == Colors.Green)
                _feedbackText.color = _green;

            _feedbackText.text = message;
            _feedbackText.transform.parent.gameObject.SetActive(false);
            _feedbackText.transform.parent.gameObject.SetActive(true);
            _feedbackText.transform.parent.localEulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(-5f, 5f));
        }
    }
}
