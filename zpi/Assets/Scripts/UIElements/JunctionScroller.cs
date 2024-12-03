using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyAndCode.UI;



public class JunctionScroller : MultiMenuWrapper, IRecyclableScrollRectDataSource
{
    public PathManager pathManager;

    [SerializeField]
    RecyclableScrollRect _recyclableScrollRect;

    [SerializeField]
    private int _dataLength;

    //Dummy data List
    private List<JunctionInfo> _contactList = new List<JunctionInfo>();

    //Recyclable scroll rect's data source must be assigned in Awake.
    private void Awake()
    {
        if(pathManager != null)
        {
            GetJunctions();
        }
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
        if(pathManager != null)
        {
            var item = cell as JunctionCell;
            item.ConfigureCell(_contactList[index], index);
        }
    }

    #endregion
}
