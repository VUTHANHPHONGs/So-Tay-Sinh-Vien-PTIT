using System.Collections;
using UnityEngine;

public class FastTravel : MonoBehaviour
{
    public GameObject fasttravelUI;    // UI element to display for fast travel options
    public Transform[] travelPos;      // Array of positions to travel to
    public GameObject player;          // Player character GameObject
    public GameObject player2;
    CharacterController characterController;
    private scr_PlayerController charControl;
    CharacterController characterController2;
    private scr_PlayerController charControl2;

    public Character3D_Manager_Ingame Character3D;

    private void Start()
    {
        fasttravelUI.SetActive(false);
        charControl = player.GetComponent<scr_PlayerController>();
        characterController = player.GetComponent<CharacterController>();

        charControl2 = player2.GetComponent<scr_PlayerController>();
        characterController2 = player2.GetComponent<CharacterController>();
    }

    // Method to activate the fast travel UI
    public void ShowFastTravelUI()
    {
       // Cursor.visible = true;
       // Cursor.lockState = CursorLockMode.None;
        fasttravelUI.SetActive(true);
    }

    // Method to hide the fast travel UI
    public void HideFastTravelUI()
    {
        fasttravelUI.SetActive(false);
    }

    // Method to move the player to the selected position
    public void TravelTo(int posIndex)
    {
        if (Character3D.index == 0)
        {
            charControl2.enabled = false; // V� hi?u h�a t?m th?i
            characterController2.enabled = false; // T?t CharacterController
            fasttravelUI.SetActive(true);

            // ??t v? tr� ch�nh x�c
            player2.transform.position = travelPos[posIndex].position;

            // Reset tr?ng th�i t??ng t�c
            Interact interactScript = player2.GetComponent<Interact>();
            if (interactScript != null && interactScript.currentInteractable != null)
            {
                interactScript.currentInteractable = null; // ??t l?i tr?ng th�i t??ng t�c
                if (interactScript.interactionUI != null)
                {
                    interactScript.interactionUI.SetActive(false); // T?t UI khi kh�ng c�n trong trigger
                }
            }

            characterController2.enabled = true; // K�ch ho?t l?i CharacterController
            charControl2.enabled = true; // K�ch ho?t l?i ?i?u khi?n nh�n v?t
            StartCoroutine(Loading());
        }
        else
        {
            charControl.enabled = false; // V� hi?u h�a t?m th?i
            characterController.enabled = false; // T?t CharacterController
            fasttravelUI.SetActive(true);

            // ??t v? tr� ch�nh x�c
            player.transform.position = travelPos[posIndex].position;

            // Reset tr?ng th�i t??ng t�c
            Interact interactScript = player.GetComponent<Interact>();
            if (interactScript != null && interactScript.currentInteractable != null)
            {
                interactScript.currentInteractable = null; // ??t l?i tr?ng th�i t??ng t�c
                if (interactScript.interactionUI != null)
                {
                    interactScript.interactionUI.SetActive(false); // T?t UI khi kh�ng c�n trong trigger
                }
            }

            characterController.enabled = true; // K�ch ho?t l?i CharacterController
            charControl.enabled = true; // K�ch ho?t l?i ?i?u khi?n nh�n v?t
            StartCoroutine(Loading());
        }
    }


    // Coroutine to re-enable player control after a delay
    IEnumerator Loading()
    {
        yield return new WaitForSeconds(3);
      //  charControl.enabled = true;
        fasttravelUI.SetActive(false);
    }
}
