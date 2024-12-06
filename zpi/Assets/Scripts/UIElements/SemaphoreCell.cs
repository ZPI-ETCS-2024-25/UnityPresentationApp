using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyAndCode.UI;
using UnityEngine.UI;
using TMPro;

public class SemaphoreCell : MonoBehaviour, ICell
{
    public TMP_Text nameLabel;
    public TMP_Dropdown position;

    //Model
    private SemaphoreInfo _contactInfo;
    private int _cellIndex;

    private void Start()
    {
        var dropdownRectTransform = position.GetComponent<RectTransform>();
        dropdownRectTransform.localScale = new Vector3(2f, 2f, 1f);

        nameLabel.fontSize = 120f;
        nameLabel.color = Color.black;
    }
    //This is called from the SetCell method in DataSource
    public void ConfigureCell(SemaphoreInfo contactInfo, int cellIndex)
    {
        _cellIndex = cellIndex;
        _contactInfo = contactInfo;

        nameLabel.text = contactInfo.Name;

        position.ClearOptions();
        List<string> newOptions = contactInfo.AllowedStates.ConvertAll(x => x.Name.GetName());
        position.AddOptions(newOptions);

        int currentStateIdx = _contactInfo.SemaphoreController.GetStateIndex();

        position.value = currentStateIdx;
        position.onValueChanged.AddListener(OnDropdownValueChange);

    }


    private void OnDropdownValueChange(int value)
    {
        ISemaphoreState changed = _contactInfo.AllowedStates[value].Name;
        _contactInfo.SemaphoreController.SetState(changed);
    }
}