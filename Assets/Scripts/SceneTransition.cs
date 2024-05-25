using System.Collections;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    private static readonly int End = Animator.StringToHash("end");
    public Animator RestartLevelAnimator { get; private set; }
    public Animator NextLevelAnimator { get; private set; }

    private void Start()
    {
        RestartLevelAnimator = GameObject.Find("Panel").GetComponent<Animator>();
        NextLevelAnimator = GameObject.Find("Image").GetComponent<Animator>();
    }

    public static IEnumerator StartAnimation(Animator animator)
    {
        animator.SetTrigger(End);
        yield return new WaitForSeconds(1.5f);
    }
}