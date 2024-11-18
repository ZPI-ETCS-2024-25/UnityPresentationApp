using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyAndCode.UI;



public class CrossingScroller : MultiMenuWrapper, IRecyclableScrollRectDataSource
{
    public CrossingManager crossingManager;

    [SerializeField]
    RecyclableScrollRect _recyclableScrollRect;

    [SerializeField]
    private int _dataLength;

    private List<CrossingInfo> _contactList = new List<CrossingInfo>();


    private void Awake()
    {
        GetCrossings();
        _recyclableScrollRect.DataSource = this;
    }


    private void GetCrossings()
    {
        _contactList = crossingManager.GetCrossingInfos();
        _dataLength = _contactList.Count;
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
        var item = cell as CrossingCell;
        item.ConfigureCell(_contactList[index], index);
    }

    #endregion
}
