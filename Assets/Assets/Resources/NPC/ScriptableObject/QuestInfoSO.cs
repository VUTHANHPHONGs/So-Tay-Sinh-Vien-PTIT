using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Quest/Quest Info")]
    public class QuestInfoSO_1 : ScriptableObject
    {
        [field: SerializeField] public string id { get; private set; }

        [Header("General")]
        public string displayName;

        [Header("Requirements")]
        public int levelRequirement;
        public QuestInfoSO_1[] questPrerequisites;

        [Header("Steps")]
        public List<QuestStep_1> questSteps;

        [Header("Rewards")]
        public int goldReward;
        public int experienceReward;
        public List<ItemReward> itemRewards;

        // ensure the id is always the name of the Scriptable Object asset
        private void OnValidate()
        {
#if UNITY_EDITOR
            id = this.name;
#endif
        }
    }
    [System.Serializable]
    public class QuestStep_1
    {
        public string stepId;
        public StepMissionType missionType;
        [TextArea(2,4)]
        public string stepName;
        [TextArea(4,8)]
        public string stepDescription;
        public string npcId;
        public NPCState npcStatus;
        public NPCConservationSO conservation;
        public string nextStep;
    }

    [System.Serializable]
    public class ItemReward
    {
       // public ItemSO item;
        public int quantity;
    }

    public enum StepMissionType
    {
        TALKING,
        QUIZ,
        MAZE
    }
}


