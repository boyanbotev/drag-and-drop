using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class DraggableLetter : VisualElement
{
    public WritingLine line;
    public Vector2 originalPos;
    public string value;
    private string draggableLetterClassName = "draggable-letter";

    public void Setup(string letter)
    {
        value = letter;
        AddToClassList(draggableLetterClassName);

        var letterLabel = new Label(letter);
        Add(letterLabel);
    }

    public void CalculatePos()
    {
        Vector2 pos = worldTransform.GetPosition();
        originalPos = new Vector2(pos.x - style.left.value.value, pos.y - style.top.value.value);
    }
}
