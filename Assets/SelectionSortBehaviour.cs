using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionSortBehaviour : MonoBehaviour
{
    [TextArea(5, 10)]
    public string code;

    public Setup setup;

    public void Init()
    {
        var list = new List<int> { 53, 17, 29, 5, 88, 22, 33 };
        var allBoxes = setup.Init(list, code);
        
        setup.Anim_Code(1, "Start of Selection Sort");
        SelectionSort_Simple(allBoxes);
        setup.Anim_Code(6, "End of Selection Sort");
        
        setup.Step();
    }

    int findMaxIndex(List<Box> boxes, int startIndex)
    {
        var max = boxes.Skip(startIndex).Max(b => b.value);
        var find = boxes.FindIndex(b => b.value == max);
        
        setup.Anim_Index($"find_max_index(arr, {startIndex}) = {find}", 3, "j", boxes[find], find);
        
        return find;
    }
    
    void SelectionSort_Simple(List<Box> boxes)
    {
        setup.Anim_Code(2, "for i in 0..5");
        for (var i = 0; i < boxes.Count - 1; i++)
        {
            setup.Anim_Index("i=" + i, 2, "i", boxes[i], i, resetIndex: "j");
            
            var idx = findMaxIndex(boxes, i);
            setup.Swap(boxes, 4, $"Swap arr[{i}] and arr[{idx}]", i, idx);
        }
    }
}
