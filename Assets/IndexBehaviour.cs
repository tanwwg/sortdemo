using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class IndexBehaviour : MonoBehaviour
{
    public TextMeshProUGUI indexPrefab;
    public Transform indexLayout;

    public List<TextMeshProUGUI> indexFabs = new();

    public Dictionary<string, int> marks = new();

    public void Init(int n)
    {
        foreach (var f in indexFabs) Destroy(f.gameObject);
        
        indexFabs = Enumerable.Range(0, n).Select(i =>
        {
            var idx = Instantiate(indexPrefab, indexLayout);
            idx.gameObject.SetActive(true);
            idx.text = i.ToString();
            return idx;
        }).ToList();
    }

    private string[] getMarks(int idx)
    {
        return marks
            .Where(m => m.Value == idx)
            .Select(m => m.Key)
            .ToArray();
    }

    void Redraw()
    {
        for (var i = 0; i < indexFabs.Count; i++)
        {
            var lm = getMarks(i);
            if (lm.Length == 0)
            {
                indexFabs[i].text = i.ToString();
            }
            else
            {
                indexFabs[i].text = string.Join("\n", lm.Select(m => $"<color=white>{m}={i}</color>"));
            }
        }
    }
    
    public void Unmark(string mark)
    {
        marks.Remove(mark);
        Redraw();
    }

    public void Mark(string mark, int markIdx)
    {
        marks[mark] = markIdx;
        Redraw();
    }
}
