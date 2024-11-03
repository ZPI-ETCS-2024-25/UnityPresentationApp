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

    public void UpdatePathfinding()
    {
        UpdatePath();
        UpdateReversePath();
        //Debug.Log("cos");
    }
}
