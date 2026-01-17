using System.Collections.Generic;
using UnityEngine;

namespace CresentProject.Player
{
    public class CollisionCheck : MonoBehaviour
    {
        [HideInInspector] public bool isColliding;
        [SerializeField] LayerMask layerMask;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        // Keep track of all objects currently inside the trigger
        private readonly HashSet<GameObject> objectsInside = new HashSet<GameObject>();
        private void OnTriggerEnter(Collider other)
        {
            // Add any object entering the trigger
            objectsInside.Add(other.gameObject);
            UpdateDetectionState();
        }

        private void OnTriggerExit(Collider other)
        {
            // Remove any object leaving the trigger
            objectsInside.Remove(other.gameObject);
            UpdateDetectionState();
        }

        private void UpdateDetectionState()
        {
            // Check if any object inside the trigger matches the detection mask
            foreach (GameObject obj in objectsInside)
            {
                if (((1 << obj.layer) & layerMask) != 0)
                {
                    isColliding = true;
                    return;
                }
            }

            // If none match, set false
            isColliding = false;
        }
    }
}
