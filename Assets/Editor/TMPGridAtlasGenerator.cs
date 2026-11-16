using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

public class TMPGridAtlasGenerator : EditorWindow
{
    private TMP_FontAsset fontAsset;

    private int atlasSize = 2048;
    private int cellsPerSide = 16;

    private string glyphSet =
        "0123456789" +
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        "abcdefghijklmnopqrstuvwxyz" +
        "!@#$%^&*()[]{}<>";

    [MenuItem("Tools/TMP Grid Atlas Generator")]
    private static void Open()
    {
        GetWindow<TMPGridAtlasGenerator>();
    }

    private void OnGUI()
    {
        fontAsset =
            (TMP_FontAsset)EditorGUILayout.ObjectField(
                "Font Asset",
                fontAsset,
                typeof(TMP_FontAsset),
                false);

        atlasSize =
            EditorGUILayout.IntField(
                "Atlas Size",
                atlasSize);

        cellsPerSide =
            EditorGUILayout.IntField(
                "Cells Per Side",
                cellsPerSide);

        GUILayout.Label("Characters");

        glyphSet =
            EditorGUILayout.TextArea(
                glyphSet,
                GUILayout.Height(120));

        GUI.enabled = fontAsset != null;

        if (GUILayout.Button("Generate"))
        {
            Generate();
        }

        GUI.enabled = true;
    }

    private void Generate()
    {
        int cellSize =
            atlasSize / cellsPerSide;

        Texture2D atlas =
            new Texture2D(
                atlasSize,
                atlasSize,
                TextureFormat.RGBA32,
                false);

        Color[] pixels =
            new Color[atlasSize * atlasSize];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.black;

        atlas.SetPixels(pixels);

        Texture2D source =
            fontAsset.atlasTextures[0] as Texture2D;

        int maxGlyphs =
            Mathf.Min(
                glyphSet.Length,
                cellsPerSide * cellsPerSide);

        for (int i = 0; i < maxGlyphs; i++)
        {
            uint codepoint = glyphSet[i];

            if (!fontAsset.characterLookupTable.TryGetValue(
                codepoint,
                out TMP_Character character))
            {
                Debug.LogWarning(
                    $"Missing glyph {glyphSet[i]}");

                continue;
            }

            Glyph glyph = character.glyph;

            GlyphRect rect =
                glyph.glyphRect;

            Color[] glyphPixels =
                source.GetPixels(
                    rect.x,
                    rect.y,
                    rect.width,
                    rect.height);

            int col =
                i % cellsPerSide;

            int row =
                i / cellsPerSide;

            int cellX =
                col * cellSize;

            int cellY =
                atlasSize -
                ((row + 1) * cellSize);

            int offsetX =
                (cellSize - rect.width) / 2;

            int offsetY =
                (cellSize - rect.height) / 2;

            atlas.SetPixels(
                cellX + offsetX,
                cellY + offsetY,
                rect.width,
                rect.height,
                glyphPixels);
        }

        atlas.Apply();

        byte[] png =
            atlas.EncodeToPNG();

        string path =
            EditorUtility.SaveFilePanelInProject(
                "Save Atlas",
                "GlyphAtlas",
                "png",
                "");

        if (string.IsNullOrEmpty(path))
            return;

        System.IO.File.WriteAllBytes(
            path,
            png);

        AssetDatabase.Refresh();

        Debug.Log("Atlas Generated");
    }
}