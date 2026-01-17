using CresentProject.Player;
using UnityEngine;
using UnityEngine.UI;

namespace CresentProject.Misc
{
    public class UICursor : MonoBehaviour
    {
        public RectTransform cursorImage;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            switch (CurrentState.state)
            {
                case CurrentState.States.Grounded:
                    cursorImage.position = Input.mousePosition; // follow mouse
                    cursorImage.gameObject.SetActive(false);
                    break;
                case CurrentState.States.Flying:
                    cursorImage.position = Input.mousePosition; // follow mouse
                    cursorImage.gameObject.SetActive(true);
                    break;
            }
        }
    }
}
