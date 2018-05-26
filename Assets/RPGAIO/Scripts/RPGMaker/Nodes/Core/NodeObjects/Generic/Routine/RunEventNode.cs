using System.Collections;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class RunEventNode : SimpleNode
    {
        [JsonIgnore]
        public string EventID
        {
            get { return (string) ValueOf("Event"); }
        }

        public override string Name
        {
            get { return "Run Event"; }
        }

        public override string Description
        {
            get { return "Runs an event"; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return true;
            }
        }



        public override string NextNodeLinkLabel(int index)
        {
            return "Next";
        }

        protected override void SetupParameters()
        {
            Add("Event", PropertyType.Event, null, "");
            Add("Async?", PropertyType.Bool, null, false);
        }

        public override bool IsRoutine
        {
            get { return true; }
        }

        protected override void Eval(NodeChain nodeChain)
        {
        }

        public override IEnumerator Routine(NodeChain nodeChain)
        {
            //todo: any other things which need to be 'reset'

            var eventId = (string)ValueOf("Event");
            var eventStatus = GetObject.EventHandler.RunEvent(eventId);
            while(eventStatus.Status != EventStatus.Complete)
            {
                yield return null;
            }

            if(eventStatus.Status == EventStatus.Failed)
            {
                Debug.LogError("Event " + eventId + " Failed!");
            }

        }
    }
}