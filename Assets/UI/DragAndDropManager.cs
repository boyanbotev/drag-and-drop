using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DragAndDropManager : MonoBehaviour
{
    [SerializeField] string[] letters = { "p", "t", "i" };
    [SerializeField] string word = "pit";
    [SerializeField] int writingLineCount = 3;

    private UIDocument uiDoc;
    VisualElement root;
    VisualElement draggableLettersEl;
    VisualElement body;
    private DraggableLetter draggedElement;
    bool isDragging = false;

    private string draggableLettersClassName = "draggable-letters";
    private string writingLinesClassName = "writing-lines";

    private List<WritingLine> writingLines;

    private void OnEnable()
    {
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        draggableLettersEl = root.Q(className: draggableLettersClassName);
        body = root.Q(className: "body");

        CreateDraggableLetters();
        CreateWritingLines();
    }

    void CreateDraggableLetters() {

        foreach (string letter in letters)
        {
            var draggableLetter = new DraggableLetter();
            draggableLetter.Setup(letter);

            draggableLetter.RegisterCallback<PointerDownEvent>(evt => OnDragStart(evt, draggableLetter));
            draggableLetter.RegisterCallback<PointerMoveEvent>(evt => OnDrag(evt, draggableLetter));
            draggableLetter.RegisterCallback<PointerUpEvent>(evt => OnDragEnd(evt, draggableLetter));
            draggableLettersEl.Add(draggableLetter);
        }
    }

    void CreateWritingLines()
    {
        var writingLinesEl = root.Q(className: writingLinesClassName);
        writingLines = new();

        for (int i = 0; i < writingLineCount; i++)
        {
            var writingLine = new WritingLine();
            writingLine.SetUp();

            writingLinesEl.Add(writingLine);
            writingLines.Add(writingLine);
        }
    }

    private void OnDragStart(PointerDownEvent evt, DraggableLetter letter)
    {
        draggedElement = letter;
        letter.CalculatePos();
        isDragging = true;
        letter.CaptureMouse();
    }

    private void OnDrag(PointerMoveEvent evt, DraggableLetter letter)
    {
        if (isDragging && draggedElement != null && draggedElement.HasMouseCapture())
        {
            SetDraggedPos(evt.position);
        }
    }

    private void OnDragEnd(PointerUpEvent evt, DraggableLetter draggedLetter)
    {
        if (isDragging && draggedElement != null)
        {
            WritingLine target = GetTarget(draggedElement);

            if (draggedLetter.line != null)
            {
                // set old writing line's letter reference to null
                draggedLetter.line.letter = null;
            }

            if (target != null)
            {
                // If there's a letter on the new position
                if (target.letter != null && target.letter != draggedLetter)
                {
                    if (draggedLetter.line != null)
                    {
                        // Swap
                        draggedLetter.line.AddLetter(target.letter);
                    } 
                    else
                    {
                        // Move old letter back to original position
                        ResetLetter(target.letter);
                    }
                }

                // move to new pos
                target.AddLetter(draggedLetter);
            }
            else
            {
                ResetLetter(draggedLetter);
            }

            EvaluateWord();

            draggedElement.ReleaseMouse();
            isDragging = false;
            draggedElement = null;
        }
    }

    void SetDraggedPos(Vector2 pos)
    {
        float elementWidth = draggedElement.resolvedStyle.width;
        float elementHeight = draggedElement.resolvedStyle.height;

        var clampedPos = new Vector2(
            Math.Clamp(pos.x, body.worldBound.x + elementWidth / 2, body.worldBound.x + body.worldBound.width - elementWidth / 2),
            Math.Clamp(pos.y, body.worldBound.y + elementHeight / 2, body.worldBound.y + body.worldBound.height - elementHeight / 2)
        );

        var adjustedPos = new Vector2(
            clampedPos.x - draggedElement.originalPos.x - elementWidth / 2,
            clampedPos.y - draggedElement.originalPos.y - elementHeight / 2
        );

        draggedElement.style.left = adjustedPos.x;
        draggedElement.style.top = adjustedPos.y;
    }

    void EvaluateWord()
    {
        string joinedWord = "";

        foreach (var writingLine in writingLines)
        {
            if (writingLine.letter != null)
            {
                joinedWord += writingLine.letter.value;
            }
        }

        if (joinedWord == word)
        {
            Debug.Log("VICTORY");
        }
    }

    private WritingLine GetTarget(DraggableLetter draggedLetter)
    {
        WritingLine target = null;

        foreach (var line in writingLines)
        {
            if (IsOverlapping(draggedLetter, line))
            {
                target = line;
                break;
            }
        }

        return target;
    }

    private void ResetLetter(DraggableLetter draggedLetter)
    {
        draggedLetter.style.left = 0;
        draggedLetter.style.top = 0;
 
        draggedLetter.line = null;
    }

    private bool IsOverlapping(VisualElement a, VisualElement b)
    {
        Rect rect1 = new(a.worldBound.position, a.worldBound.size);
        Rect rect2 = new(b.worldBound.position, b.worldBound.size);
        return rect1.Overlaps(rect2);
    }
}
