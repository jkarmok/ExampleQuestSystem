using Application.Quests.Data;
using Application.Quests.Handlers;
using Application.Quests.Providers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Application.Quests
{
    /// <summary>
    /// Хранит и обрабатывает данные о текущем состоянии квеста взятого игроком
    /// </summary>
    public class QuestInstance
    {
        private List<BaseHandler> CreateHandlers = new List<BaseHandler>();
        private List<BaseHandler> InitHandlers = new List<BaseHandler>();
        private List<BaseHandler> CompliteHandlers = new List<BaseHandler>();
        private List<BaseHandler> CancelHandlers = new List<BaseHandler>();
        private List<BaseHandler> FailHandlers = new List<BaseHandler>();

        public event Action<QuestInfo, QuestInstance> OnCreate; // действия, которые выполняются, при выдаче задания (для выдачи предметов и изменения состояний)
        public event Action<QuestInfo, QuestInstance> OnInit; // действия, которые выполняются, при выдаче задания, а так-же при входе в игру (для инициализации некоторых моментов)
        public event Action<QuestInfo, QuestInstance> OnComplite; //  действия, которые выполняются, если задание выполнено
        public event Action<QuestInfo, QuestInstance> OnCanceled; // действия, которые выполняются, если задание отменено
        public event Action<QuestInfo, QuestInstance> OnFail; // действия, которые выполняются, если задание провалено


        public List<QuestStageInfo> Stages = new List<QuestStageInfo>();
        public List<QuestReward> Rewards = new List<QuestReward>();
        public QuestInfo QuestInfo;
        QuestInstanceData instanceData;
        public QuestInstanceData InstanceData { get { return instanceData; } }
        public IHumanoidUnit PlayerCharacter;
        private bool active = false;

        public string Id { get { return instanceData.Id; } }
        public long QuestId { get { return instanceData.QuestId; } }
        public State State { get { return instanceData.State; } }

        public event Action<QuestInstanceData> OnUpdate;
        public event Action<QuestRewardData> OnGetReward;
        public QuestInstance(IHumanoidUnit player, QuestInfo questInfo, QuestInstanceData instanceData)
        {
            this.PlayerCharacter = player;
            this.QuestInfo = questInfo;
            this.instanceData = instanceData;
        }

        private void QuestInstance_OnComplite(QuestInfo questInfo, QuestInstance questInstance)
        {
            foreach (var reward in Rewards)
            {
                if (reward.GroupName == defaultRewardGroup)
                {                
                    OnGetReward?.Invoke(reward.RewardData);
                }
            }
        }


        private string defaultRewardGroup = "default";
        public void SetRewardGroup(string rewardGroup)
        {
            defaultRewardGroup = rewardGroup.ToLower().Trim();
        }
        public string GetRewardGroup()
        {
            return defaultRewardGroup;
        }

        List<long> CompliteStagesId;
        List<long> CreateStagesId;
        private int StageCount = 0;

        public void Init(bool isCreate)
        {
            OnComplite = null;
            OnFail = null;
            OnInit = null;
            OnCanceled = null;
            OnCreate = null;

            OnComplite += QuestInfo_OnComplite;
            OnFail += QuestInfo_OnFail;
            OnInit += QuestInfo_OnInit;
            OnCanceled += QuestInfo_OnCanceled;
            OnCreate += QuestInfo_OnCreate;
            OnComplite += QuestInstance_OnComplite;

            instanceData.Date = DateTime.UtcNow;
            instanceData.CharacterId = PlayerCharacter.CharacterId;
 
            if (isCreate)
            {
                CompliteStagesId = new List<long>();
                CreateStagesId = new List<long>();
                LoadStages();
                Create();
                UpdateData();
            }
            else
            {
                if (instanceData.State != State.InProcess)
                {
                    return;
                }
                CompliteStagesId = new List<long>(instanceData.CompliteStagesId);
                CreateStagesId = new List<long>(instanceData.CreateStagesId);
                LoadStages();
                foreach (var stage in Stages) // проходим по всем стадиям
                {
                    if (instanceData.CompliteStagesId != null)
                        if (instanceData.CompliteStagesId.Contains(stage.stageData.Id))
                        {
                            stage.stageData.State = State.Complite;
                        }
                    if (instanceData.CreateStagesId != null)
                        if (instanceData.CreateStagesId.Contains(stage.stageData.Id))
                        {
                            stage.IsCreate = true;
                        }
                }
            }
            LoadRewards();
            LoadHandlers(QuestInfo.questData.OnCreate, CreateHandlers);
            LoadHandlers(QuestInfo.questData.OnInit, InitHandlers);
            LoadHandlers(QuestInfo.questData.OnComplite, CompliteHandlers);
            LoadHandlers(QuestInfo.questData.OnCancel, CancelHandlers);
            LoadHandlers(QuestInfo.questData.OnFail, FailHandlers);

            active = true;
            if (isCreate)
            {
                Create();
            }
            CallInit();
            Console.WriteLine("QuestInstance Init");
        }
        public void CallCreate()
        {
            if (OnCreate != null)
                OnCreate(QuestInfo, this);
        }
        public void CallFail()
        {
            if (OnFail != null)
                OnFail(QuestInfo, this);
        }
        public void CallComplite()
        {
            if (OnComplite != null)
                OnComplite(QuestInfo, this);
        }
        public void CallInit()
        {
            if (OnInit != null)
                OnInit(QuestInfo, this);
        }
        public void CallCanceled()
        {
            if (OnCanceled != null)
                OnCanceled(QuestInfo, this);
        }
        private void QuestInfo_OnCanceled(QuestInfo obj, QuestInstance questInstance)
        {
            Console.WriteLine("QuestInfo_OnCanceled count " + CancelHandlers.Count);
            foreach (var handler in CancelHandlers)
            {
                handler.Invoke();
            }
        }

        private void LoadHandlers(string[] handlers, List<BaseHandler> eventHandlers)
        {
            if (handlers == null)
                return;
            if (handlers.Length < 1)
                return;
            foreach (var handlerString in handlers)
            {
                eventHandlers.Add(QuestInfo.QuestManager.GetHandler(this, null, handlerString));
            }
        }
        private void LoadStages()
        {
            StageCount = 0;

            var questStagesData = QuestInfo.GetInstanceStages(QuestInfo.questData.Stages);
            foreach (var questStageData in questStagesData)
            {
                Stages.Add(new QuestStageInfo(questStageData, this));
                if (questStageData.IsRequired)
                    StageCount++;
            }
        }
        private void LoadRewards()
        {
            var questRewardsData = QuestInfo.GetInstanceRewards();
            foreach (var questRewardData in questRewardsData)
            {
                Rewards.Add(new QuestReward(this, questRewardData));
            }
        }
        private void QuestInfo_OnCreate(QuestInfo obj, QuestInstance questInstance)
        {
            foreach (var handler in CreateHandlers)
            {
                try
                {
                    handler.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        private void QuestInfo_OnInit(QuestInfo obj, QuestInstance questInstance)
        {
            foreach (var handler in InitHandlers)
            {
                handler.Invoke();
            }
        }

        private void QuestInfo_OnFail(QuestInfo obj, QuestInstance questInstance)
        {
            foreach (var handler in FailHandlers)
            {
                handler.Invoke();
            }
        }

        private void QuestInfo_OnComplite(QuestInfo obj, QuestInstance questInstance)
        {
            foreach (var handler in CompliteHandlers)
            {
                try
                {
                    handler.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public void Create()
        {
            CallCreate();
        }
        public async Task Cancel()
        {
            Console.WriteLine("QuestInstance Cancel ");
            if (active)
            {
                instanceData.State = State.Cancel;
                UpdateData();

                foreach (var stage in Stages)
                {
                    if (stage.stageData.State == State.InProcess)
                    {
                        stage.CallCancel(this);
                        break;
                    }
                }
                CallCanceled();  // отменить квест
                Destroy();
            }
        }

        void UpdateData()
        {
            // save request
            OnUpdate?.Invoke(InstanceData);
        }
        void UpdateCondition(QuestStageInfo stageInfo, float deltaTime)
        {
            foreach (var condition in stageInfo.FailConditions)
            {
                condition.Update(deltaTime);
            }
            foreach (var condition in stageInfo.CompliteConditions)
            {
                condition.Update(deltaTime);
            }
        }
        void UpdateHandler(QuestStageInfo stageInfo, float deltaTime)
        {
            foreach (var baseHandler in stageInfo.CreateHandlers)
            {
                baseHandler.Update(deltaTime);
            }
            foreach (var baseHandler in stageInfo.InitHandlers)
            {
                baseHandler.Update(deltaTime);
            }
            foreach (var baseHandler in stageInfo.CancelHandlers)
            {
                baseHandler.Update(deltaTime);
            }
            foreach (var baseHandler in stageInfo.CompliteConditions)
            {
                baseHandler.Update(deltaTime);
            }
            foreach (var baseHandler in stageInfo.FailHandlers)
            {
                baseHandler.Update(deltaTime);
            }
        }
        bool CheckFailStage(QuestStageInfo stageInfo)
        {
            bool stageIsFail = false;
            foreach (var condition in stageInfo.FailConditions) // проходим по всем условиям
            {
                if (stageIsFail == false) // если не провалил идем дальше
                {
                    stageIsFail = condition.Invoke();
                }

                if (stageIsFail)
                {
                    return stageIsFail; // если провалил хоть 1 то все
                }
            }
            return stageIsFail;
        }
        bool CheckCompliteStage(QuestStageInfo stageInfo)
        {
            bool stageIsComplite = true;
            foreach (var condition in stageInfo.CompliteConditions) // проходим по всем условиям
            {
                if (stageIsComplite)
                {
                    stageIsComplite = condition.Invoke();
                }
            }
            return stageIsComplite;
        }

        public void Destroy()
        {
            active = false;
            OnUpdate = null;
            OnGetReward = null;
        }
        public void InvokeUpdate(float deltaTime)
        {
            if (active)
                UpdateConditionHandler(deltaTime);
        }


        /// <summary>
        /// Обновляет текущее состояние квеста исходи из условий
        /// </summary>
        void UpdateConditionHandler(float deltaTime)
        {
            try
            {
                if (instanceData.State != State.InProcess)
                {
                    return;
                }
                int CompliteStageCount = 0;

                foreach (var stage in Stages) // проходим по всем стадиям
                {
                    if (stage.stageData.State == State.Fail) // если провалили, то уже нет смысла проверять
                        continue;

                    if (stage.stageData.State == State.Complite) // если стадия уже выполнена, то пропускаем ее
                    {
                        if (stage.stageData.IsRequired)
                        {
                            CompliteStageCount++;
                        }
                        continue;
                    }

                    if (stage.IsInit == false)
                    {
                        stage.CallInit(this);
                    }
                    if (stage.IsCreate == false)
                    {
                        CreateStagesId.Add(stage.stageData.Id);
                        instanceData.CreateStagesId = CreateStagesId.ToArray();
                        UpdateData();
                        stage.CallCreate(this);
                    }
                    UpdateCondition(stage, deltaTime);
                    UpdateHandler(stage, deltaTime);

                    bool checkFail = false;
                    if (stage.stageData.State == State.InProcess)
                    {
                        checkFail = CheckFailStage(stage); // проверяем на фейл если уже не феил
                    }

                    if (checkFail)
                    {
                        stage.stageData.State = State.Fail;
                        stage.CallFail(this); // провалил стадию квеста
                        UpdateData();

                        if (stage.stageData.IsRequired) // если требуется для выполнения задания то фейл и стадии и задания
                        {
                            instanceData.State = State.Fail;
                            UpdateData();
                            CallFail();  // провалил квест
                            Destroy();
                            break;
                        }
                        continue;
                    }

                    bool checkComplite = CheckCompliteStage(stage);

                    if (checkComplite) // если все условия данной стадии истина, то мы как минимум остановились на ней, либо даже дальше
                    {
                        stage.stageData.State = State.Complite;
                        CompliteStagesId.Add(stage.stageData.Id);
                        if (stage.stageData.IsRequired)
                        {
                            CompliteStageCount++;
                        }
                        instanceData.CompliteStagesId = CompliteStagesId.ToArray();
                        instanceData.StageCount = CompliteStageCount;
                        UpdateData();
                        stage.CallComplite(this);
                    }
                    else // иначе прерываем все проверки
                    {
                        break;
                    }
                }

                instanceData.CompliteStagesId = CompliteStagesId.ToArray();
                instanceData.StageCount = CompliteStageCount;

                if (CompliteStageCount >= StageCount)
                {
                    instanceData.State = State.Complite;
                    UpdateData();
                    CallComplite();
                    Destroy();// уберет инстанс из цикла проверки         

                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
    }
}
