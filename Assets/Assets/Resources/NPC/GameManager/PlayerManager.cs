using Core;
using TMPro;
using UnityEngine;

namespace PlayerController
{
    public class PlayerManager_1 : MonoBehaviour
    {
        public UIAnimationController interactionPopup; // UI hi?n th? t??ng tác
        TMP_Text interactionText; // V?n b?n hi?n th? n?i dung t??ng tác
        public LayerMask talkableMask; // L?p dùng ?? raycast ki?m tra ??i t??ng có th? nói chuy?n
       
        public bool talk_input;
        PlayerInputActions playerInputActions;
        private void Awake()
        {
            // L?y tham chi?u các thành ph?n c?n thi?t
         
            playerInputActions = new PlayerInputActions();



            playerInputActions.Actions.Talk.performed += x => talk_input = true;
            interactionText = interactionPopup.gameObject.GetComponentInChildren<TMP_Text>();
        }

        private void Update()
        {
            CheckForInteractableObject(); // Ki?m tra ??i t??ng có th? t??ng tác m?i khung hình
        }

        #region Player Interaction

        /// <summary>
        /// Ki?m tra xem ng??i ch?i có th? t??ng tác v?i m?t ??i t??ng g?n ?ó không.
        /// </summary>
        public void CheckForInteractableObject()
        {
            RaycastHit hit;

            // S? d?ng SphereCast ?? ki?m tra các ??i t??ng trong ph?m vi t??ng tác
            if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, 1f, talkableMask))
            {
                if (hit.collider.CompareTag("Talkable"))
                {
                    // L?y thành ph?n "Interactable" t? ??i t??ng
                    Interactable interactableObject = hit.collider.GetComponent<TalkInteraction>();

                    if (interactableObject != null)
                    {
                        // Hi?n th? v?n b?n t??ng tác
                        interactionText.text = interactableObject.interactableText;
                        interactionPopup.Activate();

                        // Th?c hi?n hành ??ng t??ng tác n?u nh?n ??u vào t? ng??i ch?i
                        if (talk_input)
                        {
                            interactableObject.Interact();
                        }
                    }
                }
            }
            else
            {
                // ?n popup n?u không có ??i t??ng t??ng tác nào
                if (interactionPopup != null)
                {
                    interactionPopup.Deactivate();
                }
            }
        }

        #endregion
    }
}
