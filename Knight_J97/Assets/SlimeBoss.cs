using UnityEngine;

public class SlimeBoss : MonoBehaviour
{
    public Animator animator;  // Kéo thả Animator của slime vào đây (hoặc GetComponent<Animator>() ở Start)

    // Hàm này được gọi khi slime bị player tấn công
    public void TakeDamage()
    {
        // Lấy StateMachineBehaviour "IdieSlime" từ Animator
        TransformSlime idleSlimeSMB = animator.GetBehaviour<TransformSlime>();
        if (idleSlimeSMB != null)
        {
            // Gọi OnHit() để slime phản ứng
        }
        else
        {
            Debug.LogWarning("Slime đang không ở state IdleSlime hoặc chưa gắn script IdieSlime.");
        }
    }
}
