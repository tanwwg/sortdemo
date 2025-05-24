using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class IndexBehaviour : MonoBehaviour
{
    public TextMeshProUGUI indexPrefab;
    public Transform indexLayout;

    public List<TextMeshProUGUI> indexFabs = new();

    public void Init(int n)
    {
        indexFabs = Enumerable.Range(0, n).Select(i =>
        {
            var idx = Instantiate(indexPrefab, indexLayout);
            idx.gameObject.SetActive(true);
            idx.text = i.ToString();
            return idx;
        }).ToList();
    }

    public void Mark(int markIdx)
    {
        for (var i = 0; i < indexFabs.Count; i++)
        {
            indexFabs[i].text = i == markIdx ? $"<color=white>i = {i}" : i.ToString();
        }
    }
}
