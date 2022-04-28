using MadMollyCO.WorldOfAbyss;
using Application.Quests;
using Application.Quests.Data;
using Application.Quests.Providers;
using System;
using System.Collections;
using UnityEngine;
#if UNITY_STANDALONE || UNITY_SERVER

using WOTABackendAPI.Service;
#endif
namespace Assets.Scripts.Quest
{
    public class QuestTest : MonoBehaviour
    {
 
        public QuestData questData;
        QuestInstance questInstance;
        public QuestInstanceData questInstanceData;
        public bool StageComplite;
        // Use this for initialization
        QuestInstanceManager questInstanceManager;
        public static QuestTest questTest;
        void Awake()
        {
            questTest = this;
            QuestInfo.Init();
            TestIHumanoid testIHumanoid = new TestIHumanoid();
            IQuestInstanceDbProvider questInstanceTestProvider = new QuestInstanceTestProvider();
#if UNITY_STANDALONE || UNITY_SERVER
            questInstanceTestProvider = new QuestInstanceGrpcProvider(new QuestService("0a050204-b615-454a-b865-a060599acfadj8l754wt79"));
#endif
            /*
            QuestInfo questInfo = new QuestInfo(questData, testIHumanoid, QuestManager.GetInstance());
            questInstance = new QuestInstance(testIHumanoid, questInfo, questInstanceData);
            questInstance.Init(true);
            questInstance.OnInit += QuestInstance_OnInit;
            questInstance.OnComplite += QuestInstance_OnComplite;
            questInstance.OnFail += QuestInstance_OnFail;*/
            questInstanceManager = new QuestInstanceManager(questInstanceTestProvider, testIHumanoid, QuestManager.GetInstance());
            questInstanceManager.GetInstance(-2858, new QuestInstanceData() { CompliteStagesId = new long[0], CompliteTaskDate = DateTime.Now, CreateStagesId = new long[0], Date = DateTime.Now, Id = Guid.NewGuid().ToString(), Priority = 0, StageCount = 0, State = State.InProcess }, true);
        }

        private void QuestInstance_OnFail(QuestInfo arg1, QuestInstance arg2)
        {
            Debug.Log($"OnFail Description:{arg1.questData.Description}");
        }

        private void QuestInstance_OnComplite(QuestInfo arg1, QuestInstance arg2)
        {
            Debug.Log($"OnComplite Description:{arg1.questData.Description}");
        }

        private void QuestInstance_OnInit(QuestInfo arg1, QuestInstance arg2)
        {
            Debug.Log($"Init Description:{arg1.questData.Description}");
        }

        // Update is called once per frame
        void Update()
        {
            questInstanceManager.InvokeUpdate(Time.deltaTime);
            //questInstance.Update();
 
        }
    }
    public class TestIHumanoid : IHumanoidUnit
    {
        public long CharacterId { get; set; }

        public string CharacterName { get; set; }
    }
}