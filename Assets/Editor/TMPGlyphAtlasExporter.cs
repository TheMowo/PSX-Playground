using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public class TMPGlyphAtlasExporter : EditorWindow
{
    private TMP_FontAsset fontAsset;

    [MenuItem("Tools/TMP Glyph Atlas Exporter")]
    private static void Open()
    {
        GetWindow<TMPGlyphAtlasExporter>("TMP Glyph Atlas");
    }

    private void OnGUI()
    {
        fontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField(
            "TMP Font Asset",
            fontAsset,
            typeof(TMP_FontAsset),
            false);

        GUI.enabled = fontAsset != null;

        if (GUILayout.Button("Export Atlas Texture"))
        {
            Export();
        }

        GUI.enabled = true;
    }

    private void Export()
    {
        Texture source =
            fontAsset.atlasTextures[0];

        RenderTexture rt =
            RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.ARGB32);

        Graphics.Blit(source, rt);

        RenderTexture previous =
            RenderTexture.active;

        RenderTexture.active = rt;

        Texture2D output =
            new Texture2D(
                rt.width,
                rt.height,
                TextureFormat.RGBA32,
                false);

        output.ReadPixels(
            new Rect(0, 0, rt.width, rt.height),
            0,
            0);

        output.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        string path =
            EditorUtility.SaveFilePanelInProject(
                "Save Atlas",
                $"{fontAsset.name}_Atlas",
                "png",
                "Export TMP Atlas");

        if (string.IsNullOrEmpty(path))
            return;

        File.WriteAllBytes(
            path,
            output.EncodeToPNG());

        AssetDatabase.Refresh();

        Debug.Log($"Atlas exported: {path}");
    }
}