using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WritingLine : VisualElement
{
    public DraggableLetter letter;

    private string writingLineClassName = "writing-line";
    private string lineClassName = "horizontal-line";

    public WritingLine()
    {
        AddToClassList(writingLineClassName);

        VisualElement lineEl = new();
        lineEl.AddToClassList(lineClassName);
        Add(lineEl);
    }

    public void AddLetter(DraggableLetter letter)
    {
        UpdateLetterPos(letter);

        this.letter = letter;
        letter.line = this;
    }

    private void UpdateLetterPos(DraggableLetter letter)
    {
        Vector2 targetPos = worldTransform.GetPosition();
        Vector2 dir = new(targetPos.x - letter.originalPos.x, targetPos.y - letter.originalPos.y);

        letter.style.left = dir.x + (resolvedStyle.width - letter.resolvedStyle.width) / 2;
        letter.style.top = dir.y + (resolvedStyle.height - letter.resolvedStyle.height) / 2;
    }
}
