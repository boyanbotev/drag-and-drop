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
    private VisualElement draggedElement;
    bool isDragging = false;
    private Vector2 originalPosition;

    private string draggableLettersClassName = "draggable-letters";
    private string draggableLetterClassName = "draggable-letter";
    private string writingLineClassName = "writing-line";
    private string writingLinesClassName = "writing-lines";

    private List<VisualElement> writingLines;

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
            var draggableLetter = new VisualElement();
            draggableLetter.AddToClassList(draggableLetterClassName);

            var letterLabel = new Label(letter);
            draggableLetter.Add(letterLabel);

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
            VisualElement lineContainerEl = new();
            lineContainerEl.AddToClassList(writingLineClassName);

            VisualElement lineEl = new();
            lineEl.AddToClassList("horizontal-line");

            lineContainerEl.Add(lineEl);
            writingLinesEl.Add(lineContainerEl);

            writingLines.Add(lineContainerEl);
        }
    }

    private void OnDragStart(PointerDownEvent evt, VisualElement draggableLetter)
    {
        draggedElement = draggableLetter;
        isDragging = true;
        draggableLetter.CaptureMouse();
        Vector2 pos = draggedElement.worldTransform.GetPosition();
        originalPosition = new Vector2(pos.x - draggableLetter.style.left.value.value, pos.y - draggableLetter.style.top.value.value);
    }

    private void OnDrag(PointerMoveEvent evt, VisualElement draggableLetter)
    {
        if (isDragging && draggedElement != null && draggedElement.HasMouseCapture())
        {
            Vector2 mousePosition = evt.position;

            draggedElement.style.left = mousePosition.x - originalPosition.x - draggedElement.resolvedStyle.width / 2;
            draggedElement.style.top = mousePosition.y - originalPosition.y - draggedElement.resolvedStyle.height / 2;
        }
    }

    private void OnDragEnd(PointerUpEvent evt, VisualElement draggableLetter)
    {
        if (isDragging && draggedElement != null)
        {
            bool isValid = false;
            VisualElement target = null;

            foreach (var line in writingLines)
            {
                if (IsOverlapping(draggableLetter, line))
                {
                    target = line;
                    isValid = true;
                    break;
                }
            }

            // TODO: Add code for whether to swap

            if (isValid)
            {
                SnapToTarget(draggableLetter, target);
            }
            else
            {
                draggableLetter.style.left = 0;
                draggableLetter.style.top = 0;
            }

            draggedElement.ReleaseMouse();
            isDragging = false;
            draggedElement = null;
        }
    }

    private bool IsOverlapping(VisualElement a, VisualElement b)
    {
        Rect rect1 = new(a.worldBound.position, a.worldBound.size);
        Rect rect2 = new(b.worldBound.position, b.worldBound.size);
        return rect1.Overlaps(rect2);
    }

    private void SnapToTarget(VisualElement dragged, VisualElement target)
    {
        Vector2 targetPos = target.worldTransform.GetPosition();
        Vector2 dir = new(targetPos.x - originalPosition.x, targetPos.y - originalPosition.y);

        dragged.style.left = dir.x + (target.resolvedStyle.width - dragged.resolvedStyle.width) / 2;
        dragged.style.top = dir.y + (target.resolvedStyle.height - dragged.resolvedStyle.height) / 2;
    }
}
