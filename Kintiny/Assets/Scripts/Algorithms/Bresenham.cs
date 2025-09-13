using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bresenham : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;

    [SerializeField]
    private LogManager logManager;

    private float delayPassos;

    private int x, y;

    private int x1, y1, x2, y2;
    private float deltaX, deltaY, m, e;
    private bool trocaXY, trocaX, trocaY, clear;

    private List<Vector2Int> linha = new List<Vector2Int>();

    public void Desenhar(int x1, int y1, int x2, int y2, float delayPassos, bool clear = true)
    {
        if (drawingRenderer == null) return;

        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
        this.delayPassos = delayPassos;
        this.clear = clear;

        StartCoroutine(DesenharCoroutine());
    }

    private IEnumerator DesenharCoroutine()
    {
        logManager.AddLogMessage($"Iniciando desenho da linha de ({x1}, {y1}) a ({x2}, {y2}) com Bresenham.");
        linha.Clear();
        trocaXY = false;
        trocaX = false;
        trocaY = false;

        Reflexao();

        x = x1;
        y = y1;

        deltaX = x2 - x1;
        deltaY = y2 - y1;
        m = (deltaX == 0) ? 0 : deltaY / deltaX;
        e = m - 0.5f;

        if (clear)
            drawingRenderer.DrawPixelCartesian(new Vector2Int(x, y), Color.red);
        linha.Add(ReflexaoVolta(x, y));

        while (x < x2)
        {
            if (delayPassos > 0)
                yield return new WaitForSeconds(delayPassos);

            if (e > 0)
            {
                y++;
                e -= 1f;
            }

            x++;
            e += m;

            if (clear)
                drawingRenderer.DrawPixelCartesian(new Vector2Int(x, y), Color.red);

            linha.Add(ReflexaoVolta(x, y));
        }

        if (clear)
            drawingRenderer.ClearCanvas(false);

        foreach (Vector2Int point in linha)
            drawingRenderer.DrawPixelCartesian(point);

        logManager.AddLogMessage("Linha desenhada com sucesso!");
    }

    private void Reflexao()
    {
        deltaX = x2 - x1;
        deltaY = y2 - y1;

        if (Mathf.Abs(deltaY) > Mathf.Abs(deltaX))
        {
            (x1, y1) = (y1, x1);
            (x2, y2) = (y2, x2);
            trocaXY = true;
        }
        if (x1 > x2)
        {
            x1 *= -1;
            x2 *= -1;
            trocaX = true;
        }
        if (y1 > y2)
        {
            y1 *= -1;
            y2 *= -1;
            trocaY = true;
        }
    }

    private Vector2Int ReflexaoVolta(int x, int y)
    {
        if (trocaY) y *= -1;
        if (trocaX) x *= -1;
        if (trocaXY) (x, y) = (y, x);

        return new Vector2Int(x, y);
    }
}