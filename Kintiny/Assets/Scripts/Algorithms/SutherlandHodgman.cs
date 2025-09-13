using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SutherlandHodgman : MonoBehaviour
{
    [SerializeField]
    private DrawingRenderer drawingRenderer;

    [SerializeField]
    private LogManager logManager;

    [SerializeField]
    private Polilinha polilinha;

    private float x_min, y_min, x_max, y_max;
    private List<Vector2Int> pontosJanela;

    private enum ClipEdge { Left, Right, Bottom, Top };

    public bool Clip(ref List<Vector2Int> polygonVertices)
    {
        logManager.AddLogMessage($"Iniciando recorte com Sutherland-Hodgman. Janela: ({x_min},{y_min}) a ({x_max},{y_max}).");

        var clipEdges = new[] { ClipEdge.Left, ClipEdge.Right, ClipEdge.Bottom, ClipEdge.Top };
        List<Vector2> currentPolygon = polygonVertices.Select(p => (Vector2)p).ToList();

        logManager.AddLogMessage($"Polígono inicial com {currentPolygon.Count} vértices.");

        currentPolygon = ClipPolygonAgainstEdge(currentPolygon, ClipEdge.Left);
        logManager.AddLogMessage($"Após recorte 'Left', o polígono tem {currentPolygon.Count} vértices.");

        currentPolygon = ClipPolygonAgainstEdge(currentPolygon, ClipEdge.Right);
        logManager.AddLogMessage($"Após recorte 'Right', o polígono tem {currentPolygon.Count} vértices.");

        currentPolygon = ClipPolygonAgainstEdge(currentPolygon, ClipEdge.Bottom);
        logManager.AddLogMessage($"Após recorte 'Bottom', o polígono tem {currentPolygon.Count} vértices.");

        currentPolygon = ClipPolygonAgainstEdge(currentPolygon, ClipEdge.Top);
        logManager.AddLogMessage($"Polígono final com {currentPolygon.Count} vértices.");

        if (currentPolygon.Count > 0)
        {
            List<Vector2Int> finalPolygonInts = currentPolygon.Select(p => new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y))).ToList();
            finalPolygonInts.Add(finalPolygonInts[0]);
            polygonVertices = finalPolygonInts;
            return true;
        }
        else
            logManager.AddLogMessage("O polígono foi totalmente recortado e está fora da janela.");
            return false;
    }

    private List<Vector2> ClipPolygonAgainstEdge(List<Vector2> inputPolygon, ClipEdge edge)
    {
        if (inputPolygon.Count == 0) return inputPolygon;

        List<Vector2> outputPolygon = new();
        Vector2 p1 = inputPolygon[^1];

        foreach (Vector2 p2 in inputPolygon)
        {
            bool p1_inside = IsInside(p1, edge);
            bool p2_inside = IsInside(p2, edge);

            if (p1_inside && p2_inside)
                outputPolygon.Add(p2);

            else if (p1_inside && !p2_inside)
                outputPolygon.Add(Intersect(p1, p2, edge));

            else if (!p1_inside && p2_inside)
            {
                outputPolygon.Add(Intersect(p1, p2, edge));
                outputPolygon.Add(p2);
            }

            p1 = p2;
        }

        return outputPolygon;
    }

    private bool IsInside(Vector2 p, ClipEdge edge)
    {
        switch (edge)
        {
            case ClipEdge.Left: return p.x >= x_min;
            case ClipEdge.Right: return p.x <= x_max;
            case ClipEdge.Bottom: return p.y >= y_min;
            case ClipEdge.Top: return p.y <= y_max;
            default: return false;
        }
    }

    private Vector2 Intersect(Vector2 p1, Vector2 p2, ClipEdge edge)
    {
        float x = 0, y = 0;
        float dx = p2.x - p1.x;
        float dy = p2.y - p1.y;

        switch (edge)
        {
            case ClipEdge.Left:
                x = x_min;
                y = p1.y + dy * (x_min - p1.x) / dx;
                break;
            case ClipEdge.Right:
                x = x_max;
                y = p1.y + dy * (x_max - p1.x) / dx;
                break;
            case ClipEdge.Bottom:
                y = y_min;
                x = p1.x + dx * (y_min - p1.y) / dy;
                break;
            case ClipEdge.Top:
                y = y_max;
                x = p1.x + dx * (y_max - p1.y) / dy;
                break;
        }
        return new Vector2(x, y);
    }

    public void DesenharJanelaDeRecorte(Vector2Int windowMin, Vector2Int windowMax)
    {
        pontosJanela?.Clear();

        this.x_min = windowMin.x;
        this.y_min = windowMin.y;
        this.x_max = windowMax.x;
        this.y_max = windowMax.y;

        logManager.AddLogMessage($"Janela de recorte definida de ({x_min},{y_min}) até ({x_max},{y_max}).");

        DesenharJanela(windowMin, windowMax);
    }

    public void DesenharJanela(Vector2Int windowMin, Vector2Int windowMax)
    {
        Vector2Int p1 = new(Mathf.RoundToInt(windowMin.x), Mathf.RoundToInt(windowMin.y));
        Vector2Int p2 = new(Mathf.RoundToInt(windowMax.x), Mathf.RoundToInt(windowMin.y));
        Vector2Int p3 = new(Mathf.RoundToInt(windowMax.x), Mathf.RoundToInt(windowMax.x));
        Vector2Int p4 = new(Mathf.RoundToInt(windowMin.x), Mathf.RoundToInt(windowMax.y));

        pontosJanela = new() { p1, p2, p3, p4, p1 };

        polilinha.Desenhar(pontosJanela, 0);

        logManager.AddLogMessage($"Janela de recorte desenhada com os pontos: ({p1.x},{p1.y}), ({p2.x},{p2.y}), ({p3.x},{p3.y}), ({p4.x},{p4.y}).");
    }
}