using UnityEngine;
using UnityEngine.UI;

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
        cursorImage.position = Input.mousePosition; // follow mouse
    }
}
