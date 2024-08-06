using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DragAndDropItem : MonoBehaviour
{
    public Texture2D texture;
    public string[] letters = { "p", "t", "i" };
    public string word = "pit";
}
