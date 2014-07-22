using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentsTable : UIWidgetContainer
{
    public BinaryLabelBar prefab;
    public UITable table;

    public int columns = 2;
    public int rows = 2;
    public bool fill = true;
    public bool MatchWidth = true;
    public bool MatchHeight = true;

    private UIWidget _widget;
    private List<BinaryLabelBar> _items = new List<BinaryLabelBar>();
    void Start()
    {
        _widget = GetComponent<UIWidget>();
    }

    [ContextMenu("Execute")]
    public void Execute()
    {
        ClearItems();
        table.columns = columns;
        if (fill)
            Fill();
        table.Reposition();
    }

    void ClearItems()
    {
        for (int i = 0; i < table.children.Count; i++)
        {
            if (table.children[i] != null)
                NGUITools.Destroy(table.children[i].gameObject);
        }
    }

    void Fill()
    {
        for (int i = 0; i < columns * rows; i++)
        {
            AddItem("Name", "Value");
        }
    }

    void AddItem(string textOne, string textTwo)
    {
        if (_widget == null)
            _widget = GetComponent<UIWidget>();
        BinaryLabelBar binLabel = NGUITools.AddChild(table.gameObject, prefab.gameObject).GetComponent<BinaryLabelBar>();
        // Set Text
        binLabel.FirstLabel.text = textOne;
        binLabel.SecondLabel.text = textTwo;
        // Adjust Width & Height
        UIWidget binLabelWidget = binLabel.GetComponent<UIWidget>();
        if (MatchWidth)
            binLabelWidget.width = _widget.width / columns;
        if (MatchHeight)
            binLabelWidget.height = _widget.height / rows;

        _items.Add(binLabel);
    }
}
