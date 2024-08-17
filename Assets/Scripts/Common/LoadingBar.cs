using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] List<Image> segments = new List<Image>();
    [SerializeField] Transform bar;
    List<int> tweens = new List<int>();

    public void UpdateBar(float progress)
    {
        bar.localScale = Vector2.right * progress + Vector2.up;
    }

    public void UpdateSegments(float progress)
    {
        if(progress == 0)
        {
            ResetAllTweens();
            ResetLoadingBar();
            return;
        }

        int segmentsToFill = Mathf.FloorToInt(segments.Count * progress);
        float fillLastSegmentTo = (segments.Count * progress) % 1;
        Color segmentColor = segments[0].color;

        for (int i = 0; i < segmentsToFill; i++)
        {
            segmentColor = segments[i].color;
            segmentColor = new Color(segmentColor.r, segmentColor.g, segmentColor.b, 1);
            segments[i].color = segmentColor;
        }

        if (segmentsToFill >= segments.Count)
        {
            //Debug.LogError("Tried to fill non existent segment!");
            return;
        }
        segments[segmentsToFill].color = new Color(segmentColor.r, segmentColor.g, segmentColor.b, fillLastSegmentTo);
    }

    public void CycleSegments()
    {
        ResetAllTweens();
        ResetLoadingBar();

        int ii = 0;
        foreach (Image image in segments)
        {
            tweens.Add(LeanTween.value(0, 1, 1)
                .setOnUpdate((float alpha) =>
                {
                    Image img = image;
                    Color fadeColor = img.color;
                    fadeColor.a = alpha;
                    img.color = fadeColor;
                })
                .setDelay(ii * 0.1f)
                .setRepeat(-1)
                .setLoopPingPong().id);
            ii++;
        }
    }

    public void ResetLoadingBar()
    {
       foreach(Image segment in segments)
       {
            segment.color = new Color(segment.color.r, segment.color.g, segment.color.b, 0);
       }
    }

    public void ResetAllTweens()
    {
        foreach (var tween in tweens)
        {
            LeanTween.cancel(tween);
        }
    }
}