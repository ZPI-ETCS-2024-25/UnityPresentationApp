using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class PathManager : MonoBehaviour
{
    public SplineContainer splineContainer;

    public Dictionary<int, List<(int Spline,bool Backward)>> path;
    public Dictionary<int, List<(int Spline,bool Backward)>> reversePath;

    public Dictionary<int,int> junctions;
    public Dictionary<int, int> reverseJunctions;
    public event Action JunctionChanged;

    public GameObject arrowPrefab;
    private List<GameObject> arrows;

    private void Start()
    {
        UpdatePathfinding();
        CreateArrows();
    }


    public void UpdatePathfinding()
    {
        UpdatePath();
        UpdateReversePath();
        UpdateJunctions();
        UpdateReverseJunctions();
        //Debug.Log("cos");
    }


    public void UpdatePath()
    {
        Dictionary<int, List<(int,bool)>> linkedSplines = new Dictionary<int, List<(int,bool)>>();
        for (int i = 0; i < splineContainer.Splines.Count; i++)
        {
            Spline spline = splineContainer.Splines[i];
            SplineKnotIndex skiBase = new SplineKnotIndex(i, spline.Count - 1);
            IReadOnlyList<SplineKnotIndex> linked = splineContainer.KnotLinkCollection.GetKnotLinks(skiBase);

            SplineData<UnityEngine.Object> rD;
            bool gotRailData = splineContainer.Splines[i].TryGetObjectData("RailData", out rD);
            RailData railData = rD[0].Value as RailData;

            if (linked.Count != 1)
            {
                linkedSplines[i] = new List<(int,bool)>();
                foreach (SplineKnotIndex skiLinked in linked)
                {
                    if(skiLinked.Spline != i)
                    {

                        SplineData<UnityEngine.Object> rDOther;
                        bool gotRailDataOther = splineContainer.Splines[skiLinked.Spline].TryGetObjectData("RailData", out rDOther);
                        RailData railDataOther = rDOther[0].Value as RailData;

                        if (gotRailData && gotRailDataOther)
                        {
                            if (skiLinked.Knot != 0 && railData.endingKnotGroup != railDataOther.endingKnotGroup)
                            {
                                linkedSplines[i].Add((skiLinked.Spline, true));
                            }
                            else if(skiLinked.Knot == 0 && railData.endingKnotGroup != railDataOther.startingKnotGroup)
                            {
                                linkedSplines[i].Add((skiLinked.Spline, false)); ;
                            }
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


            SplineData<UnityEngine.Object> rD;
            bool gotRailData = splineContainer.Splines[i].TryGetObjectData("RailData", out rD);
            RailData railData = rD[0].Value as RailData;


            if (linked.Count != 1)
            {
                linkedSplines[i] = new List<(int,bool)>();
                foreach (SplineKnotIndex skiLinked in linked)
                {
                    if (skiLinked.Spline != i)
                    {

                        SplineData<UnityEngine.Object> rDOther;
                        bool gotRailDataOther = splineContainer.Splines[skiLinked.Spline].TryGetObjectData("RailData", out rDOther);
                        RailData railDataOther = rDOther[0].Value as RailData;

                        if (gotRailData && gotRailDataOther)
                        {
                            if (skiLinked.Knot != 0 && railData.startingKnotGroup != railDataOther.endingKnotGroup)
                            {
                                linkedSplines[i].Add((skiLinked.Spline, true));
                            }
                            else if (skiLinked.Knot == 0 && railData.startingKnotGroup != railDataOther.startingKnotGroup)
                            {
                                linkedSplines[i].Add((skiLinked.Spline, false)); ;
                            }
                        }
                    }
                }
            }
        }
        reversePath = linkedSplines;
    }

    private void UpdateJunctions()
    {
        junctions = new Dictionary<int, int>();
        foreach(int i in path.Keys)
        {
            if (path[i].Count > 1)
            {
                //junctions[i] = path[i][0].Spline;
                junctions[i] = 0;
            }
        }
    }

    private void UpdateReverseJunctions()
    {
        reverseJunctions = new Dictionary<int, int>();
        foreach (int i in reversePath.Keys)
        {
            if (reversePath[i].Count > 1)
            {
                //junctions[i] = reversePath[i][0].Spline;
                reverseJunctions[i] = 0;
            }
        }
    }


    public int GetNextSplineIndex(int currentSpline,bool currentBackward)
    {
        Dictionary<int, int> junctionsSource = currentBackward ? reverseJunctions : junctions;
        Dictionary<int, List<(int Spline, bool Backward)>> pathSource = currentBackward ? reversePath : path;
        if (junctionsSource.ContainsKey(currentSpline))
        {
            return junctionsSource[currentSpline]; //there is junction
        }
        else
        {
            if (pathSource.ContainsKey(currentSpline))
            {
                return 0; //there is no junction but there is path
            }
            else
            {
                return -1; //end of the line
            }
        }
    }


    public void CreateArrows()
    {
        arrows = new List<GameObject>();
        foreach (int i in junctions.Keys)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            PositionArrow(arrow,i,false);
            arrows.Add(arrow);
        }

        foreach (int i in reverseJunctions.Keys)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            PositionArrow(arrow, i, true);
            arrows.Add(arrow);
        }
    }


    private void PositionArrow(GameObject arrow, int spline,bool  backward)
    {
        UnityEngine.Vector3 localPosition = backward ? splineContainer[spline][0].Position 
                                                     : splineContainer[spline][  splineContainer[spline].Count-1  ].Position;
        UnityEngine.Vector3 worldPosition = splineContainer.transform.TransformPoint(localPosition);


        int nextSplinePathIndex = backward ? reverseJunctions[spline] : junctions[spline];
        int nextSplineContainerIndex = backward ? reversePath[spline][nextSplinePathIndex].Spline 
                                                : path[spline][nextSplinePathIndex].Spline;
        UnityEngine.Vector3 localPositionNext = backward ? splineContainer[nextSplineContainerIndex][1].Position
                                                         : splineContainer[nextSplineContainerIndex][splineContainer[nextSplineContainerIndex].Count - 2].Position;
        UnityEngine.Vector3 worldPositionNext = splineContainer.transform.TransformPoint(localPositionNext);

        UnityEngine.Vector3 direction = worldPositionNext - worldPosition;
        arrow.transform.position = worldPosition;
        arrow.transform.rotation = UnityEngine.Quaternion.LookRotation(direction, transform.up);
        arrow.transform.Rotate(0, 90, 0); //correct rotation
    }


    protected virtual void OnJunctionchanged()
    {
        JunctionChanged?.Invoke();
    }
}
