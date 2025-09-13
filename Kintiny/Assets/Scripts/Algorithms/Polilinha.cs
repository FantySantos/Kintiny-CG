using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polilinha : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;

    [SerializeField]
    private LogManager logManager;

    [SerializeField]
    private Bresenham bresenham;

    private List<Vector2Int> pontos;
    private float delayPassos;

    public void Desenhar(List<Vector2Int> pontos, float delayPassos)
    {
        this.pontos = new List<Vector2Int>(pontos);
        this.delayPassos = delayPassos;

        StartCoroutine(DesenharCoroutine());
    }

    private IEnumerator DesenharCoroutine()
    {
        logManager.AddLogMessage($"Iniciando desenho da polilinha com {pontos.Count} pontos.");

        for (int i = 0; i < pontos.Count - 1; i++)
        {
            Vector2Int p1 = pontos[i];
            Vector2Int p2 = pontos[i + 1];

            bresenham.Desenhar(p1.x, p1.y, p2.x, p2.y, 0, false);

            yield return new WaitForSeconds(delayPassos);
        }

        Vector2Int ultimoPonto = pontos[pontos.Count - 1];
        Vector2Int primeiroPonto = pontos[0];

        bresenham.Desenhar(ultimoPonto.x, ultimoPonto.y, primeiroPonto.x, primeiroPonto.y, 0, false);

        yield return new WaitForSeconds(delayPassos);

        logManager.AddLogMessage("Desenho da polilinha concluído!");
    }
}