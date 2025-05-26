using System.Collections.Generic;
using UnityEngine;

public class InsertionSortBehaviour : MonoBehaviour
{
    [TextArea(5, 10)]
    public string code;

    public Setup setup;

    public void Init()
    {
        var list = new List<int> { 53, 17, 29, 5, 88, 22, 33 };
        var allBoxes = setup.Init(list, code);
        
        setup.Anim_Code(1, "Start of Insertion Sort");
        BubbleSort(allBoxes);
        setup.Anim_Code(6, "End of Insertion Sort");
        
        setup.Step();
    }
    
    void BubbleSort(List<Box> boxes)
    {
        setup.Anim_Code(2, "for i in 1..6");
        for (var i = 1; i < boxes.Count; i++)
        {
            setup.Anim_Index($"i={i}", 2, "i", boxes[i], i, resetIndex: "pos");
            
            var pos = i;
            setup.Anim_Index($"pos={i}", 3, "pos", boxes[i], i);
            
            while (pos > 0)
            {
                if (setup.Compare(boxes, 4, $"arr[{pos}] > arr[{pos-1}]?", pos, pos - 1)) break;
                
                setup.Swap(boxes, 5, "Swap", pos, pos-1);

                pos -= 1;
                setup.Anim_Index($"pos={pos}", 6, "pos", boxes[pos], pos);
            }
            
            
        }
    } 
    
}
