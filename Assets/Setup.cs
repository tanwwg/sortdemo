using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AnimItem
{
    public Action Execute;
    
    public Action PostExecute;
    public Action Reverse;
}

public class Setup : MonoBehaviour
{
    public Box prefab;
    public Transform layout;
    
    public List<Box> allBoxes;

    public List<AnimItem> anims = new();

    public TextMeshProUGUI statusText;

    public IndexBehaviour index;
    public CodeBehaviour code;

    public UnityEvent onStartSwap;
    public UnityEvent onEndSwap;    

    void AnimateCompare(Box b1, Box b2)
    {
        HideArrows();
        b1.arrow.SetActive(true);
        b2.arrow.SetActive(true);
    }

    void SetStatusText(string s)
    {
        statusText.text = $"Step {animIndex + 1}: {s}";
    }

    void HideArrows()
    {
        foreach(var b in allBoxes) b.arrow.SetActive(false);
    }

    void AnimateSwap(Box b1, Box b2)
    {
        onStartSwap.Invoke();
        HideArrows();
        b1.arrow.SetActive(true);
        b2.arrow.SetActive(true);
        
        StartCoroutine(AnimateMoveAsync(b1.transform, b1.transform, b2.transform, 0.5f));
        StartCoroutine(AnimateMoveAsync(b2.transform, b2.transform, b1.transform, 0.5f));
        Invoke(nameof(OnEndSwap), 0.5f);
    }

    void OnEndSwap()
    {
        onEndSwap.Invoke();
    }

    IEnumerator AnimateMoveAsync(Transform target, Transform start, Transform end, float time)
    {
        float elapsed = 0f;
        Vector3 startPos = start.position;
        Vector3 endPos = end.position;

        while (elapsed < time)
        {
            float t = elapsed / time;
            target.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.position = endPos; // Snap to exact end at finish
    }

    void Anim_Code(int codeLine, string status)
    {
        anims.Add(new AnimItem()
        {
            Execute = () =>
            {
                HideArrows();
                code.Mark(codeLine-1);
                SetStatusText(status);
            }
        });
    }

    bool Compare(List<Box> boxes, int codeLine, string status, int i, int j)
    {
        var b1 = boxes[i];
        var b2 = boxes[j];
        anims.Add(new AnimItem()
        {
            Execute = () =>
            {
                index.Mark(i);
                code.Mark(codeLine-1);
                SetStatusText(status);
                AnimateCompare(b1, b2);
            }
        });
        return boxes[i].value > boxes[j].value;
    }
    
    void Swap(List<Box> boxes, int codeLine, string status, int i, int j)
    {
        var b1 = boxes[i];
        var b2 = boxes[j];
        (boxes[i], boxes[j]) = (boxes[j], boxes[i]);
        anims.Add(new AnimItem()
        {
            Execute = () =>
            {
                code.Mark(codeLine-1);
                SetStatusText(status);
                AnimateSwap(b1, b2);
            },
            Reverse = () =>
            {
                AnimateSwap(b1, b2);
            },
            PostExecute = () =>
            {
                code.Mark(codeLine-1);                
                SetStatusText(status);
            }
        });
    }

    void BubbleSort(List<Box> boxes)
    {
        Anim_Code(2, "for i in 0..5");
        for (var i = 0; i < boxes.Count - 1; i++)
        {
            if (Compare(boxes, 3, $"arr[{i}] > arr[{i+1}] ?", i, i + 1))
            {
                Swap(boxes, 4, $"Swap arr[{i}] and arr[{i+1}]", i, i + 1);

                Anim_Code(5, "Restart Loop");
                BubbleSort(boxes);
                
                return;
            }
        }
    }
    
    void Start()
    {
        var list = new List<int> { 53, 17, 29, 5, 88, 22, 33 };
        
        index.Init(list.Count);
        
        Anim_Code(1, "Start of Bubble Sort");
        
        
        this.allBoxes = list.Select(i =>
        {
            var fab = Instantiate(prefab, parent: layout);
            fab.gameObject.SetActive(true);
            fab.text.text = i.ToString();
            fab.value = i;
            return fab;
        }).ToList();
        BubbleSort(allBoxes);
        
        Anim_Code(6, "End of Bubble Sort");

        Step();
    }

    public int animIndex = 0;

    public void Step()
    {
        if (animIndex >= anims.Count) return;
        anims[animIndex].Execute();
        animIndex += 1;
    }

    public void StepBack()
    {
        if (animIndex <= 1) return;

        var startIndex = animIndex;

        animIndex = startIndex - 1;
        anims[animIndex].Reverse?.Invoke();
        
        animIndex = startIndex - 2;        
        var post = anims[animIndex].PostExecute ?? anims[animIndex].Execute;
        post.Invoke();

        animIndex = startIndex - 1;

    }

}
