using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.AI;

[CustomEditor(typeof(RoadTile))]
[CanEditMultipleObjects]
public class ShowRoad3DModelEditor : Editor
{
    private PreviewRenderUtility previewUtility;

    private GameObject previewInstance;
    private GameObject currentPrefab;

    private Material previewMaterial;

    private Dictionary<RoadTileDirection, GameObject> previewTexts = new Dictionary<RoadTileDirection, GameObject>();

    private Dictionary<RoadTileDirection, bool> directionsCompatible = new Dictionary<RoadTileDirection, bool>();

    private void OnEnable()
    {
        previewUtility = new PreviewRenderUtility();

        previewUtility.cameraFieldOfView = 30f;

        previewUtility.lights[0].intensity = 1.4f;
        previewUtility.lights[0].transform.rotation = Quaternion.Euler(40f, 40f, 0f);

        previewUtility.lights[1].intensity = 1.4f;

        RoadTile roadTile = (RoadTile)target;

        foreach (RoadTileDirection rtd in Enum.GetValues(typeof(RoadTileDirection)))
        {
            directionsCompatible.Add(rtd, false);
            Draw3DText(directionsCompatible[rtd] ? "✓" : "✗", new Vector3(0, 0, 0), 0.1f, directionsCompatible[rtd] ? Color.green : Color.red, rtd);
        }
    }

    public override bool HasPreviewGUI()
    {
        RoadTile roadTile = (RoadTile)target;
        return roadTile.prefab != null;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        RoadTile roadTile = (RoadTile)target;

        if (roadTile.prefab == null)
        {
            return;
        }

        previewMaterial = roadTile.prefab.GetComponentInChildren<MeshRenderer>().sharedMaterial;

        CreatePreviewInstanceIfNeeded(roadTile.prefab);

        if (previewInstance == null)
            return;

        previewInstance.transform.position = Vector3.zero;
        previewInstance.transform.rotation = Quaternion.Euler(0f, roadTile.meshRotation * 90f, 0f);

        Bounds bounds = GetBounds(previewInstance);

        Vector3 center = bounds.center;
        float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        if (size <= 0.01f)
            size = 1f;

        Camera cam = previewUtility.camera;

        cam.transform.position = center + new Vector3(0f, size * 3f, 0);
        cam.transform.LookAt(new Vector3(0, center.y, 0));

        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 100f;
        cam.clearFlags = CameraClearFlags.Color;
        cam.backgroundColor = new Color(0.18f, 0.18f, 0.18f, 1f);

        float maxPreviewSize = 512f;

        Rect safeRenderRect = new Rect(
            0f,
            0f,
            Mathf.Min(r.width, maxPreviewSize),
            Mathf.Min(r.height, maxPreviewSize)
        );

        previewUtility.BeginPreview(safeRenderRect, background);

        UpdateDirectionCompatibility(roadTile);

        DrawTicksCrossesText(bounds);


        previewUtility.Render(true, true);

        DrawCardinalPoints(r);

        Texture result = previewUtility.EndPreview();
        GUI.DrawTexture(r, result, ScaleMode.ScaleToFit, false);
    }

    private void DrawTicksCrossesText(Bounds bounds)
    {
        RoadTile roadTile = (RoadTile)target;

        foreach (RoadTileDirection rtd in Enum.GetValues(typeof(RoadTileDirection)))
        {
            if (rtd == RoadTileDirection.North || rtd == RoadTileDirection.South)
            {
                previewTexts[rtd].transform.position = new Vector3(0, 0, (bounds.size.z / 1.5f) * (rtd == RoadTileDirection.South ? -1 : 1));
            }
            else
            {
                previewTexts[rtd].transform.position = new Vector3((bounds.size.x / 1.5f) * (rtd == RoadTileDirection.West ? -1 : 1), 0, 0);
            }

            previewTexts[rtd].GetComponent<TextMesh>().text = directionsCompatible[rtd] ? "✓" : "✗";
            previewTexts[rtd].GetComponent<TextMesh>().color = directionsCompatible[rtd] ? Color.green : Color.red;
        }
    }

    private GameObject CreatePreviewText(string text, Color color, RoadTileDirection direction)
    {
        GameObject textObject = new GameObject("Preview Text " + text);
        textObject.hideFlags = HideFlags.HideAndDontSave;

        TextMesh textMesh = textObject.AddComponent<TextMesh>();

        textMesh.text = text;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 64;
        textMesh.characterSize = 0.1f;
        textMesh.color = color;

        MeshRenderer renderer = textObject.GetComponent<MeshRenderer>();

        if (textMesh.font != null)
        {
            renderer.sharedMaterial = textMesh.font.material;
        }

        previewUtility.AddSingleGO(textObject);
        previewTexts.Add(direction, textObject);

        return textObject;
    }

    private void ClearPreviewTexts()
    {
        foreach (KeyValuePair<RoadTileDirection, GameObject> previewText in previewTexts)
        {
            if (previewText.Value != null)
            {
                DestroyImmediate(previewText.Value);
            }
        }

        directionsCompatible.Clear();
        previewTexts.Clear();
    }

    private GameObject Draw3DText(string text, Vector3 position, float characterSize, Color color, RoadTileDirection direction)
    {
        GameObject textObject = CreatePreviewText(text, color, direction);

        TextMesh textMesh = textObject.GetComponent<TextMesh>();
        textMesh.characterSize = characterSize;

        textObject.transform.position = position;

        // Same orientation as the top-down camera.
        // This makes the text lie flat in XZ space and face the camera.
        textObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        return textObject;
    }

    private void UpdateDirectionCompatibility(RoadTile roadTile)
    {
        foreach (RoadTileDirection rtd in Enum.GetValues(typeof(RoadTileDirection)))
        {
            directionsCompatible[rtd] =
                roadTile.GetDirection(rtd).Second == RoadTileConnectionType.Connectable;
        }
    }

    private void DrawCardinalPoints(Rect previewRect)
    {
        float size = 120f;

        Rect boxRect = new Rect(
            previewRect.x + 10f,
            previewRect.y,
            size,
            size
        );

        EditorGUI.DrawRect(boxRect, new Color(0f, 0f, 0f, 0.55f));

        Vector2 center = boxRect.center;

        Color oldColor = GUI.color;

        GUI.color = new Color(1f, 1f, 1f, 0.8f);

        EditorGUI.DrawRect(
            new Rect(center.x + 2, boxRect.y + 25f, 2f, size - 50f),
            GUI.color
        );

        EditorGUI.DrawRect(
            new Rect(boxRect.x + 27f, center.y - 1f, size - 50f, 2f),
            GUI.color
        );

        GUI.color = oldColor;

        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.fontSize = 18;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(center.x - 12f, boxRect.y + 2f, 30f, 25f), "N", style);
        GUI.Label(new Rect(center.x - 12f, boxRect.yMax - 27f, 30f, 25f), "S", style);
        GUI.Label(new Rect(boxRect.x + 2f, center.y - 12f, 30f, 25f), "W", style);
        GUI.Label(new Rect(boxRect.xMax - 30f, center.y - 12f, 30f, 25f), "E", style);
    }

    private void CreatePreviewInstanceIfNeeded(GameObject prefab)
    {
        if (currentPrefab == prefab && previewInstance != null)
            return;

        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
            previewInstance = null;
        }

        currentPrefab = prefab;

        previewInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        if (previewInstance == null)
            previewInstance = Instantiate(prefab);

        previewInstance.hideFlags = HideFlags.HideAndDontSave;

        ApplyPreviewMaterial(previewInstance);

        previewUtility.AddSingleGO(previewInstance);
    }

    private void ApplyPreviewMaterial(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = new Material[renderer.sharedMaterials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterial;
            }

            renderer.sharedMaterials = materials;
        }
    }

    private Bounds GetBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }

    private void OnDisable()
    {
        ClearPreviewTexts();

        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
            previewInstance = null;
        }

        if (previewUtility != null)
        {
            previewUtility.Cleanup();
            previewUtility = null;
        }
    }
}