using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Quests.Conditions
{
    public abstract class BaseCondition
    {
        protected string[] args;
        protected QuestInstance questInstance;
        protected QuestStageInfo stageInfo;
        public BaseCondition(QuestInstance questInstance, QuestStageInfo stageInfo, string[] args)
        {
            this.args = args;
            this.questInstance = questInstance;
            this.stageInfo = stageInfo;
        }
        public virtual void Update(float deltaTime)
        {

        }
        public abstract bool Invoke();
    }
}
