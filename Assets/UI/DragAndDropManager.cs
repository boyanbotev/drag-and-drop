using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DragAndDropManager : MonoBehaviour
{
    private UIDocument uiDoc;
    VisualElement root;
    VisualElement menuItemsEl;

    private string menuItemsClassName = "menu-items";

    private void OnEnable()
    {
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        menuItemsEl = root.Q(className: menuItemsClassName);
    }
}
