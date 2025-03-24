using UnityEngine;

public class IdieSlime : StateMachineBehaviour
{
    private Transform slimeTransform;
    private Animator slimeAnimator;

    // Khi vào state Idle
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        slimeTransform = animator.transform;
        slimeAnimator = animator;
    }

    // Hàm bị đánh
    public void OnHit()
    {
        if (slimeTransform == null || slimeAnimator == null) return;

        // Gọi trigger animation Hurt
        slimeAnimator.SetTrigger("Hurt");

        // Hoặc knockback slime
        // slimeTransform.position += new Vector3(-1f, 0f, 0f);

        Debug.Log("Slime bị đánh trong state Idle, vị trí: " + slimeTransform.position);
    }
}
