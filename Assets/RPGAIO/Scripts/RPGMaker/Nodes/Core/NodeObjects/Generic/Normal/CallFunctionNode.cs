using System;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Custom", "")]
    public class CallFunctionNode : SimpleNode
    {
        public override string Name
        {
            get { return "Call Function"; }
        }

        public override string Description
        {
            get { return "Calls a function on a gameobject's component"; }
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
            Add("Target GameObject", PropertyType.GameObject, null, null, PropertySource.EnteredOrInput, PropertyFamily.Object);
            Add("Component Name", PropertyType.String, null, "", PropertySource.EnteredOrInput, PropertyFamily.Primitive);
            Add("Function Name", PropertyType.String, null, "", PropertySource.EnteredOrInput, PropertyFamily.Primitive);
            Add("Require Reciever", PropertyType.Bool, null, true, PropertySource.EnteredOnly, PropertyFamily.Primitive);

            for (int i = 1; i <= 5; i++)
            {  
                Add("UseParam" + i.ToString(), PropertyType.Bool, null, false, PropertySource.EnteredOnly, PropertyFamily.Primitive)
                    .WithSubParams(
                        SubParam("ParamType", PropertyType.StringArray, new string[] { "String", "Bool", "Int", "Float", "GameObject" }, 0, PropertySource.EnteredOnly, PropertyFamily.Primitive).IfTrue()
                            .WithSubParams(
                                SubParam("String Param", PropertyType.String, null, "", PropertySource.EnteredOrInput, PropertyFamily.Primitive).If(p => (int)p.Value == 0),
                                SubParam("Bool Param", PropertyType.Bool, null, false, PropertySource.EnteredOrInput, PropertyFamily.Primitive).If(p => (int)p.Value == 1),
                                SubParam("Int Param", PropertyType.Int, null, 0, PropertySource.EnteredOrInput, PropertyFamily.Primitive).If(p => (int)p.Value == 2),
                                SubParam("Float Param", PropertyType.Float, null, 0, PropertySource.EnteredOrInput, PropertyFamily.Primitive).If(p => (int)p.Value == 3),
                                SubParam("GameObject Param", PropertyType.GameObject, null, null, PropertySource.EnteredOrInput, PropertyFamily.Object).If(p => (int)p.Value == 4)
                            )
                    );
            }
        }

        protected override void Eval(NodeChain nodeChain)   
        {
            var gameObjectToCall = (GameObject) ValueOf("Target GameObject");
            var componentName = (string) ValueOf("Component Name");
            var functionName = (string) ValueOf("Function Name");
            var requireReciever = (bool)ValueOf("Require Reciever") ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver;

            var arguments = new List<object>();

            for (int i = 1; i <= 5; i++)
            {
                var paramName = "UseParam" + i.ToString();
                var useParam = (bool) ValueOf(paramName);
                if(useParam)
                {
                    var selectedParamType = (int)Parameter(paramName).ValueOf("ParamType");
                    switch (selectedParamType)
                    {
                        case 0:
                            var stringParam = (string) Parameter(paramName).SubParam("ParamType").Parameter.ValueOf("String Param");
                            arguments.Add(stringParam);
                            break;
                        case 1:
                            var boolParam = (bool)Parameter(paramName).SubParam("ParamType").Parameter.ValueOf("Bool Param");
                            arguments.Add(boolParam);
                            break;
                        case 2:
                            var intParam = (int)Parameter(paramName).SubParam("ParamType").Parameter.ValueOf("Int Param");
                            arguments.Add(intParam);
                            break;
                        case 3:
                            var floatParam = (float)Parameter(paramName).SubParam("ParamType").Parameter.ValueOf("Float Param");
                            arguments.Add(floatParam);
                            break;
                        case 4:
                            var gameObjParam = (GameObject)Parameter(paramName).SubParam("ParamType").Parameter.ValueOf("GameObject Param");
                            arguments.Add(gameObjParam);
                            break;
                    }
                }
                
            }

            if(arguments.Count == 0)
            {
                gameObjectToCall.GetComponent(componentName).SendMessage(functionName, requireReciever);
            }
            else if(arguments.Count == 1)
            {
                gameObjectToCall.GetComponent(componentName).SendMessage(functionName, arguments[0], requireReciever);
            }
            else
            {
                gameObjectToCall.GetComponent(componentName).SendMessage(functionName, arguments.ToArray(), requireReciever);
            }

        }
    }
}