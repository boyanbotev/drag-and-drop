using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ClickableImage : Image
{
    public static event Action<string> onClick;
    private readonly string className = "image";
    private string imageName;
    public ClickableImage(string name)
    {
        imageName = name;
        AddToClassList(className);
        RegisterCallback<PointerDownEvent>(evt => OnPointerDown());
    }

    void OnPointerDown()
    {
        onClick?.Invoke(imageName);
    }
}
