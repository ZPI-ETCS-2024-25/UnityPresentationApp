using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public struct JunctionInfo
{
    public string Name;
    public int Position;
    public List<(int Index,string Name)> AllowedPositions;
    public bool Backward;
    public PathManager PathManager;
}


public class PathManager : MonoBehaviour
{
    public SplineContainer splineContainer;

    public Dictionary<int, List<(int Spline,bool Backward)>> path;
    public Dictionary<int, List<(int Spline,bool Backward)>> reversePath;
    public Dictionary<(int Spline, bool Backward), List<(int Spline, bool Backward)>> paths;

    public Dictionary<int,int> junctions;
    public Dictionary<int, int> reverseJunctions;
    public List<JunctionInfo> junctionInfo;

    public event Action JunctionChanged;

    public GameObject arrowPrefab;
    public List<GameObject> arrows;

    private void Awake()
    {
        UpdatePathfinding();
        GetJunctionInfo();
        CreateArrows();
    }


    public void UpdatePathfinding()
    {
        UpdatePath();
        UpdateReversePath();
        UpdateJunctions();
        UpdateReverseJunctions();
    }


    public void UpdatePath()
    {
        Dictionary<int, List<(int,bool)>> linkedSplines = new Dictionary<int, List<(int,bool)>>();
        for (int i = 0; i < splineContainer.Splines.Count; i++)
        {
            //Debug.Log($"rail {i}");
            Spline spline = splineContainer.Splines[i];
            SplineKnotIndex skiBase = new SplineKnotIndex(i, spline.Count - 1/**/);
            IReadOnlyList<SplineKnotIndex> linked = splineContainer.KnotLinkCollection.GetKnotLinks(skiBase);

            SplineData<UnityEngine.Object> rD;
            bool gotRailData = splineContainer.Splines[i].TryGetObjectData("RailData", out rD);
            RailData railData = rD[0].Value as RailData;

            //Debug.Log($"linked number {linked.Count}");

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
                            if (skiLinked.Knot != 0 && railData.endingKnotGroup/**/ != railDataOther.endingKnotGroup)
                            {
                                linkedSplines[i].Add((skiLinked.Spline, true));
                            }
                            else if(skiLinked.Knot == 0 && railData.endingKnotGroup/**/ != railDataOther.startingKnotGroup)
                            {
                                linkedSplines[i].Add((skiLinked.Spline, false)); ;
                            }
                        }
                    }
                }
            }
        }
        path = linkedSplines/**/;
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


    public void UpdatePathCombined()
    {
        Dictionary<(int,bool), List<(int, bool)>> paths = new Dictionary<(int,bool), List<(int, bool)>>();
        for (int i = 0; i < splineContainer.Splines.Count; i++)
        {
            Spline spline = splineContainer.Splines[i];
            SplineKnotIndex lastKnot = new SplineKnotIndex(i, spline.Count - 1);
            SplineKnotIndex firstKnot = new SplineKnotIndex(i, 0);

            List<(int, bool)> connectedSplinesForward = GetConnectedPaths(lastKnot,i,false);
            List<(int, bool)> connectedSplinesBackward = GetConnectedPaths(firstKnot, i, true);

            if(connectedSplinesForward.Count != 0)
            {
                paths[(i, false)] = connectedSplinesForward;
            }
            if (connectedSplinesBackward.Count != 0)
            {
                paths[(i, true)] = connectedSplinesBackward;
            }
        }
        this.paths = paths;
    }

    private List<(int, bool)> GetConnectedPaths(SplineKnotIndex knot,int splineIndex,bool backward)
    {
        List<(int, bool)> connectedSplines = new  List<(int, bool)>();
        IReadOnlyList<SplineKnotIndex> linkedKnots = splineContainer.KnotLinkCollection.GetKnotLinks(knot);

        SplineData<UnityEngine.Object> rD;
        bool gotRailData = splineContainer.Splines[splineIndex].TryGetObjectData("RailData", out rD);
        RailData railData = rD[0].Value as RailData;

        if(linkedKnots.Count != 1)
        {
            foreach (SplineKnotIndex skiLinked in linkedKnots)
            {
                int knotGroup = backward ? railData.startingKnotGroup : railData.endingKnotGroup;
                (int,bool)? info = GetInfoFromConnected(skiLinked,splineIndex,knotGroup);
                if (info != null) {
                    connectedSplines.Add(info.Value);
                }
            }
        }
        return connectedSplines;
    }

    private (int, bool)? GetInfoFromConnected(SplineKnotIndex knotInfo,int originalSplineIndex,int originalKnotGroup)
    {
        if (knotInfo.Spline != originalSplineIndex)
        {
            SplineData<UnityEngine.Object> rD;
            bool gotRailData = splineContainer.Splines[knotInfo.Spline].TryGetObjectData("RailData", out rD);
            RailData railData = rD[0].Value as RailData;

            if (gotRailData)
            {
                bool connectedKnotIsLast = knotInfo.Knot  != 0 ? originalKnotGroup != railData.endingKnotGroup : originalKnotGroup != railData.startingKnotGroup;
                if (connectedKnotIsLast)
                {
                    return (knotInfo.Spline, true);
                }
                else if (!connectedKnotIsLast)
                {
                    return (knotInfo.Spline,false);
                }
            }
        }
        return null;
    }



    private void UpdateJunctions()
    {
        junctions = new Dictionary<int, int>();
        foreach(int i in path.Keys)
        {
            if (path[i].Count > 1)
            {
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
                reverseJunctions[i] = 0;
            }
        }
    }


    private void GetJunctionInfo()
    {
        junctionInfo = new List<JunctionInfo>();
        foreach(int i in junctions.Keys)
        {
            SplineData<UnityEngine.Object> rD;
            bool gotRailData = splineContainer.Splines[i].TryGetObjectData("RailData", out rD);
            RailData railData = rD[0].Value as RailData;

            if (gotRailData)
            {
                List<(int, string)> allowedPositions = new List<(int, string)>();
                int count = 0;
                foreach ((int Spline,bool Backward) pair in path[i])
                {
                    SplineData<UnityEngine.Object> rDOther;
                    bool gotRailDataOther = splineContainer.Splines[pair.Spline].TryGetObjectData("RailData", out rDOther);
                    RailData railDataOther = rDOther[0].Value as RailData;

                    if (gotRailData)
                    {
                        allowedPositions.Add((count, railDataOther.name));
                    }
                    count++;
                }
                
                junctionInfo.Add(new JunctionInfo() { Name = railData.name, Position = i , AllowedPositions = allowedPositions,Backward = false,PathManager = this});
            }
            else
            {
                Debug.Log("junction info: rail data not present");
            }
        }
        foreach (int i in reverseJunctions.Keys)
        {
            SplineData<UnityEngine.Object> rD;
            bool gotRailData = splineContainer.Splines[i].TryGetObjectData("RailData", out rD);
            RailData railData = rD[0].Value as RailData;

            if (gotRailData)
            {
                List<(int, string)> allowedPositions = new List<(int, string)>();
                int count = 0;
                foreach ((int Spline, bool Backward) pair in reversePath[i])
                {
                    SplineData<UnityEngine.Object> rDOther;
                    bool gotRailDataOther = splineContainer.Splines[pair.Spline].TryGetObjectData("RailData", out rDOther);
                    RailData railDataOther = rDOther[0].Value as RailData;

                    if (gotRailData)
                    {
                        allowedPositions.Add((count, railDataOther.name));
                    }
                    count++;
                }

                junctionInfo.Add(new JunctionInfo() { Name = railData.name, Position = i ,AllowedPositions = allowedPositions,Backward = true,PathManager = this});
            }
            else
            {
                Debug.Log("junction info: rail data not present");
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

    private void RepositionArrows()
    {
        for(int i = 0; i<junctions.Count;i++)
        {
            List<int> keys = junctions.Keys.ToList();
            PositionArrow(arrows[i], keys[i], false);
        }
        for(int i = junctions.Count; i < reverseJunctions.Count + junctions.Count; i++)
        {
            List<int> keys = reverseJunctions.Keys.ToList();
            PositionArrow(arrows[i], keys[i], true);
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


    public void ChangeJunction(int index,bool backward,int changed)
    {
        //Debug.Log("changing");
        Dictionary<int, int> junctionsSource = backward ? reverseJunctions : junctions;
        junctionsSource[index] = changed;
        RepositionArrows();
        OnJunctionchanged();
    }
}