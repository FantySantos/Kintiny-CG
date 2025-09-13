using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perspectiva : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;

    [SerializeField]
    private LogManager logManager;

    [SerializeField]
    private Bresenham bresenham;

    public void Projetar(List<Vector3> pontos3D, float distanciaFocal)
    {
        if (bresenham == null || drawingRenderer == null || logManager == null || pontos3D == null || pontos3D.Count == 0)
        {
            logManager?.AddLogMessage("Erro: Dependências não atribuídas ou lista de pontos 3D inválida.");
            return;
        }

        if (distanciaFocal == 0)
        {
            logManager?.AddLogMessage("Erro: A distância focal não pode ser zero.");
            return;
        }

        StartCoroutine(ProjetarCoroutine(pontos3D, distanciaFocal));
    }

    private IEnumerator ProjetarCoroutine(List<Vector3> pontos3D, float d)
    {
        logManager.AddLogMessage($"Iniciando Projeção Perspectiva com distância focal d = {d}.");
        drawingRenderer.ClearCanvas();
        yield return null;

        Matrix4x4 matrizProjecao = new()
        {
            m00 = 1,
            m11 = 1,
            m22 = 0,
            m32 = 1/d,
            m33 = 0
        };

        var pontosProjetados = new List<Vector2Int>();
        logManager.AddLogMessage("Projetando pontos 3D para o plano 2D...");

        foreach (var ponto3D in pontos3D)
        {
            if (ponto3D.z == 0)
            {
                logManager.AddLogMessage($"Aviso: Ponto {ponto3D} está em z=0 e não pode ser projetado. Ponto ignorado.");
                continue;
            }

            var pontoHomogeneo = new Vector4(ponto3D.x, ponto3D.y, ponto3D.z, 1);
            var pontoProjetadoHomogeneo = matrizProjecao * pontoHomogeneo;

            float w = pontoProjetadoHomogeneo.w;
            if (w == 0) continue;

            var ponto2D = new Vector2Int(
                Mathf.RoundToInt(pontoProjetadoHomogeneo.x / w),
                Mathf.RoundToInt(pontoProjetadoHomogeneo.y / w)
            );

            pontosProjetados.Add(ponto2D);
        }

        logManager.AddLogMessage("Desenhando o resultado da projeção com o algoritmo de Bresenham...");
        yield return null;

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
        logManager.AddLogMessage("Projeção Perspectiva concluída.");
        yield return null;
    }
}