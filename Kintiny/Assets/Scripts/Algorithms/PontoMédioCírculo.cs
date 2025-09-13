using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PontoMedioCirculo : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;

    [SerializeField]
    private LogManager logManager;

    private float delayPassos;

    private int xc, yc, r;
    private int x, y, e;

    private List<Vector2Int> circulo = new List<Vector2Int>();

    public void Desenhar(int xc, int yc, int r, float delayPassos)
    {
        if (drawingRenderer == null) return;

        this.xc = xc;
        this.yc = yc;
        this.r = r;
        this.delayPassos = delayPassos;

        StartCoroutine(DesenharCoroutine());
    }

    private IEnumerator DesenharCoroutine()
    {
        logManager.AddLogMessage($"Iniciando desenho do círculo com centro em ({xc}, {yc}) e raio {r}.");
        circulo.Clear();

        x = 0;
        y = r;
        e = -r;

        Desenhar8Pontos(x, y);

        while (x <= y)
        {
            if (delayPassos > 0)
                yield return new WaitForSeconds(delayPassos);

            e += 2 * x + 1;
            x++;

            if (e >= 0)
            {
                e += 2 - 2 * y;
                y--;
            }

            Desenhar8Pontos(x, y);
        }


        drawingRenderer.ClearCanvas(false);
        foreach (var ponto in circulo)
        {
            drawingRenderer.DrawPixelCartesian(ponto);
        }

        logManager.AddLogMessage("Círculo desenhado com sucesso!");
    }

    private void Desenhar8Pontos(int x, int y)
    {
        Vector2Int[] pontos = new Vector2Int[8];
        pontos[0] = new Vector2Int(xc + x, yc + y);
        pontos[1] = new Vector2Int(xc - x, yc + y);
        pontos[2] = new Vector2Int(xc + x, yc - y);
        pontos[3] = new Vector2Int(xc - x, yc - y);
        pontos[4] = new Vector2Int(xc + y, yc + x);
        pontos[5] = new Vector2Int(xc - y, yc + x);
        pontos[6] = new Vector2Int(xc + y, yc - x);
        pontos[7] = new Vector2Int(xc - y, yc - x);

        foreach (var ponto in pontos)
        {
            if (!circulo.Contains(ponto))
            {
                drawingRenderer.DrawPixelCartesian(ponto, Color.red);
                circulo.Add(ponto);
            }
        }
    }
}
