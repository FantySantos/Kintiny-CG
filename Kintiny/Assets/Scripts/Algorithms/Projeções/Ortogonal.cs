using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Ortogonal : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;

    [SerializeField]
    private LogManager logManager;

    [SerializeField]
    private Bresenham bresenham;

    public void Projetar(List<Vector3> pontos3D)
    {
        if (bresenham == null || pontos3D == null || pontos3D.Count == 0)
        {
            logManager.AddLogMessage("Erro: Dependências não atribuídas ou lista de pontos 3D inválida.");
            return;
        }

        StartCoroutine(ProjetarCoroutine(pontos3D));
    }

    private IEnumerator ProjetarCoroutine(List<Vector3> pontos3D)
    {
        logManager.AddLogMessage("Iniciando Projeção Ortográfica...");
        drawingRenderer.ClearCanvas();

        Matrix4x4 matrizProjecao = new(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, 0, 0),
            new Vector4(0, 0, 0, 1)
        );

        List<Vector2Int> pontosProjetados = new();

        logManager.AddLogMessage("Pontos 3D Originais | Pontos 2D Projetados:");
        foreach (var ponto3D in pontos3D)
        {
            var pontoHomogeneo = new Vector4(ponto3D.x, ponto3D.y, ponto3D.z, 1);

            var pontoProjetadoHomogeneo = matrizProjecao * pontoHomogeneo;

            var ponto2D = new Vector2Int(
                Mathf.RoundToInt(pontoProjetadoHomogeneo.x),
                Mathf.RoundToInt(pontoProjetadoHomogeneo.y)
            );

            pontosProjetados.Add(ponto2D);
            logManager.AddLogMessage($"{ponto3D} -> {ponto2D}");
        }

        logManager.AddLogMessage("Desenhando o resultado da projeção com Bresenham...");

        if (pontosProjetados.Count < 4 || pontosProjetados.Count % 2 != 0)
        {
            logManager.AddLogMessage("Erro: Número de pontos inválido para desenhar um objeto 3D com faces frontal e traseira.");
            yield return null;
        }
        else
        {
            int numPontosFace = pontosProjetados.Count / 2;

            for (int i = 0; i < numPontosFace - 1; i++)
            {
                bresenham.Desenhar(pontosProjetados[i].x, pontosProjetados[i].y, pontosProjetados[i + 1].x, pontosProjetados[i + 1].y, 0, false);
            }

            bresenham.Desenhar(pontosProjetados[numPontosFace - 1].x, pontosProjetados[numPontosFace - 1].y, pontosProjetados[0].x, pontosProjetados[0].y, 0, false);


            for (int i = numPontosFace; i < pontosProjetados.Count - 1; i++)
            {
                bresenham.Desenhar(pontosProjetados[i].x, pontosProjetados[i].y, pontosProjetados[i + 1].x, pontosProjetados[i + 1].y, 0, false);
            }

            bresenham.Desenhar(pontosProjetados[pontosProjetados.Count - 1].x, pontosProjetados[pontosProjetados.Count - 1].y, pontosProjetados[numPontosFace].x, pontosProjetados[numPontosFace].y, 0, false);


            for (int i = 0; i < numPontosFace; i++)
            {
                bresenham.Desenhar(pontosProjetados[i].x, pontosProjetados[i].y, pontosProjetados[i + numPontosFace].x, pontosProjetados[i + numPontosFace].y, 0, false);
            }
        }

        yield return null;
    }
}