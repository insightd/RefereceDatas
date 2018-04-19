using UnityEngine;
using System.Collections;

public class TextureLoader : MonoBehaviour 
{
    public void LoadTexture(GameObject target, string texPath)
    {
        StartCoroutine(TextureLoad(target, texPath));
    }

    public void LoadTexture(GameObject[] target, string texPath)
    {
        StartCoroutine(TextureLoad(target, texPath));
    }

    IEnumerator TextureLoad(GameObject target, string texPath)
    {
        //texPath = texPath.Replace("/", "\\");
        texPath = texPath.Replace("\\", "/");
        WWW _texture = new WWW("file://" + texPath);

        yield return _texture;

        Renderer renderer = target.GetComponent<Renderer>();

        if (renderer != null && _texture != null)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                Texture2D newTexture = new Texture2D(_texture.texture.width, _texture.texture.height, TextureFormat.DXT1, false);
                renderer.materials[i].SetTexture("_MainTex", newTexture);
                _texture.LoadImageIntoTexture(newTexture);
            }
        }

        _texture.Dispose();
        _texture = null;

        //Resources.UnloadUnusedAssets();
    }

    IEnumerator TextureLoad(GameObject[] target, string texPath)
    {
        //texPath = texPath.Replace("/", "\\");
        texPath = texPath.Replace("\\", "/");
        WWW _texture = new WWW("file://" + texPath);

        yield return _texture;

        for (int k = 0; k < target.Length; k++)
        {
            Renderer renderer = target[k].GetComponent<Renderer>();

            if (renderer != null && _texture != null)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    Texture2D newTexture = new Texture2D(_texture.texture.width, _texture.texture.height, TextureFormat.DXT1, false);
                    renderer.materials[i].SetTexture("_MainTex", newTexture);
                    _texture.LoadImageIntoTexture(newTexture);
                }
            }
        }

        _texture.Dispose();
        _texture = null;

        //Resources.UnloadUnusedAssets();
    }
    
    public void DisposeTexture(GameObject target)
    {
        Renderer renderer = target.GetComponent<Renderer>();

        if (renderer != null)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].mainTexture != null)
                {
                    Texture2D.DestroyImmediate(renderer.materials[i].mainTexture, true);
                    renderer.materials[i].mainTexture = null;
                }
            }
        }

        target = null;

        Resources.UnloadUnusedAssets();
    }

    public void DisposeTexture(GameObject[] target)
    {
        for (int k = 0; k < target.Length; k++)
        {
            Renderer renderer = target[k].GetComponent<Renderer>();

            if (renderer != null)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    if (renderer.materials[i].mainTexture != null)
                    {
                        Texture2D.DestroyImmediate(renderer.materials[i].mainTexture, true);
                        renderer.materials[i].mainTexture = null;
                    }
                }
            }
        }

        target = null;

        Resources.UnloadUnusedAssets();
    }
}
