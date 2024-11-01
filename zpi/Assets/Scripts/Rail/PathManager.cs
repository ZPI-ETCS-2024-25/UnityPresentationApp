using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class PathManager : MonoBehaviour
{
    public SplineContainer splineContainer;
    public Dictionary<int, List<(int Spline,bool Backward)>> path;
    public Dictionary<int, List<(int Spline,bool Backward)>> reversePath;

    private void Start()
    {
        UpdatePathfinding();       
    }

    public void UpdatePath()
    {
        Dictionary<int, List<(int,bool)>> linkedSplines = new Dictionary<int, List<(int,bool)>>();
        for (int i = 0; i < splineContainer.Splines.Count; i++)
        {
            Spline spline = splineContainer.Splines[i];
            SplineKnotIndex skiBase = new SplineKnotIndex(i, spline.Count - 1);
            IReadOnlyList<SplineKnotIndex> linked = splineContainer.KnotLinkCollection.GetKnotLinks(skiBase);
            if (linked.Count != 1)
            {
                linkedSplines[i] = new List<(int,bool)>();
                foreach (SplineKnotIndex skiLinked in linked)
                {
                    if(skiLinked.Spline != i)
                    {
                        if(skiLinked.Knot != 0)
                        {
                            linkedSplines[i].Add((skiLinked.Spline,true));
                        }
                        else
                        {
                            linkedSplines[i].Add((skiLinked.Spline, false));;
                        }
                    }
                }
            }
        }
        path = linkedSplines;
    }

    public void UpdateReversePath()
    {
        Dictionary<int, List<(int,bool)>> linkedSplines = new Dictionary<int, List<(int,bool)>>();
        for (int i = 0; i < splineContainer.Splines.Count; i++)
        {
            Spline spline = splineContainer.Splines[i];
            SplineKnotIndex skiBase = new SplineKnotIndex(i, 0);
            IReadOnlyList<SplineKnotIndex> linked = splineContainer.KnotLinkCollection.GetKnotLinks(skiBase);
            if (linked.Count != 1)
            {
                linkedSplines[i] = new List<(int,bool)>();
                foreach (SplineKnotIndex skiLinked in linked)
                {
                    if (skiLinked.Spline != i)
                    {

                        if (skiLinked.Knot != 0)
                        {
                            linkedSplines[i].Add((skiLinked.Spline, true));
                        }
                        else
                        {
                            linkedSplines[i].Add((skiLinked.Spline, false)); ;
                        }
                    }
                }
            }
        }
        reversePath = linkedSplines;
    }

    public void UpdatePathfinding()
    {
        UpdatePath();
        UpdateReversePath();
        //Debug.Log("cos");
    }
}
