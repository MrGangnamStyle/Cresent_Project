using TMPro;
using UnityEngine;

public class VelocityCounter : MonoBehaviour
{
    TextMeshProUGUI velocityTextMesh;
    [SerializeField] Rigidbody playerRB;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        velocityTextMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        velocityTextMesh.text = playerRB.linearVelocity.ToString();
    }
}
