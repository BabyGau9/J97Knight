using UnityEngine;

public class TransformSlime : StateMachineBehaviour
{
    // Khi bắt đầu state “transform”
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Slime: Bắt đầu transform!");
        // Bạn có thể viết code: tắt collider nhỏ, bật collider lớn, thay đổi scale, ...
        // Ví dụ:
        // var slime = animator.GetComponent<Slime>();
        // if (slime != null) slime.DoSomethingWhenTransformStarts();
    }

    // Khi state “transform” kết thúc (animation xong)
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Slime: Kết thúc transform -> chuyển Idle.");
        // Sau khi animation transform xong, ta gọi trigger “idle”:
        animator.SetTrigger("Idle");

        // Hoặc logic tùy ý (nếu cần):
        // var slime = animator.GetComponent<Slime>();
        // if (slime != null) slime.DoSomethingWhenTransformEnds();
    }
}
