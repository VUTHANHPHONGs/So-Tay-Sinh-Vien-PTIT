using Core;
using TMPro;
using UnityEngine;

namespace PlayerController
{
    public class PlayerManager_1 : MonoBehaviour
    {
        public UIAnimationController interactionPopup; // UI hi?n th? t??ng t�c
        TMP_Text interactionText; // V?n b?n hi?n th? n?i dung t??ng t�c
        public LayerMask talkableMask; // L?p d�ng ?? raycast ki?m tra ??i t??ng c� th? n�i chuy?n
       
        public bool talk_input;
        PlayerInputActions playerInputActions;
        private void Awake()
        {
            // L?y tham chi?u c�c th�nh ph?n c?n thi?t
         
            playerInputActions = new PlayerInputActions();



            playerInputActions.Actions.Talk.performed += x => talk_input = true;
            interactionText = interactionPopup.gameObject.GetComponentInChildren<TMP_Text>();
        }

        private void Update()
        {
            CheckForInteractableObject(); // Ki?m tra ??i t??ng c� th? t??ng t�c m?i khung h�nh
        }

        #region Player Interaction

        /// <summary>
        /// Ki?m tra xem ng??i ch?i c� th? t??ng t�c v?i m?t ??i t??ng g?n ?� kh�ng.
        /// </summary>
        public void CheckForInteractableObject()
        {
            RaycastHit hit;

            // S? d?ng SphereCast ?? ki?m tra c�c ??i t??ng trong ph?m vi t??ng t�c
            if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, 1f, talkableMask))
            {
                if (hit.collider.CompareTag("Talkable"))
                {
                    // L?y th�nh ph?n "Interactable" t? ??i t??ng
                    Interactable interactableObject = hit.collider.GetComponent<TalkInteraction>();

                    if (interactableObject != null)
                    {
                        // Hi?n th? v?n b?n t??ng t�c
                        interactionText.text = interactableObject.interactableText;
                        interactionPopup.Activate();

                        // Th?c hi?n h�nh ??ng t??ng t�c n?u nh?n ??u v�o t? ng??i ch?i
                        if (talk_input)
                        {
                            interactableObject.Interact();
                        }
                    }
                }
            }
            else
            {
                // ?n popup n?u kh�ng c� ??i t??ng t??ng t�c n�o
                if (interactionPopup != null)
                {
                    interactionPopup.Deactivate();
                }
            }
        }

        #endregion
    }
}
