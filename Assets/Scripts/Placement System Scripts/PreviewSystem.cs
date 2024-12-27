using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f; // Ensures that the preview isn't clipping into floor

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;
    private Renderer cellIndicatorRenderer;

    private void Start()
    {
        // Initialize member variables
        previewMaterialInstance = new Material(previewMaterialPrefab); // Instantiate new version of preview material so we don't modify ALL preview materials
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    // Sets up everything for showing the placement preview object and cell indicator
    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        // Instantiate our preview object and set up the transparency material
        previewObject = Instantiate(prefab);
        SetupPreviewMaterial(previewObject);
        SetupPreviewCursor(size);
        cellIndicator.SetActive(true);
    }

    // Sets up everything for showing the removal placement preview
    public void StartShowingCursorPreview()
    {
        // Preview the default cursor
        SetupPreviewCursor(Vector2Int.one);
        UpdateCursorValidity(false);
        cellIndicator.SetActive(true);
    }

    // Makes the preview object fully transparent by overriding the materials
    private void SetupPreviewMaterial(GameObject previewObject)
    {
        // Get all renderers of each object and their children
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        // Override each material with the transparency material
        foreach (Renderer renderer in renderers)
        {
            // Get the array of all materials in the renderer
            Material[] materials = renderer.materials;

            // For each material, replace it with the transparency material
            for (int index = 0; index < materials.Length; index++)
            {
                materials[index] = previewMaterialInstance;
            }

            // Replace the renderer's materials with our new material array
            renderer.materials = materials;
            renderer.shadowCastingMode = 0; // Disable casting shadows from object previews
        }
    }

    // Scales up the preview cursor to fit the object to be placed
    private void SetupPreviewCursor(Vector2Int size)
    {
        // Make sure the size is actually valid
        if (size.x > 0 && size.y > 0)
        {
            // Scale the preview indicator to match the entire area
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
        }
    }

    // Destroys the preview object and hides the preview cursor
    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
            Destroy(previewObject);
    }

    // Updates the position of the preview and indicator
    public void UpdatePosition(Vector3 position, bool validPlacement)
    {
        // Only update the preview if it is valid
        if (previewObject != null)
        {
            MovePreview(position);
            UpdatePreviewValidity(validPlacement);
        }

        // The cursor will always be valid so update it no matter what
        MoveIndicator(position);
        UpdateCursorValidity(validPlacement);
    }

    // Updates the rotation of the preview
    public void UpdateRotation(Vector3 rotation)
    {
        previewObject.transform.GetChild(0).rotation = Quaternion.Euler(rotation);
    }

    // Moves the preview object to the specified position
    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
    }

    // Moves the cell indicator to the specified position
    private void MoveIndicator(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    // Updates the color of the preview based on placement validity
    private void UpdatePreviewValidity(bool validPlacement)
    {
        // Get the color and set the alpha for the preview colors
        Color color = validPlacement ? Color.white : Color.red;
        color.a = 0.5f;

        // Change the color of the preview material
        previewMaterialInstance.color = color;
    }

    // Updates the color of the cursor based on placement validity
    private void UpdateCursorValidity(bool validPlacement)
    {
        // Get the color and set the alpha for the preview colors
        Color color = validPlacement ? Color.white : Color.red;
        color.a = 0.5f;

        // Change the color of the cursor material
        cellIndicatorRenderer.material.color = color;
    }
}
