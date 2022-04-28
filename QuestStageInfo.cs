using Application.Quests.Conditions;
using Application.Quests.Data;
using Application.Quests.Handlers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Quests
{
    public class QuestStageInfo
    {

        public bool IsHide
        {
            get
            {
                if (stageData.RequiredStagesId != null)
                {
                    if (questInstance.InstanceData.CompliteStagesId == null)
                        return true;
                    foreach (var stageId in stageData.RequiredStagesId) // если хотябы одна требуемая стадия не выполнена, то стадия скрыта
                    {
                        if (!questInstance.InstanceData.CompliteStagesId.Contains(stageId))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        public bool IsInit { get; private set; }
        public bool IsCreate { get; set; }

        public delegate void Handler(QuestInfo questInfo, QuestInstance questInstance, string[] args);

        public List<BaseCondition> CompliteConditions = new List<BaseCondition>();
        public List<BaseCondition> FailConditions = new List<BaseCondition>();

        public List<BaseHandler> CreateHandlers = new List<BaseHandler>();
        public List<BaseHandler> InitHandlers = new List<BaseHandler>();
        public List<BaseHandler> CompliteHandlers = new List<BaseHandler>();
        public List<BaseHandler> CancelHandlers = new List<BaseHandler>();
        public List<BaseHandler> FailHandlers = new List<BaseHandler>();

        public event Action<QuestStageInfo> OnCreate; // действия, которые выполняются, при создании стадии 1 раз
        public event Action<QuestStageInfo> OnInit; // действия, которые выполняются, при входе в стадию каждрый раз при входе
        public event Action<QuestStageInfo> OnComplite; //  действия, которые выполняются, если стадия выполнена
        public event Action<QuestStageInfo> OnFail; // действия, которые выполняются, если стадия провалена
        public event Action<QuestStageInfo> OnCancel; // действия, которые выполняются, если стадия отменена

        public QuestStageData stageData;
        QuestInstance questInstance;
        public QuestStageInfo(QuestStageData stageData, QuestInstance questInstance)
        {
            this.questInstance = questInstance;
            this.stageData = stageData;
            LoadConditions(stageData.CompliteConditions, CompliteConditions);
            LoadConditions(stageData.FailConditions, FailConditions);

            LoadHandlers(stageData.OnCreate, CreateHandlers);
            LoadHandlers(stageData.OnInit, InitHandlers);
            LoadHandlers(stageData.OnComplite, CompliteHandlers);
            LoadHandlers(stageData.OnCancel, CancelHandlers);
            LoadHandlers(stageData.OnFail, FailHandlers);

            OnCreate += QuestStageInfo_OnCreate;
            OnInit += QuestStageInfo_OnInit;
            OnComplite += QuestStageInfo_OnComplite;
            OnFail += QuestStageInfo_OnFail;
            OnCancel += QuestStageInfo_OnCancel;
        }

        private void QuestStageInfo_OnCreate(QuestStageInfo obj)
        {
            Console.WriteLine("OnCreate " + JsonConvert.SerializeObject(obj.stageData));
        }

        private void QuestStageInfo_OnCancel(QuestStageInfo obj)
        {
            Console.WriteLine("OnCancel " + JsonConvert.SerializeObject(obj.stageData));
        }

        private void QuestStageInfo_OnFail(QuestStageInfo obj)
        {
            Console.WriteLine("OnFail " + JsonConvert.SerializeObject(obj.stageData));
        }

        private void QuestStageInfo_OnComplite(QuestStageInfo obj)
        {
            Console.WriteLine("Stage OnComplite " + JsonConvert.SerializeObject(obj.stageData));
        }

        private void QuestStageInfo_OnInit(QuestStageInfo obj)
        {
            Console.WriteLine("OnInit " + JsonConvert.SerializeObject(obj.stageData));
        }

        public void CallCancel(QuestInstance questInstance)
        {
            if (OnCancel != null)
                OnCancel(this);

            foreach (var handler in CancelHandlers)
            {
                handler.Invoke();
            }
        }
        public void CallFail(QuestInstance questInstance)
        {
            if (OnFail != null)
                OnFail(this);

            foreach (var handler in FailHandlers)
            {
                handler.Invoke();
            }
        }
        public void CallComplite(QuestInstance questInstance)
        {
            if (OnComplite != null)
                OnComplite(this);
            Console.WriteLine("CallComplite handlers.count " + InitHandlers.Count);
            foreach (var handler in CompliteHandlers)
            {
                handler.Invoke();
                Console.WriteLine("handler init");
            }
        }
        public void CallInit(QuestInstance questInstance)
        {
            IsInit = true;
            if (OnInit != null)
                OnInit(this);

            foreach (var handler in InitHandlers)
            {
                handler.Invoke();
            }
        }
        public void CallCreate(QuestInstance questInstance)
        {
            IsCreate = true;
            if (OnCreate != null)
                OnCreate(this);

            foreach (var handler in CreateHandlers)
            {
                handler.Invoke();
            }
        }
        private void LoadConditions(string[] conditions, List<BaseCondition> conditionHandlers)
        {
            if (conditions == null)
                return;
            if (conditions.Length < 1)
                return;
            foreach (var conditionString in conditions)
            {
                conditionHandlers.Add(questInstance.QuestInfo.QuestManager.GetCondition(questInstance, this, conditionString));
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
                eventHandlers.Add(questInstance.QuestInfo.QuestManager.GetHandler(questInstance, this, handlerString));
            }
        }
    }
}
