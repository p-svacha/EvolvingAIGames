using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class AIPlayer : Player
    {
        public Subject Subject;
        public override string DisplayName => Subject.Name;

        public AIPlayer(Subject subject)
        {
            if (subject == null) throw new System.Exception("Cannot make AI player with subject = NULL");
            Subject = subject;
        }

        public override List<Upgrade> GetInputs()
        {
            throw new System.NotImplementedException();
        }
    }
}
