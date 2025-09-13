using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodFill : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;

    [SerializeField]
    private LogManager logManager;

    private float delayPassos;

    public void IniciarPreenchimento(Vector2Int startPoint, float delayPassos)
    {
        this.delayPassos = delayPassos;
        StartCoroutine(FloodFillCoroutine(startPoint));
    }

    private IEnumerator FloodFillCoroutine(Vector2Int startPoint)
    {
        Queue<Vector2Int> pixels = new();
        pixels.Enqueue(startPoint);

        logManager.AddLogMessage($"Iniciando Flood Fill em ({startPoint.x},{startPoint.y}).");

        while (pixels.Count > 0)
        {
            Vector2Int currentPixel = pixels.Dequeue();
            int x = currentPixel.x;
            int y = currentPixel.y;

            Color currentColor = drawingRenderer.GetPixelColorCartesian(new Vector2Int(x, y));

            if (currentColor != Color.black && currentColor != Color.blue)
            {
                drawingRenderer.DrawPixelCartesian(currentPixel, Color.blue);

                pixels.Enqueue(new Vector2Int(x + 1, y)); // Direita
                pixels.Enqueue(new Vector2Int(x - 1, y)); // Esquerda
                pixels.Enqueue(new Vector2Int(x, y + 1)); // Cima
                pixels.Enqueue(new Vector2Int(x, y - 1)); // Baixo

                if (delayPassos > 0)
                    yield return new WaitForSeconds(delayPassos);
            }
        }

        logManager.AddLogMessage("Preenchimento concluído!");
    }
}