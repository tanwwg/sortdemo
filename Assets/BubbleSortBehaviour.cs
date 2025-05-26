using System.Collections.Generic;
using UnityEngine;

public class BubbleSortBehaviour : MonoBehaviour
{
    [TextArea(5, 10)]
    public string code;

    public Setup setup;

    public void Init()
    {
        var list = new List<int> { 53, 17, 29, 5, 88, 22, 33 };
        var allBoxes = setup.Init(list, code);
        
        setup.Anim_Code(1, "Start of Bubble Sort");
        BubbleSort(allBoxes);
        setup.Anim_Code(6, "End of Bubble Sort");
        
        setup.Step();
    }
    
    void BubbleSort(List<Box> boxes)
    {
        setup.Anim_Code(2, "for i in 0..5");
        for (var i = 0; i < boxes.Count - 1; i++)
        {
            setup.Anim_Index($"i={i}", 2, "i", boxes[i], i);
            
            if (setup.Compare(boxes, 3, $"arr[{i}] > arr[{i+1}] ?", i, i + 1))
            {
                setup.Swap(boxes, 4, $"Swap arr[{i}] and arr[{i+1}]", i, i + 1);

                setup.Anim_Code(5, "Restart Loop");
                BubbleSort(boxes);
                
                return;
            }
        }
    }

}
