using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Application.Quests.Data
{
    [CreateAssetMenu(fileName = "QuestData", menuName = "ScriptableObjects/QuestData", order = 5)]
    public class QuestData : ScriptableObject
    {
        [ReadOnly]
        [SerializeField]
        public long Id;
        public string Icon;
        public string Title;
        public string Description;
        public long GiverEntityId;
        public long RewardMoney;
        public string[] OnComplite;
        public string[] OnFail;
        public string[] OnCancel;
        public string[] OnInit;
        public string[] OnCreate;

        public List<QuestStageData> Stages;
        public List<QuestRewardData> Rewards;

        public TaskFilter TaskType;

        public QuestData Clone()
        {
            return new QuestData()
            {
                Id = Id,
                Title = Title,
                Description = Description,
                OnComplite = OnComplite,
                OnFail = OnFail,
                OnCancel = OnCancel,
                OnInit = OnInit,
                GiverEntityId = GiverEntityId,
                Icon = Icon,
                RewardMoney = RewardMoney,
                TaskType = TaskType,
                OnCreate = OnCreate,
                Stages = Stages,
                Rewards = Rewards
            };
        }
#if UNITY_EDITOR
        private bool isInited;
        private void Awake()
        {
            isInited = true;
        }
        private void OnValidate()
        {
            if (isInited == false)
            {
                return;
            }
            if (Id == 0)
            {
                // Unique id from unity object system
                Id = this.GetInstanceID();
                saveContainer();
                Debug.Log("Id: " + Id);
            }


        }
        private void saveContainer()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif

    }
    [Serializable]
    public struct JournalData
    {
        public List<JournalTaskData> Tasks;
    }
    [Serializable]
    public struct JournalTaskData
    {
        public QuestInstanceData InstanceData;
    }
    public enum TaskFilter
    {
        defaultTask,
        dailyTasks,
        factionTask,
        compliteTask
    }
 
    [Serializable]
    public struct QuestInstanceData
    {
        public string Id;
        [NonSerialized]
        public long QuestId;
        public int Priority;
        public long[] CompliteStagesId;
        public int StageCount;
        public State State;
        public long CharacterId;
        public DateTime Date;
        public DateTime CompliteTaskDate;
        public long[] CreateStagesId;
    }
    [Serializable]
    public struct QuestStageData
    {
        public long Id;
        [NonSerialized]
        public long QuestId;
        public string Title;
        public string Description;
        public bool IsRequired;
        public State State;
        public string[] CompliteConditions;
        public string[] FailConditions;
        public string[] CancelConditions;
        public string[] OnComplite;
        public string[] OnFail;
        public string[] OnCancel;
        public string[] OnInit;
        public string[] OnCreate;

        public long[] RequiredStagesId;
        public QuestStageData Clone()
        {
            return new QuestStageData()
            {
                Id = Id,
                QuestId = QuestId,
                Title = Title,
                Description = Description,
                IsRequired = IsRequired,
                State = State,
                CompliteConditions = CompliteConditions,
                FailConditions = FailConditions,
                CancelConditions = CancelConditions,
                OnComplite = OnComplite,
                OnFail = OnFail,
                OnCancel = OnCancel,
                OnInit = OnInit,
                RequiredStagesId = RequiredStagesId,
                OnCreate = OnCreate
            };
        }
    }
    [Serializable]
    public struct QuestRewardData
    {
        public long Id;
        [NonSerialized]
        public long QuestId;
        public string Name;
        public int ItemId;
        public int Count;
        public long Money;
        public float Exp;
        public string EventName;
    }
    [Serializable]
    public struct QuestCancelData
    {
        public long QuestInstanceId;
        public TaskFilter TaskFilter;
    }
    public enum State
    {
        InProcess,
        Cancel,
        Fail,
        Complite
    }
}
