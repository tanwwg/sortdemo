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

    public UnityEvent onStartSwap;
    public UnityEvent onEndSwap;    

    void AnimateCompare(Box b1, Box b2)
    {
        foreach(var b in allBoxes) b.arrow.SetActive(false);
        b1.arrow.SetActive(true);
        b2.arrow.SetActive(true);
    }

    void SetStatusText(string s)
    {
        statusText.text = $"Step {animIndex + 1}: {s}";
    }

    void AnimateSwap(Box b1, Box b2)
    {
        onStartSwap.Invoke();
        foreach(var b in allBoxes) b.arrow.SetActive(false);
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

    bool Compare(List<Box> boxes, int i, int j)
    {
        var b1 = boxes[i];
        var b2 = boxes[j];
        anims.Add(new AnimItem()
        {
            Execute = () =>
            {
                SetStatusText($"Compare [{i}] and [{j}]");
                AnimateCompare(b1, b2);
            }
        });
        return boxes[i].value > boxes[j].value;
    }
    
    void Swap(List<Box> boxes, int i, int j)
    {
        var b1 = boxes[i];
        var b2 = boxes[j];
        (boxes[i], boxes[j]) = (boxes[j], boxes[i]);
        anims.Add(new AnimItem()
        {
            Execute = () =>
            {
                SetStatusText($"Swap [{i}] and [{j}]");
                AnimateSwap(b1, b2);
            },
            Reverse = () =>
            {
                AnimateSwap(b1, b2);
            },
            PostExecute = () =>
            {
                SetStatusText($"Swap [{i}] and [{j}]");
            }
        });
    }

    void BubbleSort(List<Box> boxes)
    {
        for (var i = 0; i < boxes.Count - 1; i++)
        {
            if (Compare(boxes, i, i + 1))
            {
                Swap(boxes, i, i + 1);
                BubbleSort(boxes);
                return;
            }
        }
    }
    
    void Start()
    {
        var list = new List<int> { 53, 17, 29, 5, 88, 22, 33 };
        
        anims.Add(new AnimItem()
        {
            Execute = () =>
            {
                SetStatusText("Start of Bubble Sort");
            }
        });
        
        this.allBoxes = list.Select(i =>
        {
            var fab = Instantiate(prefab, parent: layout);
            fab.gameObject.SetActive(true);
            fab.text.text = i.ToString();
            fab.value = i;
            return fab;
        }).ToList();
        BubbleSort(allBoxes);
        anims.Add(new AnimItem()
        {
            Execute = () =>
            {
                SetStatusText("End");
            }
        });

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
