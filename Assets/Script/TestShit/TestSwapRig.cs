using UnityEngine;

namespace Test
{
    public class TestSwapRig : MonoBehaviour
    {
        public SkinnedMeshRenderer targetRenderer; // your current character mesh
        public Animator animator;                  // Animator on the character

        [Header("New Rig Data")]
        public SkinnedMeshRenderer newRendererPrefab;
        public Avatar newAvatar;
        public RuntimeAnimatorController newController;

        private SkinnedMeshRenderer currentRenderer;

        void Start()
        {
            currentRenderer = targetRenderer;
        }

        public void SwapRig()
        {
            // Destroy old mesh
            if (currentRenderer != null)
            {
                Destroy(currentRenderer.gameObject);
            }

            // Instantiate new mesh (with its rig)
            SkinnedMeshRenderer newRenderer = Instantiate(newRendererPrefab, transform);

            currentRenderer = newRenderer;

            // Swap Avatar (rig definition)
            animator.avatar = newAvatar;

            // Optional: swap animation controller
            if (newController != null)
            {
                animator.runtimeAnimatorController = newController;
            }

            // Rebind animator to new skeleton
            animator.Rebind();
            animator.Update(0f);
        }
    }
}
