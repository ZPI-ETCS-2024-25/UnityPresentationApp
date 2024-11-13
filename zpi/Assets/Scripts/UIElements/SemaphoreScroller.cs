using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyAndCode.UI;



public class SemaphoreScroller : MonoBehaviour, IRecyclableScrollRectDataSource
{
    public PathManager pathManager;

    [SerializeField]
    RecyclableScrollRect _recyclableScrollRect;

    [SerializeField]
    private int _dataLength;

    private List<JunctionInfo> _contactList = new List<JunctionInfo>();


    private void Awake()
    {
        GetJunctions();
        //InitData();
        _recyclableScrollRect.DataSource = this;
    }


    private void GetJunctions()
    {
        _dataLength = pathManager.junctionInfo.Count;
        _contactList = pathManager.junctionInfo;
    }

    #region DATA-SOURCE

    /// <summary>
    /// Data source method. return the list length.
    /// </summary>
    public int GetItemCount()
    {
        return _contactList.Count;
    }

    /// <summary>
    /// Data source method. Called for a cell every time it is recycled.
    /// Implement this method to do the necessary cell configuration.
    /// </summary>
    public void SetCell(ICell cell, int index)
    {
        //Casting to the implemented Cell
        var item = cell as JunctionCell;
        item.ConfigureCell(_contactList[index], index);
    }

    #endregion
}
