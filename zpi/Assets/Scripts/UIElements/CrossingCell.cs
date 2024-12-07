using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyAndCode.UI;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CrossingCell : MonoBehaviour, ICell
{
    public TMP_Text nameLabel;
    public TMP_Dropdown position;

    //Model
    private CrossingInfo _contactInfo;
    private int _cellIndex;

    private void Start()
    {
        var dropdownRectTransform = position.GetComponent<RectTransform>();
        dropdownRectTransform.localScale = new Vector3(2f, 2f, 1f);
        dropdownRectTransform.localPosition = dropdownRectTransform.localPosition - new Vector3(100f, 0f, 0f);
        dropdownRectTransform.sizeDelta = dropdownRectTransform.sizeDelta + new Vector2(100f, 0f);

        nameLabel.fontSize = 120f;
        nameLabel.color = Color.black;
        nameLabel.alignment = TextAlignmentOptions.Left;
    }
    //This is called from the SetCell method in DataSource
    public void ConfigureCell(CrossingInfo contactInfo, int cellIndex)
    {
        _cellIndex = cellIndex;
        _contactInfo = contactInfo;



        nameLabel.text = contactInfo.Name;

        position.ClearOptions();
        List<string> newOptions = contactInfo.AllowedStates.ConvertAll(x => x.Name);
        position.AddOptions(newOptions);

        int currentStateIdx = _contactInfo.CrossingSystem.GetStateIndex();

        position.value = currentStateIdx;

        position.onValueChanged.RemoveAllListeners();
        position.onValueChanged.AddListener(OnDropdownValueChange);
    }


    private void OnDropdownValueChange(int value)
    {
        int changed = _contactInfo.AllowedStates[value].Index;
        _contactInfo.CrossingSystem.ChangeCrossingState(changed);
    }
}