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
    public UnityServerComm comm;
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
            Spline spline = splineContainer.Splines[i];
            SplineKnotIndex skiBase = new SplineKnotIndex(i, spline.Count - 1);
            IReadOnlyList<SplineKnotIndex> linked = splineContainer.KnotLinkCollection.GetKnotLinks(skiBase);

            SplineData<UnityEngine.Object> rD;
            bool gotRailData = splineContainer.Splines[i].TryGetObjectData("RailData", out rD);
            RailData railData = rD[0].Value as RailData;

            if (linked.Count != 1) //are we connected more than 1 knots
            {
                linkedSplines[i] = new List<(int,bool)>();
                foreach (SplineKnotIndex skiLinked in linked)
                {
                    if (skiLinked.Knot >= spline.Count)//prevent ghost knots
                    {
                        continue;
                    }
                    if(skiLinked.Spline != i) //knot different from myself
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
                if (linkedSplines[i].Count == 0)//prevent empty paths
                {
                    linkedSplines.Remove(i);
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
            //Debug.Log(i);
            if (path[i].Count > 1)
            {
                SplineData<UnityEngine.Object> rD;
                bool gotRailData = splineContainer.Splines[i].TryGetObjectData("RailData", out rD);
                RailData railData = rD[0].Value as RailData;

                if (gotRailData)
                {
                    junctions[i] = railData.junctionDataForward.straightIndex;
                    //Debug.Log(junctions[i]);
                }
                else
                {
                    junctions[i] = 0;
                }
            }
        }
    }

    private void UpdateReverseJunctions()
    {
        reverseJunctions = new Dictionary<int, int>();
        foreach (int i in reversePath.Keys)
        {
            //Debug.Log(i);
            if (reversePath[i].Count > 1)
            {
                SplineData<UnityEngine.Object> rD;
                bool gotRailData = splineContainer.Splines[i].TryGetObjectData("RailData", out rD);
                RailData railData = rD[0].Value as RailData;

                if (gotRailData)
                {
                    reverseJunctions[i] = railData.junctionDataBackward.straightIndex;
                    //Debug.Log($"spline {i} junction is set at index {reverseJunctions[i]} that is spline {reversePath[i][reverseJunctions[i]]}");
                }
                else
                {
                    reverseJunctions[i] = 0;
                }
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


    public int GetJunctionCurrentPosition(int junction,bool backward)
    {
        Dictionary<int, int> junctionsSource = backward ? reverseJunctions : junctions;
        if (junctionsSource.Keys.Contains(junction)){
            return junctionsSource[junction];
        }
        return -1;
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
            arrow.name = $"Arrow-junction{i}";
            PositionArrow(arrow,i,false);
            arrows.Add(arrow);
        }

        foreach (int i in reverseJunctions.Keys)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.name = $"Arrow-reverseJunction{i}";
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
            PositionArrow(arrows[i], keys[i-junctions.Count], true);
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
        bool nextSplineBackward = backward ? reversePath[spline][nextSplinePathIndex].Backward
                                                : path[spline][nextSplinePathIndex].Backward;
        UnityEngine.Vector3 localPositionNext = nextSplineBackward ? splineContainer[nextSplineContainerIndex][splineContainer[nextSplineContainerIndex].Count - 2].Position
                                                                   : splineContainer[nextSplineContainerIndex][1].Position;
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
        Dictionary<int, int> junctionsSource = backward ? reverseJunctions : junctions;
        string dir = backward ? "back" : "forward";
        Debug.Log($"changing {dir} {index} into {changed}");
        junctionsSource[index] = changed;
        RepositionArrows();
        OnJunctionchanged();

        SplineData<UnityEngine.Object> rD;
        bool gotRailData = splineContainer.Splines[index].TryGetObjectData("RailData", out rD);
        RailData railData = rD[0].Value as RailData;

        JunctionData junctionData = backward ? railData.junctionDataBackward : railData.junctionDataForward;

        if (railData != null) {
            comm.SendJunctionState(100 +junctionData.junctionGroup*10+junctionData.junctionNumber, changed == junctionData.straightIndex ? true : false);
        }
    }

    public void DebugPrintPathing()
    {
        Debug.Log("Path forward");
        DebugPrintPath(path);
        Debug.Log("Path backward");
        DebugPrintPath(reversePath);
        Debug.Log("Junctions forward");
        DebugPrintJunctions(junctions,path);
        Debug.Log("Junctions backward");
        DebugPrintJunctions(reverseJunctions, reversePath);
    }

    public void DebugPrintPath(Dictionary<int, List<(int Spline, bool Backward)>> path)
    {
        UpdatePathfinding();
        foreach(int spline in path.Keys)
        {
            string nextSplines = "";
            foreach((int,bool) pair in path[spline])
            {
                nextSplines += $"({pair.Item1},{pair.Item2})";
            }
            Debug.Log($"Spline {spline} can go : {nextSplines}");
        }
    }

    public void DebugPrintJunctions(Dictionary<int, int> junctions, Dictionary<int, List<(int Spline, bool Backward)>> path)
    {
        UpdatePathfinding();
        foreach (int spline in junctions.Keys)
        {
            Debug.Log($"Junction at spline {spline} is currently set at : {path[spline][junctions[spline]]}");
        }
    }
}