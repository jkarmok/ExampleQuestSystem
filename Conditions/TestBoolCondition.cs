using Assets.Scripts.Quest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Quests.Conditions
{
    public class TestBoolCondition : BaseCondition
    {
        public TestBoolCondition(QuestInstance questInstance, QuestStageInfo stageInfo, string[] args) : base(questInstance, stageInfo, args)
        {

        }
        public override bool Invoke()
        {
          return  QuestTest.questTest.StageComplite;
        }
    }
}
