using Interaction;
//using PlayerStatsController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManager_1
{
    public class QuestManager_1 : MonoBehaviour
    {
        Dictionary<string, NPCController> npcMap;
        Dictionary<string, Quest_1> questMap;
       // PlayerStats playerStats;
   
        [Header("Config")]
        [SerializeField] private bool loadQuestState = true;

        // Start is called before the first frame update
        void Start()
        {
            questMap = CreateQuestMap();
            npcMap = CreateNPCMap();
            //playerStats = FindObjectOfType<PlayerStats>();
            UpdateRequirementsMetQuest();
        }

        #region Handle Quest Step Update

        public void UpdateRequirementsMetQuest()
        {
            // loop through ALL quests
            foreach (Quest_1 quest in questMap.Values)
            {
                // if we're now meeting the requirements, switch over to the CAN_START state
                if (quest.state == QuestState_1.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
                {
                    quest.ChangeQuestState(QuestState_1.CAN_START);
                    InitQuestStep(quest.info.questSteps[quest.currentQuestStepIndex], quest.info.id);
                }
            }
        }

        public void InitQuestStep(QuestStep_1 questStep, string questId)
        {
            NPCController npc = GetNPCById(questStep.npcId);
            npc.UpdateQuestStep(questStep, questId);
        }

        public void OnFinishQuestStep(string questId)
        {
            bool isQuestFinished = !questMap[questId].TryNextStep();

            if (isQuestFinished)
            {
                Debug.Log("Quest complete..reward");
            } else
            {
                Quest_1 quest = questMap[questId];
                InitQuestStep(quest.info.questSteps[quest.currentQuestStepIndex], quest.info.id);
            }
        }

        #endregion

        #region Handle Reward
        public int CheckFinishedQuestsForMedals()
        {
            int medal = 0; // Reset Medal trước khi kiểm tra

            foreach (Quest_1 quest in questMap.Values)
            {
                if (quest.state == QuestState_1.FINISHED)
                {
                    medal++; // Tăng số huân chương nếu quest đã hoàn thành
                }
            }

            return medal;
        }

        #endregion

        #region Handle Quest Manager Data

        private bool CheckRequirementsMet(Quest_1 quest)
        {
            // start true and prove to be false
            bool meetsRequirements = true;

            // check player level requirements
          //  if (playerStats.characterLevel < quest.info.levelRequirement)
        //    {
        //        meetsRequirements = false;
        //    }

            // check quest prerequisites for completion
            foreach (QuestInfoSO_1 prerequisiteQuestInfo in quest.info.questPrerequisites)
            {
                if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState_1.FINISHED)
                {
                    meetsRequirements = false;
                }
            }

            return meetsRequirements;
        }

        private Quest_1 GetQuestById(string id)
        {
            Quest_1 quest = questMap[id];
            if (quest == null)
            {
                Debug.LogError("ID not found in the Quest Map: " + id);
            }
            return quest;
        }

        private Dictionary<string, Quest_1> CreateQuestMap()
        {
            // loads all QuestInfo Scriptable Objects under the Assets/Resources/Quests folder
            QuestInfoSO_1[] allQuests = Resources.LoadAll<QuestInfoSO_1>("Quests");
            
            // Create the quest map
            Dictionary<string, Quest_1> questMap = new Dictionary<string, Quest_1>();
            foreach (QuestInfoSO_1 questInfo in allQuests)
            {
                if (questMap.ContainsKey(questInfo.id))
                {
                    Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
                }
                questMap.Add(questInfo.id, LoadQuest(questInfo));
            }
            return questMap;
        }

        private Quest_1 LoadQuest(QuestInfoSO_1 questInfo)
        {
            Quest_1 quest = null;
            try
            {
                // load quest from saved data
                if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
                {
                    string serializedData = PlayerPrefs.GetString(questInfo.id);
                    QuestData_1 questData = JsonUtility.FromJson<QuestData_1>(serializedData);
                    quest = new Quest_1(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
                }
                // otherwise, initialize a new quest
                else
                {
                    quest = new Quest_1(questInfo);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load quest with id " + quest.info.id + ": " + e);
            }
            return quest;
        }

        private Dictionary<string, NPCController> CreateNPCMap()
        {
            NPCController[] npcs = FindObjectsOfType<NPCController>();

            // Create the quest map
            Dictionary<string, NPCController> npcMap = new Dictionary<string, NPCController>();
            foreach (NPCController npc in npcs)
            {
                if (npcMap.ContainsKey(npc.npcInfo.npcId))
                {
                    Debug.LogWarning("Duplicate ID found when creating NPC map: " + npc.npcInfo.npcId);
                }
                npcMap.Add(npc.npcInfo.npcId, npc);
            }
            return npcMap;
        }

        private NPCController GetNPCById(string id)
        {
            NPCController npc = npcMap[id];
            if (npc == null)
            {
                Debug.LogError("ID not found in the NPC Map: " + id);
            }
            return npc;
        }

        #endregion
    }
}

