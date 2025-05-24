using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CodeBehaviour : MonoBehaviour
{
    public TextMeshProUGUI text;

    public string markColor = "#D3D3D3";
    
    public List<string> origLines;

    public void Init(string srcCode)
    {
        this.text.text = srcCode;
        this.origLines = text.text.Split("\n").ToList();
    }

    public void Mark(int lineNum)
    {
        var newText = Enumerable.Range(0, origLines.Count)
            .Select(i => i == lineNum ? $"<mark=${markColor}>{origLines[i]}</mark>" : origLines[i])
            .ToArray();
        text.text = string.Join("\n", newText);
    }
}
