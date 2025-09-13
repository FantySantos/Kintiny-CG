using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PontoMedioElipse : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;

    [SerializeField]
    private LogManager logManager;

    private float delayPassos;

    private int xc, yc, a, b;
    private List<Vector2Int> elipse = new();

    public void Desenhar(int xc, int yc, int a, int b, float delayPassos)
    {
        if (drawingRenderer == null) return;

        this.xc = xc;
        this.yc = yc;
        this.a = a;
        this.b = b;
        this.delayPassos = delayPassos;

        StartCoroutine(DesenharCoroutine());
    }

    private IEnumerator DesenharCoroutine()
    {
        logManager.AddLogMessage($"Iniciando desenho da elipse com centro em ({xc}, {yc}), semi-eixo maior {a} e semi-eixo menor {b}.");
        elipse.Clear();

        int x = 0;
        int y = b;
        long a2 = (long)a * a;
        long b2 = (long)b * b;
        long d1 = b2 - (a2 * b) + (a2 / 4);
        long dx = 2 * b2 * x;
        long dy = 2 * a2 * y;

        // Região 1
        while (dx < dy)
        {
            Desenhar4Pontos(x, y);

            if (delayPassos > 0)
                yield return new WaitForSeconds(delayPassos);

            if (d1 < 0)
            {
                x++;
                dx = dx + (2 * b2);
                d1 = d1 + dx + b2;
            }
            else
            {
                x++;
                y--;
                dx = dx + (2 * b2);
                dy = dy - (2 * a2);
                d1 = d1 + dx - dy + b2;
            }
        }

        // Região 2
        long d2 = (b2 * (2 * x + 1) * (2 * x + 1)) / 4 + (a2 * (y - 1) * (y - 1)) - (a2 * b2);

        while (y >= 0)
        {
            Desenhar4Pontos(x, y);
            yield return new WaitForSeconds(delayPassos);

            if (d2 > 0)
            {
                y--;
                dy = dy - (2 * a2);
                d2 = d2 + a2 - dy;
            }
            else
            {
                y--;
                x++;
                dx = dx + (2 * b2);
                dy = dy - (2 * a2);
                d2 = d2 + dx - dy + a2;
            }
        }

        drawingRenderer.ClearCanvas(false);
        foreach (var ponto in elipse)
        {
            drawingRenderer.DrawPixelCartesian(ponto);
        }

        logManager.AddLogMessage("Elipse desenhada com sucesso!");
    }

    private void Desenhar4Pontos(int x, int y)
    {
        Vector2Int[] pontos = new Vector2Int[4];
        pontos[0] = new Vector2Int(xc + x, yc + y);
        pontos[1] = new Vector2Int(xc - x, yc + y);
        pontos[2] = new Vector2Int(xc - x, yc - y);
        pontos[3] = new Vector2Int(xc + x, yc - y);

        foreach (var ponto in pontos)
        {
            if (!elipse.Contains(ponto))
            {
                drawingRenderer.DrawPixelCartesian(ponto, Color.red);
                elipse.Add(ponto);
            }
        }
    }
}