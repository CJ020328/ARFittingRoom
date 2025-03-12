using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public List<Material> clothesMaterials; // List of materials for clothes
    private int currentColorIndex;
    private GameObject activeClothModel;

    private AudioSource audioSource; // Reference to the AudioSource component

    void Start()
    {
        currentColorIndex = 0;

        // Subscribe to swipe events
        SwipeGestureDetector swipeDetector = FindObjectOfType<SwipeGestureDetector>();
        if (swipeDetector != null)
        {
            swipeDetector.OnSwipe += HandleSwipe;
        }

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on this GameObject.");
        }
    }

    private void HandleSwipe(int direction)
    {
        CycleColor(direction);
    }

    public void SetActiveClothModel(GameObject clothModel)
    {
        activeClothModel = clothModel;

        if (activeClothModel != null)
        {
            var clothTransform = activeClothModel.transform.Find("Cloth");
            if (clothTransform != null)
            {
                var renderer = clothTransform.GetComponent<SkinnedMeshRenderer>();
                if (renderer != null)
                {
                    Debug.Log("SkinnedMeshRenderer found and material can be assigned.");
                }
                else
                {
                    Debug.LogError("No SkinnedMeshRenderer found on the 'Cloth' child object.");
                }
            }
            else
            {
                Debug.LogError("'Cloth' child object not found in the active clothing model.");
            }
        }
    }

    public void CycleColor(int direction)
    {
        currentColorIndex = (currentColorIndex + direction) % clothesMaterials.Count;
        if (currentColorIndex < 0)
        {
            currentColorIndex = clothesMaterials.Count - 1;
        }

        ApplyMaterial();
    }

    private void ApplyMaterial()
    {
        if (activeClothModel != null)
        {
            var clothTransform = activeClothModel.transform.Find("Cloth");
            if (clothTransform != null)
            {
                var renderer = clothTransform.GetComponent<SkinnedMeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = clothesMaterials[currentColorIndex];
                    Debug.Log($"Material changed to: {clothesMaterials[currentColorIndex].name}");
                    Debug.Log($"Renderer Material: {renderer.material.name}");

                    // Play the sound effect
                    if (audioSource != null)
                    {
                        audioSource.Play();
                    }
                }
                else
                {
                    Debug.LogError("No SkinnedMeshRenderer found on the 'Cloth' child object.");
                }
            }
            else
            {
                Debug.LogError("'Cloth' child object not found in the active clothing model.");
            }
        }
        else
        {
            Debug.LogError("Active clothing model is null.");
        }
    }
}
