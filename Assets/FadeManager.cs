using System;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private Action onFadeCompleteAction;

    public void StartFade(Action onFadeComplete=null)
    {
        onFadeCompleteAction = onFadeComplete;
        animator.SetBool("Fade", true);
    }

    public void OnFadeComplete()
    {
        animator.SetBool("Fade", false);
        onFadeCompleteAction?.Invoke();
    }
}
