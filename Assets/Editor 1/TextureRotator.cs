using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Rotates texture files - creates new rotated versions
/// Put this in Assets/Editor/ folder
/// </summary>
public class TextureRotator : EditorWindow
{
    private Texture2D sourceTexture;
    private int rotationAngle = 90;

    [MenuItem("Tools/Rotate Texture")]
    static void Init()
    {
        TextureRotator window = (TextureRotator)EditorWindow.GetWindow(typeof(TextureRotator));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Texture Rotator", EditorStyles.boldLabel);

        sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Source Texture", sourceTexture, typeof(Texture2D), false);

        rotationAngle = EditorGUILayout.IntSlider("Rotation Angle", rotationAngle, 0, 360);

        if (GUILayout.Button("Rotate 90Åã CW"))
        {
            rotationAngle = 90;
            RotateTexture();
        }

        if (GUILayout.Button("Rotate 180Åã"))
        {
            rotationAngle = 180;
            RotateTexture();
        }

        if (GUILayout.Button("Rotate 270Åã CW (90Åã CCW)"))
        {
            rotationAngle = 270;
            RotateTexture();
        }

        if (GUILayout.Button("Flip Horizontal"))
        {
            FlipTexture(true, false);
        }

        if (GUILayout.Button("Flip Vertical"))
        {
            FlipTexture(false, true);
        }
    }

    void RotateTexture()
    {
        if (sourceTexture == null)
        {
            Debug.LogError("No texture selected!");
            return;
        }

        // Make texture readable
        string path = AssetDatabase.GetAssetPath(sourceTexture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.isReadable = true;
            AssetDatabase.ImportAsset(path);
        }

        Texture2D rotated = null;

        if (rotationAngle == 90)
        {
            rotated = RotateTexture90CW(sourceTexture);
        }
        else if (rotationAngle == 180)
        {
            rotated = RotateTexture180(sourceTexture);
        }
        else if (rotationAngle == 270)
        {
            rotated = RotateTexture90CCW(sourceTexture);
        }

        if (rotated != null)
        {
            SaveTexture(rotated, path);
        }
    }

    void FlipTexture(bool horizontal, bool vertical)
    {
        if (sourceTexture == null)
        {
            Debug.LogError("No texture selected!");
            return;
        }

        string path = AssetDatabase.GetAssetPath(sourceTexture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.isReadable = true;
            AssetDatabase.ImportAsset(path);
        }

        Texture2D flipped = FlipTexturePixels(sourceTexture, horizontal, vertical);
        SaveTexture(flipped, path);
    }

    Texture2D RotateTexture90CW(Texture2D source)
    {
        Texture2D result = new Texture2D(source.height, source.width);

        for (int x = 0; x < source.width; x++)
        {
            for (int y = 0; y < source.height; y++)
            {
                result.SetPixel(source.height - y - 1, x, source.GetPixel(x, y));
            }
        }

        result.Apply();
        return result;
    }

    Texture2D RotateTexture90CCW(Texture2D source)
    {
        Texture2D result = new Texture2D(source.height, source.width);

        for (int x = 0; x < source.width; x++)
        {
            for (int y = 0; y < source.height; y++)
            {
                result.SetPixel(y, source.width - x - 1, source.GetPixel(x, y));
            }
        }

        result.Apply();
        return result;
    }

    Texture2D RotateTexture180(Texture2D source)
    {
        Texture2D result = new Texture2D(source.width, source.height);

        for (int x = 0; x < source.width; x++)
        {
            for (int y = 0; y < source.height; y++)
            {
                result.SetPixel(source.width - x - 1, source.height - y - 1, source.GetPixel(x, y));
            }
        }

        result.Apply();
        return result;
    }

    Texture2D FlipTexturePixels(Texture2D source, bool horizontal, bool vertical)
    {
        Texture2D result = new Texture2D(source.width, source.height);

        for (int x = 0; x < source.width; x++)
        {
            for (int y = 0; y < source.height; y++)
            {
                int newX = horizontal ? source.width - x - 1 : x;
                int newY = vertical ? source.height - y - 1 : y;
                result.SetPixel(newX, newY, source.GetPixel(x, y));
            }
        }

        result.Apply();
        return result;
    }

    void SaveTexture(Texture2D texture, string originalPath)
    {
        byte[] bytes = texture.EncodeToPNG();

        string directory = Path.GetDirectoryName(originalPath);
        string filename = Path.GetFileNameWithoutExtension(originalPath);
        string extension = Path.GetExtension(originalPath);

        string newPath = Path.Combine(directory, filename + "_rotated" + extension);

        File.WriteAllBytes(newPath, bytes);
        AssetDatabase.Refresh();

        Debug.Log($"Saved rotated texture to: {newPath}");
    }
}