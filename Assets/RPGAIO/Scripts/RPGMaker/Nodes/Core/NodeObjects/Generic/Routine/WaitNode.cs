using System;
using System.Collections;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class WaitNode : SimpleNode
    {

        public override string Name
        {
            get { return "Wait For Seconds"; }
        }

        public override string Description
        {
            get { return "Waits for given seconds"; }
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
            Add("Seconds", PropertyType.Float, null, 1.0F);
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
            var seconds = Convert.ToSingle(ValueOf("Seconds"));
            yield return new WaitForSeconds(seconds);
        }
    }
}