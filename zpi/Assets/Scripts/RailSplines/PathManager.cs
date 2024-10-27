using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class PathManager : MonoBehaviour
{
    public SplineContainer splineContainer;
    public Dictionary<int, List<int>> path;
    public Dictionary<int, List<int>> reversePath;

    private void Start()
    {
        UpdatePathfinding();       
    }

    public void UpdatePath()
    {
        Dictionary<int, List<int>> linkedSplines = new Dictionary<int, List<int>>();
        for (int i = 0; i < splineContainer.Splines.Count; i++)
        {
            Spline spline = splineContainer.Splines[i];
            SplineKnotIndex skiBase = new SplineKnotIndex(i, spline.Count - 1);
            IReadOnlyList<SplineKnotIndex> linked = splineContainer.KnotLinkCollection.GetKnotLinks(skiBase);
            if (linked.Count != 1)
            {
                linkedSplines[i] = new List<int>();
                foreach (SplineKnotIndex skiLinked in linked)
                {
                    if(skiLinked.Spline != i)
                    {
                        linkedSplines[i].Add(skiLinked.Spline);
                    }
                }
            }
        }
        path = linkedSplines;
    }

    public void UpdateReversePath()
    {
        reversePath = new Dictionary<int, List<int>>();
        foreach (int i in path.Keys)
        {
            foreach(int j in path[i])
            {
                if (reversePath.ContainsKey(j))
                {
                    reversePath[j].Add(i);
                }
                else
                {
                    reversePath[j] = new List<int>() { i};
                }
            }
        }
    }

    public void UpdatePathfinding()
    {
        UpdatePath();
        UpdateReversePath();
    }
}
