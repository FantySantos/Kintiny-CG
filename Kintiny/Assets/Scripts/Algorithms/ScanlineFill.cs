using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScanlineFill : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;

    [SerializeField]
    private LogManager logManager;

    private float delayPassos;

    private class ArestaAtiva
    {
        public int y_max;
        public float x_interseccao;
        public float inv_slope;
    }

    public void IniciarPreenchimento(List<Vector2Int> vertices, float delayPassos)
    {
        if (drawingRenderer == null || vertices == null || vertices.Count < 3)
        {
            logManager.AddLogMessage("Erro: DrawingRenderer não atribuído ou polígono inválido.");
            return;
        }

        this.delayPassos = delayPassos;
        StartCoroutine(ScanlineCoroutine(vertices));
    }

    private IEnumerator ScanlineCoroutine(List<Vector2Int> vertices)
    {
        logManager.AddLogMessage("Iniciando preenchimento com Scanline.");

        int y_min_global = vertices.Min(v => v.y);
        int y_max_global = vertices.Max(v => v.y);

        Dictionary<int, List<ArestaAtiva>> tabelaDeArestas = new();
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2Int p1 = vertices[i];
            Vector2Int p2 = vertices[(i + 1) % vertices.Count];

            if (p1.y == p2.y) continue;

            Vector2Int v_min = (p1.y < p2.y) ? p1 : p2;
            Vector2Int v_max = (p1.y < p2.y) ? p2 : p1;
            float invSlope = (float)(p2.x - p1.x) / (p2.y - p1.y);

            if (!tabelaDeArestas.ContainsKey(v_min.y))
                tabelaDeArestas[v_min.y] = new List<ArestaAtiva>();

            tabelaDeArestas[v_min.y].Add(new ArestaAtiva { y_max = v_max.y, x_interseccao = v_min.x, inv_slope = invSlope });
        }

        List<ArestaAtiva> arestasAtivas = new();
        for (int y = y_min_global; y < y_max_global; y++)
        {
            if (tabelaDeArestas.ContainsKey(y))
                arestasAtivas.AddRange(tabelaDeArestas[y]);

            arestasAtivas.RemoveAll(aresta => aresta.y_max == y);
            arestasAtivas = arestasAtivas.OrderBy(a => a.x_interseccao).ToList();

            for (int i = 0; i < arestasAtivas.Count; i += 2)
            {
                int x_start = Mathf.RoundToInt(arestasAtivas[i].x_interseccao);
                int x_end = Mathf.RoundToInt(arestasAtivas[i + 1].x_interseccao);
                logManager.AddLogMessage($"Preenchendo linha y={y} de x={x_start} a x={x_end}.");

                for (int x = x_start; x < x_end; x++)
                {
                    Vector2Int pixelPosition = new(x, y);
                    Color currentPixelColor = drawingRenderer.GetPixelColorCartesian(pixelPosition);
                    
                    if (currentPixelColor != Color.black)
                        drawingRenderer.DrawPixelCartesian(pixelPosition, Color.blue);
                }
            }

            foreach (var aresta in arestasAtivas)
            {
                aresta.x_interseccao += aresta.inv_slope;
            }

            if (delayPassos > 0)
                yield return new WaitForSeconds(delayPassos);
        }

        logManager.AddLogMessage("Preenchimento concluído!");
    }
}