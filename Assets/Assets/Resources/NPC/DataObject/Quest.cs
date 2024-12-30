using Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState_1
{
    REQUIREMENTS_NOT_MET,
    CAN_START,
    IN_PROGRESS,
    FINISHED
}

public class Quest_1
{
    // static info
    public QuestInfoSO_1 info;

    // state info
    public QuestState_1 state;
    public int currentQuestStepIndex;
    public QuestStepState_1[] questStepStates;

    public bool isCurrentStepExists() => (currentQuestStepIndex < info.questSteps.Count);

    public Quest_1(QuestInfoSO_1 questInfo)
    {
        info = questInfo;
        state = QuestState_1.REQUIREMENTS_NOT_MET;
        currentQuestStepIndex = 0;
        questStepStates = new QuestStepState_1[info.questSteps.Count];
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState_1();
        }
    }

    public Quest_1(QuestInfoSO_1 questInfo, QuestState_1 questState, int currentQuestStepIndex, QuestStepState_1[] questStepStates)
    {
        this.info = questInfo;
        this.state = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;

        // if the quest step states and prefabs are different lengths,
        // something has changed during development and the saved data is out of sync.
        if (this.questStepStates.Length != this.info.questSteps.Count)
        {
            Debug.LogWarning("Quest Step Prefabs and Quest Step States are "
                + "of different lengths. This indicates something changed "
                + "with the QuestInfo and the saved data is now out of sync. "
                + "Reset your data - as this might cause issues. QuestId: " + this.info.id);
        }
    }

    public bool TryNextStep()
    {
        currentQuestStepIndex++;
        //check if there is step behind
        if (isCurrentStepExists())
        {
            ChangeQuestState(QuestState_1.IN_PROGRESS);
            return true;
        } 
        else
        {
            ChangeQuestState(QuestState_1.FINISHED);
            return false;
        }
    }

    public void ChangeQuestState(QuestState_1 state)
    {
        this.state = state;
    }

}

[System.Serializable]
public class QuestData_1
{
    public QuestState_1 state;
    public int questStepIndex;
    public QuestStepState_1[] questStepStates;

    public QuestData_1(QuestState_1 state, int questStepIndex, QuestStepState_1[] questStepStates)
    {
        this.state = state;
        this.questStepIndex = questStepIndex;
        this.questStepStates = questStepStates;
    }
}

[System.Serializable]
public class QuestStepState_1
{
    public string state;
    public string status;


    public QuestStepState_1(string state, string status)
    {
        this.state = state;
        this.status = status;
    }

    public QuestStepState_1()
    {
        this.state = "";
        this.status = "";
    }
}
