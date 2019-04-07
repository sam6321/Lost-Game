using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemainingUI : MonoBehaviour
{
    [SerializeField]
    private Vector2 start = new Vector2(0, 0);

    [SerializeField]
    private GameObject imagePrefab;

    [SerializeField]
    private GameObject showOnZero = null;

    private List<GameObject> activeImages = new List<GameObject>();

    public void SetCount(int count)
    {
        if(count != activeImages.Count)
        {
            if(count < activeImages.Count)
            {
                int range = activeImages.Count - count;
                int start = activeImages.Count - range;
                foreach (GameObject go in activeImages.GetRange(start, range)) {
                    Destroy(go);
                }
                activeImages.RemoveRange(start, range);
            }
            else
            {
                int toAdd = count - activeImages.Count;
                for(int i = 0; i < toAdd; i++)
                {
                    GameObject go = Instantiate(imagePrefab, transform);
                    
                    RectTransform rectTransform = go.transform as RectTransform;
                    if(activeImages.Count > 0)
                    {
                        RectTransform last = activeImages[activeImages.Count - 1].transform as RectTransform;
                        float width = last.sizeDelta.x;
                        rectTransform.anchoredPosition = new Vector2(last.anchoredPosition.x + width, last.anchoredPosition.y);
                    }
                    else
                    {
                        rectTransform.anchoredPosition = start;
                    }

                    activeImages.Add(go);
                }
            }
        }

        showOnZero?.SetActive(count == 0);
    }
}
