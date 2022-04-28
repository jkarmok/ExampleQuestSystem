using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Application.Quests.Handlers
{
    public class DebugHandler : BaseHandler
    {
        string debugString = "";
        public DebugHandler(QuestInstance questInstance, QuestStageInfo stageInfo, string[] args) : base(questInstance, stageInfo, args)
        {
            debugString = string.Join(" ", args);
        }

        public override void Invoke()
        {
            Debug.Log("QuestDebug: "+ debugString);
        }
    }
}
