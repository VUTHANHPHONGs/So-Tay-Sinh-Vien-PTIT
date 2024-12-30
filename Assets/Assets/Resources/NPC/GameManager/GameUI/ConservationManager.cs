using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interaction
{
    public class ConservationManager : MonoBehaviour
    {
        [Header("NPC Message")]
        public UIAnimationController messageContainer;
        public TMP_Text npcMessage;

        [Header("Player Response")]
        public UIAnimationController responseController;
        public GameObject responsePrefab;
        public Sprite defautIcon;

        GameObject targetNPC;
        NPCConservationSO conservationData;

        public GameObject UI;
        public GameObject targetGameObject;
        public GameObject targetGameObject2;
        public Character3D_Manager_Ingame character;
        public scr_CameraController CameraToggle;
        public scr_CameraController CameraToggle2;
        public scr_PlayerController MovementToggle;
        public scr_PlayerController MovementToggle2;

        public ButtonActivator ButtonActivator;
        public MouseManager MouseManager;

        public void InitConservation(GameObject npc, NPCConservationSO conservation)
        {
            Debug.Log(conservation);
            targetNPC = npc;
            conservationData = conservation;
            
            //setup first conservation
            DialogConservation firstDialog;
            if(conservationData.startFromFirstDialog)
            {
                firstDialog = conservation.dialogs[0];
            } else
            {
                //can get a random dialog
                firstDialog = conservation.dialogs[0];
            }
            SetupDialogInConservation(firstDialog);
        }

        public void ChangeTargetNPC(GameObject npc) { targetNPC = npc; }

        void SetupDialogInConservation(DialogConservation dialog)
        {
            //Setup npc message
            npcMessage.text = dialog.message;
   
            //Setup player response
            //Clear old responses in container
            foreach (Transform child in responseController.transform)
            {
                child.gameObject.SetActive(false);
            }

            //Add new response
            for(int i = 0; i < dialog.possibleResponses.Count; i++)
            {
                DialogResponse responseData = dialog.possibleResponses[i];

                //find or create response object
                GameObject responseObject;
                if (i < responseController.transform.childCount)
                {
                    responseObject = responseController.transform.GetChild(i).gameObject;
                } else
                {
                    responseObject = Instantiate(responsePrefab, responseController.transform);
                }

                //setup response data
                responseObject.GetComponentInChildren<TMP_Text>().text = responseData.message;
                responseObject.transform.GetChild(1).GetComponent<Image>().sprite = responseData.icon != null? responseData.icon : defautIcon;
                responseObject.GetComponent<Button>().onClick.AddListener(delegate () 
                {
                    if(responseData.nextDialogId != "") 
                    {
                        DialogConservation nextDialog = conservationData.GetDialog(responseData.nextDialogId);
                        if(nextDialog != null) { StartCoroutine(UpdateConservation(nextDialog)); }
                    } 
                    if (responseData.executedFunction != DialogExecuteFunction.None) { targetNPC.SendMessage(responseData.executedFunction.ToString()); }
                });

                responseObject.SetActive(true);
            }
            responseController.UpdateObjectChange();
        }

        void ClearAllButtonEvent()
        {
            Button[] btns = responseController.GetComponentsInChildren<Button>();
            foreach(Button b in btns)
            {
                b.onClick.RemoveAllListeners();
            }
            //
            DisableAllButton();
        }

        void EnableAllButton()
        {
            Button[] btns = responseController.GetComponentsInChildren<Button>();
            foreach (Button b in btns)
            {
                b.interactable = true;
            }
        }

        void DisableAllButton()
        {
            Button[] btns = responseController.GetComponentsInChildren<Button>();
            foreach (Button b in btns)
            {
                b.interactable = false;
            }
        }
        public IEnumerator UpdateConservation(DialogConservation nextDialog)
        {
            ClearAllButtonEvent();
            responseController.Deactivate();
            
            yield return new WaitForSeconds(0.4f);
            messageContainer.Deactivate();
            
            yield return new WaitForSeconds(0.6f);
            SetupDialogInConservation(nextDialog);
            messageContainer.Activate();

            yield return new WaitForSeconds(0.2f);
            responseController.Activate();
            EnableAllButton();
            yield return null;
        }

        public IEnumerator ActivateConservationDialog()
        {
            messageContainer.Activate();
            yield return new WaitForSeconds(0.3f);
            responseController.Activate();
            EnableAllButton();
        }

        public IEnumerator DeactivateConservationDialog()
        {
            ClearAllButtonEvent();
            responseController.Deactivate();
            yield return new WaitForSeconds(0.3f);
            messageContainer.Deactivate();




            scr_PlayerController script = targetGameObject.GetComponent<scr_PlayerController>();
            scr_PlayerController script2 = targetGameObject2.GetComponent<scr_PlayerController>();
            if (script != null)
            {
                if (character.index == 0)
                {
                    MovementToggle.isCheck = true;
                    CameraToggle.isCheck = true;
                    UI.SetActive(true);
                }
                else
                {
                    MovementToggle2.isCheck = true;
                    CameraToggle2.isCheck = true;
                    UI.SetActive(true);
                }
                MouseManager.HideCursor();
                ButtonActivator.IsUIShow = false;
                Debug.Log("Script đã được bật.");
            }
            else
            {
                Debug.LogWarning("Không tìm thấy script trên game object.");
            }
        }
    }
}

