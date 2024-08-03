using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DragAndDropManager : MonoBehaviour
{
    [SerializeField] string[] letters = { "p", "t", "i" };
    [SerializeField] int writingLineCount = 3;

    private UIDocument uiDoc;
    VisualElement root;
    VisualElement draggableLettersEl;
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
            Vector2 mousePosition = evt.position;

            draggedElement.style.left = mousePosition.x - draggedElement.originalPos.x - draggedElement.resolvedStyle.width / 2;
            draggedElement.style.top = mousePosition.y - draggedElement.originalPos.y - draggedElement.resolvedStyle.height / 2;
        }
    }

    private void OnDragEnd(PointerUpEvent evt, DraggableLetter draggedLetter)
    {
        if (isDragging && draggedElement != null)
        {
            bool isValid = false;
            WritingLine target = null;

            foreach (var line in writingLines)
            {
                if (IsOverlapping(draggedLetter, line))
                {
                    target = line;
                    isValid = true;
                    break;
                }
            }

            if (isValid)
            {
                if (target.letter != null && target.letter != draggedLetter)
                {
                    if (draggedLetter.line != null)
                    {
                        draggedLetter.line.AddLetter(target.letter);
                    } 
                    else
                    {
                        ResetLetter(target.letter);
                    }
                } 
                else if (draggedLetter.line != null)
                {
                    draggedLetter.line.letter = null;
                    draggedLetter.line = null;
                }

                target.AddLetter(draggedLetter);
            }
            else
            {
                if (draggedLetter.line != null)
                {
                    draggedLetter.line.letter = null;
                }

                ResetLetter(draggedLetter);
            }

            draggedElement.ReleaseMouse();
            isDragging = false;
            draggedElement = null;
        }
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
