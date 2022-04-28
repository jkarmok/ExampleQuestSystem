using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Application.Quests.Handlers
{
    public class NotifyHandler : BaseHandler
    {
        string debugString = "";
        MessageUI messageUI;
        public NotifyHandler(QuestInstance questInstance, QuestStageInfo stageInfo, string[] args) : base(questInstance, stageInfo, args)
        {
            debugString = string.Join(" ", args);
            Player player = (Player)questInstance.PlayerCharacter;
            messageUI = player.CanvasPlayer.QuestCanvas.messageUI;
        }

        public override void Invoke()
        {
            Debug.Log("QuestDebug: " + debugString);
            messageUI.Notify(debugString);
        }
    }
}
