using UnityEngine;

/// <summary>
/// 최종 수정일 : 2016.03.30
/// 
/// Texture2D rImage = ImageRotator.RotateImage(source, ImageRotator.RotationAngle.CW90, false);
/// 
/// </summary>

public class ImageRotator : MonoBehaviour 
{
    public enum RotationAngle
    {
        CW0,
        CW90,
        CW180,
        CW270
    };

    private static int sourceWidth = 0;
    private static int sourceHeight = 0;
    private static int resultWidth = 0;
    private static int resultHeight = 0;

    public static Texture2D RotateImage(Texture2D _source, RotationAngle _angle, bool _flip)
    {
        sourceWidth = _source.width;
        sourceHeight = _source.height;

        Color32[] sourceColors = _source.GetPixels32();
        Color32[] resultColors = null;

        switch (_angle)
        {
            case RotationAngle.CW0:
                resultWidth = sourceWidth;
                resultHeight = sourceHeight;
                resultColors = GetColorsCW0(sourceColors, _flip);
                break;
            case RotationAngle.CW90:
                resultWidth = sourceHeight;
                resultHeight = sourceWidth;
                resultColors = GetColorsCW90(sourceColors, _flip);
                break;
            case RotationAngle.CW180:
                resultWidth = sourceWidth;
                resultHeight = sourceHeight;
                resultColors = GetColorsCW180(sourceColors, _flip);
                break;
            case RotationAngle.CW270:
                resultWidth = sourceHeight;
                resultHeight = sourceWidth;
                resultColors = GetColorsCW270(sourceColors, _flip);
                break;
        }

        Texture2D result = new Texture2D(resultWidth, resultHeight, _source.format, false);
        result.filterMode = _source.filterMode;

        result.SetPixels32(resultColors);
        result.Apply();

        return result;
        
        /*
        try
        {
            result[index] = _source[oIndex];
        }
        catch (Exception e)
        {
            Debug.Log(xx + "/" + yy + "/" + oIndex + "/" + index);
            break;
        }
        */
    }

    private static Color32[] GetColorsCW0(Color32[] _source, bool _flip)
    {
        if (_flip)
        {
            Color32[] result = new Color32[_source.Length];

            for (int y = 0; y < sourceHeight; y++)
            {
                for (int x = 0; x < sourceWidth; x++)
                {
                    int xx = resultWidth - x;
                    int yy = y;
                    int oIndex = (yy - 1) * resultWidth + xx - 1 + resultWidth;
                    int index = resultWidth * y + x;

                    result[index] = _source[oIndex];
                }
            }

            return result;
        }
        else
        {
            return _source;
        }
    }

    private static Color32[] GetColorsCW90(Color32[] _source, bool _flip)
    {
        Color32[] result = new Color32[_source.Length];

        if (_flip)
        {
            for (int y = 0; y < sourceHeight; y++)
            {
                for (int x = 0; x < sourceWidth; x++)
                {
                    int xx = resultHeight - (resultHeight - x);
                    int yy = y;
                    int oIndex = yy * resultHeight + xx;
                    int index = resultWidth * x + y;

                    result[index] = _source[oIndex];
                }
            }
        }
        else
        {
            for (int y = 0; y < sourceHeight; y++)
            {
                for (int x = 0; x < sourceWidth; x++)
                {
                    int xx = resultHeight - x;
                    int yy = y;
                    int oIndex = yy * resultHeight + xx - 1;
                    int index = resultWidth * x + y;

                    result[index] = _source[oIndex];
                }
            }
        }

        return result;
    }

    private static Color32[] GetColorsCW180(Color32[] _source, bool _flip)
    {
        Color32[] result = new Color32[_source.Length];

        if (_flip)
        {
            for (int y = 0; y < sourceHeight; y++)
            {
                for (int x = 0; x < sourceWidth; x++)
                {
                    int xx = resultWidth - (resultWidth - x);
                    int yy = resultHeight - y;
                    int oIndex = (yy - 1) * resultWidth + xx;
                    int index = resultWidth * y + x;
                    
                    result[index] = _source[oIndex];
                }
            }
        }
        else
        {
            for (int y = 0; y < sourceHeight; y++)
            {
                for (int x = 0; x < sourceWidth; x++)
                {
                    int xx = resultWidth - x;
                    int yy = resultHeight - y;
                    int oIndex = (yy - 1) * resultWidth + xx - 1;
                    int index = resultWidth * y + x;
                    
                    result[index] = _source[oIndex];
                }
            }
        }

        return result;
    }

    private static Color32[] GetColorsCW270(Color32[] _source, bool _flip)
    {
        Color32[] result = new Color32[_source.Length];

        if (_flip)
        {
            for (int y = 0; y < sourceHeight; y++)
            {
                for (int x = 0; x < sourceWidth; x++)
                {
                    int xx = resultHeight - x;
                    int yy = resultWidth - y;
                    int oIndex = (yy - 1)* resultHeight + xx - 1;
                    int index = resultWidth * x + y;

                    result[index] = _source[oIndex];
                }
            }
        }
        else
        {
            for (int y = 0; y < sourceHeight; y++)
            {
                for (int x = 0; x < sourceWidth; x++)
                {
                    int xx = x;
                    int yy = resultWidth - y;
                    int oIndex = (yy - 1) * resultHeight + xx;
                    int index = resultWidth * x + y;

                    result[index] = _source[oIndex];
                }
            }
        }

        return result;
    }

    public static Texture2D ResizeImage(Texture2D _source, int _targetWidth, int _targetHeight)
    {
        Texture2D result = new Texture2D(_targetWidth, _targetHeight, _source.format, true);
        Color32[] rpixels = result.GetPixels32(0);
        float incX = (1.0f / (float)_targetWidth);
        float incY = (1.0f / (float)_targetHeight);

        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = _source.GetPixelBilinear(incX * ((float)px % _targetWidth), incY * ((float)Mathf.Floor(px / _targetWidth)));
        }

        result.SetPixels32(rpixels, 0);
        result.Apply();

        return result;
    }

}
