using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;

    [SerializeField]
    private LogManager logManager;

    private float delayPassos;
    private List<Vector2Int> pontosDeControle;
    private int numeroDeSegmentos;

    private List<Vector2Int> curva = new List<Vector2Int>();

    public void Desenhar(List<Vector2Int> pontosDeControle, int numeroDeSegmentos, float delayPassos)
    {
        if (drawingRenderer == null || pontosDeControle == null || pontosDeControle.Count < 2)
        {
            logManager.AddLogMessage("Erro: Pontos de controle insuficientes.");
            return;
        }

        this.pontosDeControle = new List<Vector2Int>(pontosDeControle);
        this.numeroDeSegmentos = numeroDeSegmentos;
        this.delayPassos = delayPassos - 0.5f;

        StartCoroutine(DesenharCoroutine());
    }

    private IEnumerator DesenharCoroutine()
    {
        logManager.AddLogMessage($"Iniciando desenho da curva de Bézier com {pontosDeControle.Count} pontos de controle e {numeroDeSegmentos} segmentos.");
        curva.Clear();
        drawingRenderer.ClearCanvas();

        foreach (var p in pontosDeControle)
            drawingRenderer.DrawPixelCartesian(p, Color.blue);

        Vector2Int pontoAnterior = pontosDeControle[0];
        curva.Add(pontoAnterior);
        drawingRenderer.DrawPixelCartesian(pontoAnterior, Color.red);

        yield return new WaitForSeconds(delayPassos);

        for (int i = 1; i <= numeroDeSegmentos; i++)
        {
            float t = (float)i / numeroDeSegmentos;

            Vector2 pontoFlutuante = CalcularPontoBezier(pontosDeControle, t);
            Vector2Int pontoAtual = new(Mathf.RoundToInt(pontoFlutuante.x), Mathf.RoundToInt(pontoFlutuante.y));

            if (pontoAtual != pontoAnterior)
            {
                drawingRenderer.DrawPixelCartesian(pontoAtual, Color.red);
                curva.Add(pontoAtual);
                pontoAnterior = pontoAtual;
            }

            if (delayPassos > 0)
                yield return new WaitForSeconds(delayPassos);
        }

        drawingRenderer.ClearCanvas();
        foreach (var ponto in curva)
            drawingRenderer.DrawPixelCartesian(ponto);

        logManager.AddLogMessage("Curva de Bézier desenhada com sucesso!");
    }

    private Vector2 CalcularPontoBezier(List<Vector2Int> ctrlPts, float t)
    {
        int n = ctrlPts.Count - 1;

        List<Vector2> pts = ctrlPts.Select(p => (Vector2)p).ToList();

        for (int r = 1; r <= n; r++)
        {
            for (int i = 0; i <= n - r; i++)
            {
                //p_i^r(t) = (1-t) * p_i^(r-1)(t) + t * p_(i+1)^(r-1)(t) 
                pts[i] = (1 - t) * pts[i] + t * pts[i + 1];
            }
        }

        return pts[0];
    }
}