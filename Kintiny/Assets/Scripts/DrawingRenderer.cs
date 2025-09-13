using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawingRenderer : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField]
    private TextMeshProUGUI coordinateText;

    [SerializeField]
    private RawImage canvasRawImage;

    private Texture2D drawingTexture;

    public Color32[] pixelColors;
    private List<Vector2Int> firstPaintedPixels = new();

    private readonly Color backgroundColorA = new(0.8f, 0.8f, 0.8f);
    private readonly Color backgroundColorB = new(0.7f, 0.7f, 0.7f);

    private float pixelZoomFactor = 1f;

    private const float ZOOM_STEP = 1.2f;
    private const float MIN_ZOOM = 0.232568f;
    private const float MAX_ZOOM = 8.916104f;
    private const float DEFAULT_ZOOM = 1.0f;

    private Vector2Int cartesianOrigin;

    public Vector2Int TextureSize => new(drawingTexture.width, drawingTexture.height);

    void Start()
    {
        ResizeCanvas(new Vector2Int(32, 32));
    }

    private Vector2Int CartesianToTextureSpace(Vector2Int cartesianPoint)
    {
        return cartesianOrigin + cartesianPoint;
    }

    private Vector2Int TextureToCartesianSpace(Vector2Int texturePoint)
    {
        return texturePoint - cartesianOrigin;
    }

    public void UpdateFirstPaintedPixels(List<Vector2Int> newPixels)
    {
        firstPaintedPixels.Clear();
        foreach (var pixel in newPixels)
        {
            Debug.Log($"Adding pixel: {pixel}");
            Vector2Int pixelCartesian = CartesianToTextureSpace(pixel);
            firstPaintedPixels.Add(pixelCartesian);
        }
    }

    public List<Vector2Int> GetAllPaintedPixelsCartesian()
    {
        List<Vector2Int> paintedPixels = new();

        foreach (var pixel in firstPaintedPixels)
        {
            Vector2Int cartesianPos = TextureToCartesianSpace(pixel);
            paintedPixels.Add(cartesianPos);
        }

        return paintedPixels;
    }

    public Color GetPixelColorCartesian(Vector2Int pixel)
    {
        Vector2Int texturePos = CartesianToTextureSpace(pixel);
        if (texturePos.x < 0 || texturePos.x >= TextureSize.x ||
            texturePos.y < 0 || texturePos.y >= TextureSize.y)
        {
            return Color.clear;
        }
        int pixelIndex = texturePos.y * TextureSize.x + texturePos.x;
        return pixelColors[pixelIndex];
    }

    public void ResizeCanvas(Vector2Int newSize)
    {
        cartesianOrigin = new Vector2Int(newSize.x / 2, newSize.y / 2);

        drawingTexture = new(newSize.x, newSize.y)
        {
            filterMode = FilterMode.Point
        };

        canvasRawImage.texture = drawingTexture;

        pixelColors = new Color32[newSize.x * newSize.y];
        ClearCanvas();
    }

    public void ClearCanvas(bool resetFirstPaint = true)
    {
        for (int y = 0; y < drawingTexture.height; y++)
        {
            for (int x = 0; x < drawingTexture.width; x++)
            {
                Color backgroundColor = ((x + y) % 2 == 0) ? backgroundColorA : backgroundColorB;

                int pixelIndex = y * drawingTexture.width + x;
                pixelColors[pixelIndex] = backgroundColor;
            }
        }

        if (resetFirstPaint)
            firstPaintedPixels.Clear();

        ApplyPixelChanges();
    }

    public void ClearPaintedPixels()
    {
        firstPaintedPixels.Clear();
    }

    public void ClearRedPixels()
    {
        for (int y = 0; y < drawingTexture.height; y++)
        {
            for (int x = 0; x < drawingTexture.width; x++)
            {
                int pixelIndex = y * drawingTexture.width + x;

                if (pixelColors[pixelIndex] == Color.red)
                {
                    Vector2Int texturePosition = new(x, y);
                    ErasePixelTextureSpace(texturePosition);
                }
            }
        }
        ApplyPixelChanges();
    }

    public void ErasePixelCartesian(Vector2Int cartesianPosition)
    {
        Vector2Int texturePosition = CartesianToTextureSpace(cartesianPosition);
        ErasePixelTextureSpace(texturePosition);
    }

    private void ErasePixelTextureSpace(Vector2Int texturePosition)
    {
        firstPaintedPixels.Remove(texturePosition);
        Color backgroundColor = ((texturePosition.x + texturePosition.y) % 2 == 0) ? backgroundColorA : backgroundColorB;
        int pixelIndex = texturePosition.y * TextureSize.x + texturePosition.x;
        pixelColors[pixelIndex] = backgroundColor;
        ApplyPixelChanges();
    }

    private void ApplyPixelChanges()
    {
        drawingTexture.SetPixels32(pixelColors);
        drawingTexture.Apply();
    }

    private void RegisterFirstPaintedPixel(Vector2Int texturePosition)
    {
        if (!firstPaintedPixels.Contains(texturePosition))
            firstPaintedPixels.Add(texturePosition);
    }

    private void DrawPixelTextureSpace(Vector2Int texturePosition, Color? color = null)
    {
        if (texturePosition.x < 0 || texturePosition.x >= TextureSize.x ||
            texturePosition.y < 0 || texturePosition.y >= TextureSize.y)
        {
            return;
        }

        int pixelIndex = texturePosition.y * TextureSize.x + texturePosition.x;

        pixelColors[pixelIndex] = color ?? Color.black;

        ApplyPixelChanges();
    }

    public void DrawPixelCartesian(Vector2Int cartesianPosition, Color? color = null)
    {
        Vector2Int texturePosition = CartesianToTextureSpace(cartesianPosition);
        DrawPixelTextureSpace(texturePosition, color);
    }

    public void ZoomIn()
    {
        pixelZoomFactor = Mathf.Clamp(pixelZoomFactor * ZOOM_STEP, MIN_ZOOM, MAX_ZOOM);
        UpdateCanvasScale();
    }
    public void ZoomOut()
    {
        pixelZoomFactor = Mathf.Clamp(pixelZoomFactor / ZOOM_STEP, MIN_ZOOM, MAX_ZOOM);
        UpdateCanvasScale();
    }

    public void ResetZoom()
    {
        pixelZoomFactor = DEFAULT_ZOOM;
        UpdateCanvasScale();
    }

    private void UpdateCanvasScale()
    {
        canvasRawImage.rectTransform.localScale = new Vector3(pixelZoomFactor, pixelZoomFactor, 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2Int texturePosition = GetTexturePositionFromPointer(eventData);

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (UIController.selectedAlgorithmIndex == 7)
            {
                DrawPixelTextureSpace(texturePosition, Color.red);
                RegisterFirstPaintedPixel(texturePosition);
                return;
            }
            DrawPixelTextureSpace(texturePosition);
            RegisterFirstPaintedPixel(texturePosition);
            return;
        }

        ErasePixelTextureSpace(texturePosition);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        Vector2Int texturePosition = GetTexturePositionFromPointer(eventData);
        Vector2Int cartesianPosition = TextureToCartesianSpace(texturePosition);

        if (coordinateText != null)
        {
            coordinateText.gameObject.SetActive(true);
            coordinateText.text = $"X: {cartesianPosition.x}, Y: {cartesianPosition.y}";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (coordinateText != null)
        {
            coordinateText.text = "X: -, Y: -";
        }
    }

    private Vector2Int GetTexturePositionFromPointer(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRawImage.rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPosition);

        Rect rect = canvasRawImage.rectTransform.rect;
        float normalizedX = (localPosition.x + canvasRawImage.rectTransform.pivot.x * rect.width) / rect.width;
        float normalizedY = (localPosition.y + canvasRawImage.rectTransform.pivot.y * rect.height) / rect.height;

        return new Vector2Int(
            Mathf.FloorToInt(normalizedX * drawingTexture.width),
            Mathf.FloorToInt(normalizedY * drawingTexture.height)
        );
    }
}
