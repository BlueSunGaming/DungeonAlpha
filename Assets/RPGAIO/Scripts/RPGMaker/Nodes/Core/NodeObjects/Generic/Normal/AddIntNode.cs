using System;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Calculations", "Int")]
    public class AddIntNode : SimpleNode
    {
        public override string Name
        {
            get { return "Add Int Value"; }
        }

        public override string Description
        {
            get { return "Adds X to the int value."; }
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
            //Action
                //With Params
            //  to
            //Target of compatible Type'


            //Add Node:
                //Action: Add 'ValueParam' to Target
                //Target: Value of type: int, float, PhysicalDamage, ElementalDamage, AllElementalDamage, AllDamage

            //Value Param
            //Add("Value", PropertyType.Int, null, 1);

            //name: chance of landing some attack , 0 is 0% , 1 is 100%
            //input type: combatant
            //return type: float value

            /*        ,
        ,
        Attribute,
        Statistic,
        Trait,
        Vital,
        CustomVariable,
        Random*/

            //SETVALUE
            //ALL: popup of: raw (int, float, string, bool, Vector3), nodesWithReturnType, Cvars, Random
            //COMBAT:
                //core_damage_taken: physical, elemental [n] , totaldamage
                //default: attacker/defender (attr, statistic, trait, vital, level, enemytype, position)
            //DIALOG:
                //default: dialog/npc (attr, statistic, trait, vital, level, enemytype, position)
            //EVENT:
                //default: combatants.where(). (attr, statistic, trait, vital, level, enemytype, position)
            //Add("Target", PropertyType.GenericValue, VarType.Null, null); //param value format: (index, index/string/value), e.g. 0,abc-123 => 0 represents attribute, abc-123 represents attribute
            Add("Value", PropertyType.Int, null, 0 , PropertySource.EnteredOrInput); //param value format: (index, index/string/value), e.g. 0,abc-123 => 0 represents attribute, abc-123 represents attribute
            

            //New Style
            Add("Target", PropertyType.Int,null, 0, PropertySource.EnteredOrInput);

            //inside nodewindow:


            //popup : generic type
            //enumpopup / etc for subparam if applicable (e.g. attrs)
            //textfield/popup etc for value

            //E.G:
            //Type: Attribute                
            //AttributeName: Strength
            //ValueType: TotalValue

            //

        }

        protected override void Eval(NodeChain nodeChain)
        {
            var valueToAdd = Convert.ToInt32(ValueOf("Value"));

            var valueOfTarget = Convert.ToInt32(ValueOf("Target"));
            valueOfTarget += valueToAdd;
            ApplyFunctionTo("Target", o => { 
                o = valueOfTarget;
                return o;
            });
        }
    }
}