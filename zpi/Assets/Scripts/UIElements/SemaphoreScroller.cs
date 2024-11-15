using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyAndCode.UI;



public class SemaphoreScroller : MonoBehaviour, IRecyclableScrollRectDataSource
{
    public SemaphoreManager semaphoreManager;

    [SerializeField]
    RecyclableScrollRect _recyclableScrollRect;

    [SerializeField]
    private int _dataLength;

    private List<SemaphoreInfo> _contactList = new List<SemaphoreInfo>();


    private void Awake()
    {
        GetSemaphores();
        //InitData();
        _recyclableScrollRect.DataSource = this;
    }


    private void GetSemaphores()
    {
        _contactList = semaphoreManager.GetSempahoreInfos();
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
        var item = cell as SemaphoreCell;
        item.ConfigureCell(_contactList[index], index);
    }

    #endregion
}
