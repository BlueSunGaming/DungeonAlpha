//using System;
//using System.Collections.Generic;
//using System.Linq;
//using LogicSpawn.RPGMaker.Generic;
//using UnityEngine;
//using UnityEditor;
//
//namespace LogicSpawn.RPGMaker.Editor
//{
//    public abstract class Rme_NodeWindow : EditorWindow
//    {
//        public static Rme_NodeWindow Window;
//        public NodeTreeType WindowType;
//        public NodeBank NodeBank
//        {
//            get
//            {
//                switch(WindowType)
//                {
//                    case NodeTreeType.Combat:
//                        return Rm_RPGHandler.Instance.Nodes.CombatNodeBank;
//                    case NodeTreeType.Dialog:
//                        return Rm_RPGHandler.Instance.Nodes.DialogNodeBank;
//                    case NodeTreeType.Event:
//                        return Rm_RPGHandler.Instance.Nodes.EventNodeBank;
//                    case NodeTreeType.WorldMap:
//                        return Rm_RPGHandler.Instance.Nodes.WorldMapNodeBank;
//                    default:
//                        throw new ArgumentOutOfRangeException();
//                }
//            }
//        }
//
//        public Rme_NodeWindowDraw Draw;
//
//        public Rm_NodeTree SelectedNodeTree;
//        public List<Rm_Node> Nodes
//        {
//            get { return SelectedNodeTree.Nodes; }
//        }
//
//        public int SelectedNodeTreeIndex;
//        public Rm_Node SelectedNode;
//
//        public Rm_NodeLink NodeLinkFrom = null;
//        public Rm_Node NodeLinkTo = null;
//
//        private bool Loaded = false;
//
//        public string[] PrimaryTrees;
//        private Vector2 nodeScrollPos = Vector2.zero;
//        private int selectedRandomMinIndex;
//        private int selectedRandomMaxIndex;
//        private int selectedVarSetterBoolResult;
//        private bool SnapWindows;
//        private bool DrawGrid;
//
//        private Rmh_ASVT ASVT
//        {
//            get { return Rm_RPGHandler.Instance.ASVT; }
//        }
//
//        public Rme_NodeWindow()
//        {
//            Draw = new Rme_NodeWindowDraw();
//        }
//
//        void OnEnable()
//        {
//            EditorGameDataSaveLoad.LoadIfNotLoadedFromEditor();
//        }
//
//        void OnDestroy()
//        {
//
//            var option = EditorUtility.DisplayDialogComplex("Save Changes?", "Would you like to save changes to the Game Data? \n\n Note: Discarding will reload the previous game Data (not just node info)",
//                                                            "Save", "Close without Saving", "Discard");
//
//            if(option == 0)
//            {
//                EditorGameDataSaveLoad.SaveGameData();
//            }
//            else if (option == 2)
//            {
//                EditorGameDataSaveLoad.LoadGameDataFromEditor();
//            }
//        }
//
//        void OnGUI()
//        {
//            try
//            {
//                OnGUIx();
//            }
//            catch (Exception e)
//            {
//                Debug.Log(e);
//            }
//        }
//
//        void Update()
//        {
//            if(!Loaded)
//            {
//                if(NodeBank.NodeTrees.Count > 0)
//                {
//                    SelectedNodeTree = NodeBank.NodeTrees[0];
//                    InitNodes();
//                }
//                else
//                {
//                    SelectedNodeTree = null;
//                }
//
//                Loaded = true;
//            }
//
//            if(NodeLinkFrom != null && NodeLinkTo != null)
//            {
//                if(NodeLinkFrom.ParentNode == NodeLinkTo)
//                {
//                    Debug.Log("You cannot link a node to itself.");
//                }
//                else if(NodeLinkTo.NodeLinkingToThis != null && 
//                        !NodeLinkTo.AllowMultipleNodesToLinkToThis)
//                {
//                    Debug.Log("A node is already linking to this");        
//                }
//                else if(NodeLinkFrom.ParentNode.DisallowedLinks.All(d => d != NodeLinkTo.NodeType))
//                {
//                    if(NodeLinkTo.NextPoints.All(n => n.LinkedNode != NodeLinkFrom.ParentNode))
//                    {
//
//                        if(NodeLinkFrom.LinkedNode != null)
//                        {
//                            NodeLinkFrom.LinkedNode.NodeLinkingToThis = null;
//                        }
//
//
//                        NodeLinkFrom.LinkedNode = NodeLinkTo;
//                        NodeLinkTo.NodeLinkingToThis = NodeLinkFrom.ParentNode;
//                    }
//                    else
//                    {
//                        Debug.Log("These are already linked.");
//                    }
//                }
//                else
//                {
//                    Debug.Log("You cannot link those.");
//                }
//
//                NodeLinkFrom = null;
//                NodeLinkTo = null;
//            }
//        }
//
//        public abstract void InitNodes();
//
//        public abstract void AddContextMenu(Event evt, Vector2 mousePos);
//
//        public Rm_Node[] GetResultNodes (bool booleanNodes)
//        {
//            if (booleanNodes)
//            {
//                return NodeBank.NodeTrees
//                    .SelectMany(n => n.Nodes)
//                    .Where(n => n.NodeType == Rm_NodeType.Result_Node && n.IsBooleanResultNode).ToArray();
//            }
//
//            return NodeBank.NodeTrees
//                    .SelectMany(n => n.Nodes)
//                    .Where(n => n.NodeType == Rm_NodeType.Result_Node).ToArray();     
//        }
//
//        public void NodeSettings()
//        {
//
//            //todo: regions, revisit code and if possible refactor into less code or seperate functions/classes
//
//            var varNode = SelectedNode as Rm_VariableNode;
//            var calcNode = SelectedNode as Rm_CalcNode;
//            var conditionNode = SelectedNode as Rm_ConditionNode;
//            var boolNode = SelectedNode as Rm_BooleanNode;
//            var varSetterNode = SelectedNode as Rm_VarSetterNode;
//            var eventNode = SelectedNode as Rm_RunEventNode;
//            var responseNode = SelectedNode as Rm_ResponseNode;
//            var isPlayerNode = SelectedNode as Rm_IsPlayerNode;
//
//            if (isPlayerNode != null)
//            {
//                RPGMakerGUI.Toggle("Check Attacker?" , ref isPlayerNode.IsAttacker);
//            }
//
//            if (responseNode != null)
//            {
//                RPGMakerGUI.Label("Response:");
//                responseNode.Response = RPGMakerGUI.TextArea(responseNode.Response);
//                if(responseNode.NodeType == Rm_NodeType.Player_Response)
//                {
//                    RPGMakerGUI.Toggle("Show based on Quest?", ref responseNode.HasQuestCondition);
//                    if (responseNode.HasQuestCondition)
//                    {
//                        RPGMakerGUI.Toggle("Player Meets Quest Req. ?", ref responseNode.MeetsRequirements);
//                        responseNode.ReqQuestStatus = (QuestStatus)EditorGUILayout.EnumPopup("Req. Quest Status:", responseNode.ReqQuestStatus);
//
//                        var questArray = Rm_RPGHandler.Instance.Repositories.Quests.AllQuests;
//                        RPGMakerGUI.PopupList(ref responseNode.QuestID, questArray, "Quest");
//                    }
//                }
//                else
//                {
//                    if(GUILayout.Button("Add Response Slot", GUILayout.Height(30)))
//                    {
//                        responseNode.NextPoints.Add(new Rm_NodeLink(responseNode) {Label = ">"});
//                    }
//                }
//            }
//
//            if (SelectedNode.NodeType == Rm_NodeType.Begin_Quest || SelectedNode.NodeType == Rm_NodeType.Complete_Quest)
//            {
//                //todo: only first quest in a quest chain rather than all
//                var customVariableArray = Rm_RPGHandler.Instance.Repositories.Quests.AllQuests;
//                RPGMakerGUI.PopupList(ref SelectedNode.Identifier, customVariableArray,"Quest");
//            }
//
//            if (SelectedNode.NodeType == Rm_NodeType.Result_Node)
//            {
//                SelectedNode.Identifier = RPGMakerGUI.TextField("Identifier: ", SelectedNode.Identifier);
//                SelectedNode.IsBooleanResultNode = EditorGUILayout.Toggle("Returns Boolean? ",
//                                                                          SelectedNode.IsBooleanResultNode);
//                if (SelectedNode.IsBooleanResultNode)
//                {
//                    EditorGUILayout.HelpBox("Connect this to a node link that has the [TRUE/FALSE] label and this value will be used as the boolean returned.", MessageType.Info);
//                }
//            }
//
//            if (calcNode != null)
//            {
//                calcNode.CalcType = (Rm_CalcNodeType)EditorGUILayout.EnumPopup("CalcType:", calcNode.CalcType);
//            }
//
//            #region EventNode
//
//            if (eventNode != null)
//            {
//                var allList = Rm_RPGHandler.Instance.Nodes.EventNodeBank.NodeTrees;
//                RPGMakerGUI.PopupList(ref eventNode.EventName, allList, "Event");
//            }
//
//            #endregion
//
//
//            #region Var Setter
//
//            if (varSetterNode != null)
//            {
//                var customVariableArray = Rm_RPGHandler.Instance.DefinedVariables.Vars;
//                RPGMakerGUI.PopupList(ref varSetterNode.CustomVarName, customVariableArray, "CustomVar");
//
//                varSetterNode.CustomVarType =
//                    Rm_RPGHandler.Instance.DefinedVariables.Vars.First(v => v.ID == varSetterNode.CustomVarName).
//                        VariableType;
//
//                varSetterNode.SetToType =
//                    (Rm_VarSetterType) EditorGUILayout.EnumPopup("Set Type", varSetterNode.SetToType);
//                if (varSetterNode.SetToType == Rm_VarSetterType.ResultNode)
//                {
//
//                    if (varSetterNode.CustomVarType == Rmh_CustomVariableType.String)
//                    {
//                        Debug.Log("Cannot set string with result node.");
//                        varSetterNode.SetToType = Rm_VarSetterType.Value;
//                    }
//                    else
//                    {
//                        var boolResultNodeArray = varSetterNode.CustomVarType == Rmh_CustomVariableType.Bool
//                                                            ? GetResultNodes(true)
//                                                            : GetResultNodes(false);
//                        RPGMakerGUI.PopupList(ref varSetterNode.SetToResultNodeName, boolResultNodeArray, "Result Node", "ID", "Identifier");
//                    }
//                }
//                else if (varSetterNode.SetToType == Rm_VarSetterType.Value)
//                {
//                    switch (varSetterNode.CustomVarType)
//                    {
//                        case Rmh_CustomVariableType.Float:
//                            varSetterNode.SetToFloat = RPGMakerGUI.FloatField("Set To:", varSetterNode.SetToFloat);
//                            break;
//                        case Rmh_CustomVariableType.Int:
//                            varSetterNode.SetToInt = RPGMakerGUI.IntField("Set To:", varSetterNode.SetToInt);
//                            break;
//                        case Rmh_CustomVariableType.String:
//                            varSetterNode.SetToString = RPGMakerGUI.TextField("Set To:", varSetterNode.SetToString);
//                            break;
//                        case Rmh_CustomVariableType.Bool:
//                            selectedVarSetterBoolResult = varSetterNode.SetToBool ? 0 : 1;
//                            selectedVarSetterBoolResult = EditorGUILayout.Popup("Set To:", selectedVarSetterBoolResult,
//                                                                                new[] {"True", "False"});
//                            varSetterNode.SetToBool = selectedVarSetterBoolResult == 0;
//                            break;
//                        default:
//                            throw new ArgumentOutOfRangeException();
//                    }
//                }
//
//                varSetterNode.SetOnce = EditorGUILayout.Toggle("Set Once Only?", varSetterNode.SetOnce);
//            }
//
//            #endregion
//
//            if (boolNode != null)
//            {
//                boolNode.BoolType = (Rm_BooleanNodeType)EditorGUILayout.EnumPopup("Boolean Type:", boolNode.BoolType);
//                if (boolNode.BoolType == Rm_BooleanNodeType.ResultNode)
//                {
//                    var boolResultNodeArray = GetResultNodes(true);
//                    RPGMakerGUI.PopupList(ref boolNode.BoolIdentifier, boolResultNodeArray, "Result Node", "ID", "Identifier");
//                }
//                else
//                {
//                    var customVariableArray = Rm_RPGHandler.Instance.DefinedVariables.Vars;
//                    RPGMakerGUI.PopupList(ref boolNode.BoolIdentifier, customVariableArray, "Custom Var");
//
//                    boolNode.CustomVarType =
//                        Rm_RPGHandler.Instance.DefinedVariables.Vars.First(v => v.ID == boolNode.BoolIdentifier).VariableType;
//
//                    if (boolNode.CustomVarType != Rmh_CustomVariableType.Bool)
//                    {
//                        switch (boolNode.CustomVarType)
//                        {
//                            case Rmh_CustomVariableType.Float:
//                                boolNode.EqualsFloat = RPGMakerGUI.FloatField("Equals Value:", boolNode.EqualsFloat);
//                                break;
//                            case Rmh_CustomVariableType.Int:
//                                boolNode.EqualsInt = RPGMakerGUI.IntField("Equals Value:", boolNode.EqualsInt);
//                                break;
//                            case Rmh_CustomVariableType.String:
//                                boolNode.EqualsString = RPGMakerGUI.TextField("Equals Value:", boolNode.EqualsString);
//                                break;
//                            default:
//                                throw new ArgumentOutOfRangeException();
//                        }
//                    }
//                }
//            }
//
//            if (conditionNode != null)
//            {
//                conditionNode.ConditionVarType =
//                    (Rm_ConditionVarType)EditorGUILayout.EnumPopup("Condition Type:", conditionNode.ConditionVarType);
//
//                if (conditionNode.ConditionVarType != Rm_ConditionVarType.AtOrAboveLevel)
//                {
//                    switch (conditionNode.ConditionVarType)
//                    {
//                        case Rm_ConditionVarType.HasItem:
//                            var itemArray = Rm_RPGHandler.Instance.Repositories.Items.AllItems;
//                            RPGMakerGUI.PopupList(ref conditionNode.StringVar, itemArray, "Item");
//                            break;
//                        case Rm_ConditionVarType.HasSkill:
//                            var skillArray = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills;
//                            RPGMakerGUI.PopupList(ref conditionNode.StringVar, skillArray, "Skill");
//                            break;
//                        case Rm_ConditionVarType.TalentEnabled:
//                            var talentArray = Rm_RPGHandler.Instance.Repositories.Talents.AllTalents;
//                            RPGMakerGUI.PopupList(ref conditionNode.StringVar, talentArray, "Talent");
//                            break;
//                        case Rm_ConditionVarType.HasStatusEffect:
//                            var statEffectArray = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects;
//                            RPGMakerGUI.PopupList(ref conditionNode.StringVar, statEffectArray, "Status Effect");
//                            break;
//                        case Rm_ConditionVarType.IsClass:
//                            var classArray = Rm_RPGHandler.Instance.Player.CharacterDefinitions;
//                            RPGMakerGUI.PopupList(ref conditionNode.StringVar, classArray, "Class");
//                            break;
//                        default:
//                            throw new ArgumentOutOfRangeException();
//                    }
//
//                    if (conditionNode.ConditionVarType == Rm_ConditionVarType.HasSkill
//                        || conditionNode.ConditionVarType == Rm_ConditionVarType.TalentEnabled
//                        || conditionNode.ConditionVarType == Rm_ConditionVarType.HasStatusEffect
//                        || conditionNode.ConditionVarType == Rm_ConditionVarType.AtOrAboveLevel )
//                    {
//                        conditionNode.AttackerSkillOrStatus = EditorGUILayout.Toggle("Attacker's Skill/Status?",
//                                                                                     conditionNode.AttackerSkillOrStatus);
//                    }
//                }
//                else
//                {
//                    conditionNode.IntVar = RPGMakerGUI.IntField("Level:", conditionNode.IntVar);
//                }
//            }
//
//            if (varNode != null)
//            {
//                var showVarInfo = true;
//                if (calcNode != null)
//                {
//                    if (calcNode.IsCombatNode)
//                    {
//                        calcNode.CalcNodeTarget = (Rm_CalcNodeTarget)EditorGUILayout.EnumPopup("Target:", calcNode.CalcNodeTarget);
//                        if (calcNode.CalcNodeTarget == Rm_CalcNodeTarget.ElementalDamage)
//                        {
//                            var elements = ASVT.ElementalDamageDefinitions.ToArray();
//                            RPGMakerGUI.PopupList(ref calcNode.CalcNodeTargetVar, elements, "Element");
//                        }
//                    }
//
//                    if (calcNode.CalcType == Rm_CalcNodeType.Square || calcNode.CalcType == Rm_CalcNodeType.SquareRoot)
//                    {
//                        showVarInfo = false;
//                    }
//                }
//
//                if (showVarInfo)
//                {
//                    if (varNode.NodeType != Rm_NodeType.Random && varNode.NodeType != Rm_NodeType.Comparison)
//                        varNode.VarType = (Rm_NodeVarType)EditorGUILayout.EnumPopup("VarType:", varNode.VarType);
//
//                    Rm_ComparisonNode comparisonNode;
//                    if (varNode.VarType == Rm_NodeVarType.Number)
//                    {
//                        varNode.NumberValue = RPGMakerGUI.FloatField("Value:", varNode.NumberValue);
//                    }
//                    else if (varNode.VarType == Rm_NodeVarType.Attribute)
//                    {
//                        var elements = ASVT.AttributesDefinitions.ToArray();
//                        RPGMakerGUI.PopupList(ref varNode.StringValue, elements, "Attribute");
//                    }
//                    else if (varNode.VarType == Rm_NodeVarType.Statistic)
//                    {
//                        var elements = ASVT.StatisticDefinitions.ToArray();
//                        RPGMakerGUI.PopupList(ref varNode.StringValue, elements, "Statistic");
//                    }
//                    else if (varNode.VarType == Rm_NodeVarType.Vital)
//                    {
//                        var elements = ASVT.VitalDefinitions.ToArray();
//                        RPGMakerGUI.PopupList(ref varNode.StringValue, elements, "Vital");
//                    }
//                    else if (varNode.VarType == Rm_NodeVarType.CustomVariable)
//                    {
//                        var customVariableArray = Rm_RPGHandler.Instance.DefinedVariables.Vars
//                            .Where(v => v.VariableType == Rmh_CustomVariableType.Float ||
//                                        v.VariableType == Rmh_CustomVariableType.Int).ToArray();
//
//                        RPGMakerGUI.PopupList(ref varNode.StringValue, customVariableArray, "Custom Var");
//
//                        if(!string.IsNullOrEmpty(varNode.StringValue))
//                        {
//                            varNode.CustomVarType =
//                                Rm_RPGHandler.Instance.DefinedVariables.Vars.First(v => v.ID == varNode.StringValue).VariableType;
//                        }
//                    }
//                    else if (varNode.VarType == Rm_NodeVarType.Trait)
//                    {
//                        var elements = ASVT.TraitDefinitions.ToArray();
//                        RPGMakerGUI.PopupList(ref varNode.StringValue, elements, "Trait");
//                    }
//                    else if (varNode.VarType == Rm_NodeVarType.ResultNode)
//                    {
//                        var listOfResultNodes = NodeBank.NodeTrees
//                            .SelectMany(n => n.Nodes)
//                            .Where(n => n.NodeType == Rm_NodeType.Result_Node).ToList();
//                        RPGMakerGUI.PopupList(ref varNode.StringValue, listOfResultNodes, "Result Node", "ID", "Identifier");
//                    }
//                    else if (varNode.VarType == Rm_NodeVarType.Random)
//                    {
//                        comparisonNode = varNode as Rm_ComparisonNode;
//                        var valueAPrefix = comparisonNode == null ? "Random Min" : "Value A";
//                        var valueBPrefix = comparisonNode == null ? "Random Max" : "Value B";
//
//                        var valueAPrefix2 = comparisonNode == null ? "Min Value" : "Value A";
//                        var valueBPrefix2 = comparisonNode == null ? "Max Value" : "Value B";
//
//                        var randomNodeTypes = Enum.GetValues(typeof(Rm_RandomVarType)) as Rm_RandomVarType[];
//                        selectedRandomMinIndex = Array.IndexOf(randomNodeTypes, varNode.ValueAType);
//                        selectedRandomMaxIndex = Array.IndexOf(randomNodeTypes, varNode.ValueBType);
//
//                        //Min
//
//                        selectedRandomMinIndex = EditorGUILayout.Popup(valueAPrefix + " Type:", selectedRandomMinIndex,
//                                                                       randomNodeTypes.Select(n => n.ToString()).
//                                                                           ToArray());
//                        varNode.ValueAType = randomNodeTypes[selectedRandomMinIndex];
//
//                        if (varNode.ValueAType == Rm_RandomVarType.Number)
//                        {
//                            varNode.ValueA = RPGMakerGUI.FloatField(valueAPrefix2, varNode.ValueA);
//                        }
//                        else if (varNode.ValueAType == Rm_RandomVarType.Attribute)
//                        {
//                            var elements = ASVT.AttributesDefinitions.ToArray();
//                            RPGMakerGUI.PopupList(ref varNode.ValueAString, elements, "- Attribute");
//                        }
//                        else if (varNode.ValueAType == Rm_RandomVarType.Statistic)
//                        {
//                            var elements = ASVT.StatisticDefinitions.ToArray();
//                            RPGMakerGUI.PopupList(ref varNode.ValueAString, elements, "- Statistic");
//                        }
//                        else if (varNode.ValueAType == Rm_RandomVarType.Vital)
//                        {
//                            var elements = ASVT.VitalDefinitions.ToArray();
//                            RPGMakerGUI.PopupList(ref varNode.ValueAString, elements, "- Vital");
//                        }
//                        else if (varNode.ValueAType == Rm_RandomVarType.CustomVariable)
//                        {
//                            var customVariableArray = Rm_RPGHandler.Instance.DefinedVariables.Vars
//                                .Where(v => v.VariableType == Rmh_CustomVariableType.Float ||
//                                            v.VariableType == Rmh_CustomVariableType.Int).ToArray();
//
//                            RPGMakerGUI.PopupList(ref varNode.ValueAString, customVariableArray, "- Custom Var");
//
//                            if (!string.IsNullOrEmpty(varNode.ValueAString))
//                            {
//                                varNode.CustomVarType =
//                                    Rm_RPGHandler.Instance.DefinedVariables.Vars.First(v => v.ID == varNode.ValueAString).VariableType;
//                            }
//                        }
//                        else if (varNode.ValueAType == Rm_RandomVarType.Trait)
//                        {
//                            var elements = ASVT.TraitDefinitions.ToArray();
//                            RPGMakerGUI.PopupList(ref varNode.ValueAString, elements, "- Trait");
//                        }
//                        else if (varNode.ValueAType == Rm_RandomVarType.ResultNode)
//                        {
//                            var listOfResultNodes = NodeBank.NodeTrees
//                                .SelectMany(n => n.Nodes)
//                                .Where(n => n.NodeType == Rm_NodeType.Result_Node).ToList();
//                            RPGMakerGUI.PopupList(ref varNode.ValueAString, listOfResultNodes, "- Result Node", "ID", "Identifier");
//                        }
//
//                        //Max
//
//                        selectedRandomMaxIndex = EditorGUILayout.Popup(valueBPrefix + " Type:", selectedRandomMaxIndex,
//                                                                       randomNodeTypes.Select(n => n.ToString()).
//                                                                           ToArray());
//                        varNode.ValueBType = randomNodeTypes[selectedRandomMaxIndex];
//
//                        if (varNode.ValueBType == Rm_RandomVarType.Number)
//                        {
//                            varNode.ValueB = RPGMakerGUI.FloatField(valueBPrefix2, varNode.ValueB);
//                        }
//                        else if (varNode.ValueBType == Rm_RandomVarType.Attribute)
//                        {
//                            var elements = ASVT.AttributesDefinitions.ToArray();
//                            RPGMakerGUI.PopupList(ref varNode.ValueBString, elements, "- Attribute");
//                        }
//                        else if (varNode.ValueBType == Rm_RandomVarType.Statistic)
//                        {
//                            var elements = ASVT.StatisticDefinitions.ToArray();
//                            RPGMakerGUI.PopupList(ref varNode.ValueBString, elements, "- Statistic");
//                        }
//                        else if (varNode.ValueBType == Rm_RandomVarType.Vital)
//                        {
//                            var elements = ASVT.VitalDefinitions.ToArray();
//                            RPGMakerGUI.PopupList(ref varNode.ValueBString, elements, "- Vital");
//                        }
//                        else if (varNode.ValueBType == Rm_RandomVarType.CustomVariable)
//                        {
//                            var customVariableArray = Rm_RPGHandler.Instance.DefinedVariables.Vars
//                                .Where(v => v.VariableType == Rmh_CustomVariableType.Float ||
//                                            v.VariableType == Rmh_CustomVariableType.Int).ToArray();
//
//                            RPGMakerGUI.PopupList(ref varNode.ValueBString, customVariableArray, "- Custom Var");
//
//                            if (!string.IsNullOrEmpty(varNode.ValueBString))
//                            {
//                                varNode.CustomVarType =
//                                    Rm_RPGHandler.Instance.DefinedVariables.Vars.First(v => v.ID == varNode.ValueBString).VariableType;
//                            }
//                        }
//                        else if (varNode.ValueBType == Rm_RandomVarType.Trait)
//                        {
//                            var elements = ASVT.TraitDefinitions.ToArray();
//                            RPGMakerGUI.PopupList(ref varNode.ValueBString, elements, "- Trait");
//                        }
//                        else if (varNode.ValueBType == Rm_RandomVarType.ResultNode)
//                        {
//                            var listOfResultNodes = NodeBank.NodeTrees
//                                .SelectMany(n => n.Nodes)
//                                .Where(n => n.NodeType == Rm_NodeType.Result_Node).ToList();
//                            RPGMakerGUI.PopupList(ref varNode.ValueBString, listOfResultNodes, "- Result Node", "ID", "Identifier");
//                        }
//
//                    }
//
//                    if (varNode.VarType == Rm_NodeVarType.Attribute || varNode.VarType == Rm_NodeVarType.Statistic ||
//                        varNode.VarType == Rm_NodeVarType.Vital ||
//                        (varNode.VarType == Rm_NodeVarType.Random && varNode.NodeType != Rm_NodeType.Comparison))
//                    {
//                        varNode.IsAttacker = EditorGUILayout.Toggle("Attacker values?", varNode.IsAttacker);
//                    }
//
//                    if (varNode.NodeType != Rm_NodeType.Comparison && varNode.VarType == Rm_NodeVarType.Random)
//                        varNode.WholeNumberResult = EditorGUILayout.Toggle("Whole Number Result?", varNode.WholeNumberResult);
//
//                    comparisonNode = varNode as Rm_ComparisonNode;
//                    if (comparisonNode != null)
//                    {
//                        comparisonNode.Comparison = (Rm_ComparisonOperator)EditorGUILayout.EnumPopup("Value A to B:", comparisonNode.Comparison);
//                        if (varNode.ValueAType == Rm_RandomVarType.Attribute || varNode.ValueAType == Rm_RandomVarType.Statistic ||
//                            varNode.ValueAType == Rm_RandomVarType.Vital)
//                            comparisonNode.ValueAIsAttacker = EditorGUILayout.Toggle("Value A is Attacker?", comparisonNode.ValueAIsAttacker);
//
//                        if (varNode.ValueBType == Rm_RandomVarType.Attribute || varNode.ValueBType == Rm_RandomVarType.Statistic ||
//                            varNode.ValueBType == Rm_RandomVarType.Vital)
//                            comparisonNode.ValueBIsAttacker = EditorGUILayout.Toggle("Value B is Attacker?", comparisonNode.ValueBIsAttacker);
//                    }
//
//                    var resultNode = SelectedNode as Rm_RandomNode;
//                    if (resultNode != null)
//                    {
//                        resultNode.GreaterThanValue = resultNode.WholeNumberResult
//                                                          ? RPGMakerGUI.IntField("True when greater than:", (int)resultNode.GreaterThanValue)
//                                                          : RPGMakerGUI.FloatField("True when greater than:", resultNode.GreaterThanValue);
//
//                        resultNode.OrEqualsTo = EditorGUILayout.Toggle("Allow Equals To?", resultNode.OrEqualsTo);
//                    }
//                }
//            }
//        }
//
//        private void OnGUIx()
//        {
//            GUI.skin = null;
//            GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;
//
//            //
//            if (DrawGrid)
//            {
//                Rme_NodeWindowDraw.DrawNodeGrid(position, 25, Color.gray);
//            }
//
//            if (SelectedNode != null)
//            {
//                Rme_NodeWindowDraw.DrawLines(SelectedNodeTree,Nodes,nodeScrollPos,position);
//            }
//            
//            if(Window == null)
//            {
//                switch(NodeBank.Type)
//                {
//                    case NodeTreeType.Combat:
//                        Rme_Node_CombatCalc.Init();
//                        break;
//                    case NodeTreeType.Dialog:
//                        Rme_Node_Dialog.Init();
//                        break;
//                    case NodeTreeType.Event:
//                        Rme_Node_Events.Init();
//                        break;
//                    case NodeTreeType.WorldMap:
//                        Rme_Node_WorldMap.Init();
//                        break;
//                    default:
//                        throw new ArgumentOutOfRangeException();
//                }
//
//            }
//
//            GUILayout.BeginHorizontal();
//            GUILayout.BeginVertical("Box", GUILayout.Width(Window.position.width - 350), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
//            GUILayout.EndVertical();
//            GUILayout.BeginVertical("Box", GUILayout.Width(350), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
//            GUILayout.BeginHorizontal(GUILayout.Height(25));
//
//            if (SelectedNode != null)
//            {
//                if (GUILayout.Button("DEL"))
//                {
//                    if (SelectedNode.NodeType != Rm_NodeType.Start_Combat &&
//                        SelectedNode.NodeType != Rm_NodeType.End_Combat &&
//                        SelectedNode.NodeType != Rm_NodeType.Start_Dealt_Damage &&
//                        SelectedNode.NodeType != Rm_NodeType.End_Dialog &&
//                        SelectedNode.NodeType != Rm_NodeType.End_Dealt_Damage &&
//                        SelectedNode.NodeType != Rm_NodeType.Start_Event &&
//                        SelectedNode.NodeType != Rm_NodeType.Start_Dialog)
//                    {
//                        if (SelectedNode.NodeLinkingToThis != null)
//                            SelectedNode.NodeLinkingToThis.NextPoints.First(n => n.LinkedNode == SelectedNode).
//                                LinkedNode = null;
//
//                        var linksMade = SelectedNode.NextPoints.Where(n => n.LinkedNode != null).ToList();
//                        linksMade.ForEach(n => n.LinkedNode.NodeLinkingToThis = null);
//
//                        Nodes.Remove(SelectedNode);
//                        SelectedNode = null;
//                        return;
//                    }
//                }
//            }
//            GUILayout.FlexibleSpace();
//            if(GUILayout.Button("?"))
//            {
//                SelectedNode = null;
//            }
//            if(GUILayout.Button("GRID"))
//            {
//                DrawGrid = !DrawGrid;
//            }
//            if(GUILayout.Button("SNAP"))
//            {
//                SnapWindows = !SnapWindows;
//            }
//            GUILayout.EndHorizontal();
//
//            
//            if(SelectedNode != null)
//            {
//                GUILayout.Box("Node Settings");
//                NodeSettings();
//            }
//            else
//            {
//                EditorGUILayout.LabelField("Help:");
//                var r = GUILayoutUtility.GetLastRect();
//                Drawing.DrawLine(new Vector2(r.x,r.y + 20),new Vector2(r.xMax, r.y + 20 ),Color.black,2,false);
//                GUILayout.Space(5);
//                EditorGUILayout.LabelField("Node Trees:");
//                EditorGUILayout.HelpBox("Node trees can be added at the bottom of the window.", MessageType.Info);
//
//                EditorGUILayout.LabelField("Adding Nodes:");
//                EditorGUILayout.HelpBox("Once you have selected a node tree, right click the scroll-view area to add a new node.", MessageType.Info);
//            }
//
//            GUILayout.FlexibleSpace();
//
//            var oldSelTreeIndex = SelectedNodeTreeIndex;
//            SelectedNodeTreeIndex = EditorGUILayout.Popup("NodeTree:", SelectedNodeTreeIndex,
//                                                          NodeBank.NodeTrees.Select((q,indexOf) => indexOf + ". " +  q.Name).ToArray());
//            if(NodeBank.NodeTrees.Count > 0)
//            {
//                SelectedNodeTree =
//                    NodeBank.NodeTrees.ToArray()[SelectedNodeTreeIndex];
//            }
//        
//
//            if (oldSelTreeIndex != SelectedNodeTreeIndex)
//            {
//                SelectedNode = null;
//                return;
//            }
//
//            if(SelectedNodeTree != null)
//            {
//                if (PrimaryTrees.All(p => SelectedNodeTree.Name != p))
//                    SelectedNodeTree.Name = RPGMakerGUI.TextField("Edit Name:", SelectedNodeTree.Name);
//            }
//        
//
//            GUILayout.BeginHorizontal(GUILayout.Height(25));
//            if (GUILayout.Button("ADD NODE TREE"))
//            {
//                var newTree = GetNewTree(NodeBank.Type);
//                NodeBank.NodeTrees.Add(newTree);
//                SelectedNodeTreeIndex = NodeBank.NodeTrees.IndexOf(newTree);
//                SelectedNodeTree = newTree;
//                SelectedNode = null;
//                return;
//            }
//            if (GUILayout.Button("DEL TREE"))
//            {
//                if (PrimaryTrees.All(p => SelectedNodeTree.Name != p))
//                {
//                    NodeBank.NodeTrees.Remove(SelectedNodeTree);
//
//                    if(PrimaryTrees.Any()){
//                        SelectedNodeTree = NodeBank.NodeTrees.First(n => n.Name == PrimaryTrees[0]);
//                        SelectedNode = null;
//                        SelectedNodeTreeIndex = NodeBank.NodeTrees.IndexOf(SelectedNodeTree);
//                    }
//                    else
//                    {
//                        SelectedNodeTree = null;
//                        SelectedNode = null;
//                        SelectedNodeTreeIndex = 0;
//                    }
//                    return;
//                }
//            }
//            if (GUILayout.Button("Save Data"))
//            {
//                EditorGameDataSaveLoad.SaveGameData();
//            }
//            //todo: remove this/upgrade to better
//            if (GUILayout.Button("TEST"))
//            {
//                TestPath();
//            }
//            GUILayout.EndHorizontal();
//            GUILayout.EndVertical();
//            GUILayout.EndHorizontal();
//
//            var scrollViewRect = new Rect(0, 0, position.width - 350, position.height);
//            var evt = Event.current;
//            if (evt.type == EventType.ContextClick)
//            {
//                var mousePos = evt.mousePosition;
//                var adjustedRect = scrollViewRect;
//
//                if (adjustedRect.Contains(mousePos) && SelectedNodeTree != null)
//                {
//                    AddContextMenu(evt, mousePos + nodeScrollPos);
//                }
//            }
//            if(SelectedNodeTree != null)
//                GUI.Label(new Rect(5,5,500,100), SelectedNodeTree.Name, new GUIStyle() { fontSize = 32 });
//
//            nodeScrollPos = GUI.BeginScrollView(scrollViewRect,
//                                                nodeScrollPos,
//                                                new Rect(0, 0, 2500, 2500), false, false);
//
//
//            if(SelectedNodeTree != null)
//            {
//                BeginWindows();
//
//                for (int index = 0; index < Nodes.Count; index++)
//                {
//                    var node = Nodes[index];
//                    node.Rect = GUILayout.Window(node.WindowID, node.Rect, NodeWindow, "", "nodeStartWindow");
//                }
//
//                EndWindows();
//            }
//        
//            GUI.EndScrollView();
//        }
//
//        private void TestPath()
//        {
//            throw new NotImplementedException();
//        }
//
//        public static Rm_NodeTree GetNewTree(NodeTreeType NodeType)
//        {
//            int newCount;
//            NodeBank nodeBank;
//            switch (NodeType)
//            {
//                case NodeTreeType.Combat:
//                    newCount = Rm_RPGHandler.Instance.Nodes.CombatNodeBank.NodeTrees.Count;
//                    nodeBank = Rm_RPGHandler.Instance.Nodes.CombatNodeBank;
//                    break;
//                case NodeTreeType.Dialog:
//                    newCount = Rm_RPGHandler.Instance.Nodes.DialogNodeBank.NodeTrees.Count;
//                    nodeBank = Rm_RPGHandler.Instance.Nodes.DialogNodeBank;
//                    break;
//                case NodeTreeType.Event:
//                    newCount = Rm_RPGHandler.Instance.Nodes.EventNodeBank.NodeTrees.Count;
//                    nodeBank = Rm_RPGHandler.Instance.Nodes.EventNodeBank;
//                    break;
//                case NodeTreeType.WorldMap:
//                    throw new NotImplementedException();
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//
//            var tree = new Rm_NodeTree { Name = "New Tree" + newCount };
//            switch (NodeType)
//            {
//                case NodeTreeType.Combat:
//                    tree.AddStartEndNode(Rm_NodeType.Start_Combat_Var, Vector2.zero, GetNextID(nodeBank));
//                    tree.AddStartEndNode(Rm_NodeType.Result_Node, Vector2.zero, (GetNextID(nodeBank) + 1));
//                    break;
//                case NodeTreeType.Dialog:
//                    tree.AddStartEndNode(Rm_NodeType.Start_Dialog, Vector2.zero, GetNextID(nodeBank));
//                    tree.AddStartEndNode(Rm_NodeType.End_Dialog, Vector2.zero, (GetNextID(nodeBank) + 1));
//                    break;
//                case NodeTreeType.Event:
//                    tree.AddStartEndNode(Rm_NodeType.Start_Event, Vector2.zero, GetNextID(nodeBank));
//                    break;
//                case NodeTreeType.WorldMap:
//                    throw new NotImplementedException();
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//
//            return tree;
//        }
//
//        public static int GetNextID(NodeBank nodeBank)
//        {
//            return nodeBank.NodeTrees.SelectMany(n => n.Nodes).Any()
//                       ? (nodeBank.NodeTrees.SelectMany(n => n.Nodes).Max(n => n.WindowID) + 1)
//                       : 0;
//        }
//
//        private void NodeWindow(int id)
//        {
//         
//            var node = Nodes.First(n => n.WindowID == id);
//
//            //Set Correct Height
//            var isStartCombatEventHeight = node.NodeType == Rm_NodeType.Start_Combat ||
//                                           node.NodeType == Rm_NodeType.Start_Event ||
//                                           node.NodeType == Rm_NodeType.Start_Dialog ? 0 : 25;
//            var height = 25 + isStartCombatEventHeight + (node.NextPoints.Count * 25);
//            node.Rect.height = height;
//
//            
//
//            if ((Event.current.button == 0) && (Event.current.type == EventType.mouseUp))
//            {
//                if (NodeLinkFrom != null)
//                {
//                    NodeLinkTo = node;
//                }
//
//                SelectedNode = node;
//                GUI.FocusControl("");
//            }
//            GUILayout.BeginVertical();
//            GUILayout.FlexibleSpace();
//            if (node.NodeType != Rm_NodeType.Start_Combat && 
//                node.NodeType != Rm_NodeType.Start_Event &&
//                node.NodeType != Rm_NodeType.Start_Dialog && 
//                node.NodeType != Rm_NodeType.Start_Dealt_Damage)
//            {
//                GUILayout.Box(GetNodeInfo(node));
//            }
//
//            foreach(var link in node.NextPoints)
//            {
//                GUILayout.BeginHorizontal(GUILayout.Height(25));
//            
//                if(link.ParentNode.NodeType == Rm_NodeType.Start_Combat)
//                {
//                    GUILayout.Box("Starting Damage", GUILayout.Height(25));
//                    GUILayout.Box(" ", GUILayout.Width(25), GUILayout.Height(25));
//                }
//                else if (link.ParentNode.NodeType == Rm_NodeType.Start_Event)
//                {
//                    GUILayout.Box("Event Start", GUILayout.Height(25));
//                    GUILayout.Box(" ", GUILayout.Width(25), GUILayout.Height(25));
//                }
//                else if (link.ParentNode.NodeType == Rm_NodeType.Start_Dealt_Damage)
//                {
//                    var eleDamage = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(e => e.ID == node.Identifier);
//                    var damageName = eleDamage == null ? "<color=red>NULL</color>" : eleDamage.Name;
//                    if (node.Identifier == "Physical") damageName = "Physical";
//                    GUILayout.Box("Start of" + damageName + " Damage", GUILayout.Height(25));
//                    GUILayout.Box(" ", GUILayout.Width(25), GUILayout.Height(25));
//                }
//                else if (link.ParentNode.NodeType == Rm_NodeType.Start_Dialog)
//                {
//                    GUILayout.Box("Dialog Start", GUILayout.Height(25));
//                    GUILayout.Box(" ", GUILayout.Width(25), GUILayout.Height(25));
//                }
//                else
//                {
//                    GUILayout.Box(GetLinkInfo(link));
//                    GUILayout.Box(" ", GUILayout.Width(25), GUILayout.Height(25));
//                }
//
//                if (Event.current.type == EventType.Repaint)
//                {
//                    link.Rect = GUILayoutUtility.GetLastRect();
//                }
//
//                if ((Event.current.button == 0) && (Event.current.type == EventType.mouseUp))
//                {
//                    if(link.Rect.Contains(Event.current.mousePosition))
//                    {
//                        if(NodeLinkFrom == null)
//                        {
//                            NodeLinkFrom = link;
//                        }
//                    }
//                }
//                else if ((Event.current.button == 1) && (Event.current.type == EventType.mouseUp))
//                {
//                    if (link.Rect.Contains(Event.current.mousePosition))
//                    {
//                        if(link.LinkedNode != null)
//                        {
//                            link.LinkedNode.NodeLinkingToThis = null;
//                            link.LinkedNode = null;
//                        }
//                    }
//                }
//
//                GUILayout.EndHorizontal();
//            }
//            GUILayout.FlexibleSpace();
//            GUILayout.EndVertical();
//            GUI.DragWindow();
//            
//            if(SnapWindows)
//                RoundWindow(ref node.Rect);
//
//            ClampWindow(ref node.Rect);
//        }
//
//        private void RoundWindow(ref Rect rect)
//        {
//            rect.x = rect.x.RoundToNearest(25);
//            rect.y = rect.y.RoundToNearest(25);
//        }
//
//        private void ClampWindow(ref Rect rect)
//        {
//            rect.x = Mathf.Clamp(rect.x, 0, 2500 - rect.width);
//            rect.y = Mathf.Clamp(rect.y, 0, 2500 - rect.height);
//        }
//
//        private string GetNodeInfo(Rm_Node node)
//        {
//            if (node.NodeType == Rm_NodeType.Start_Combat_Var)
//            {
//                return "Starting Variable";
//            }
//            if (node.NodeType == Rm_NodeType.NPC_Response || node.NodeType == Rm_NodeType.Player_Response)
//            {
//                return (node.NodeType == Rm_NodeType.NPC_Response ? "NPC " : "Player ") + "Response";
//            }
//            if (node.NodeType == Rm_NodeType.End_Combat)
//            {
//                return "Damage Recieved";
//            }
//            if (node.NodeType == Rm_NodeType.MinMaxNode)
//            {
//                return "Min Max Damage";
//            }
//            if (node.NodeType == Rm_NodeType.IsPlayerNode)
//            {
//                var info = "Is Player";
//                if(SelectedNodeTree.Name != "Core_DamageDealt")
//                {
//                    var isAttacker = (node as Rm_IsPlayerNode).IsAttacker;
//                    info = (isAttacker ? "[Attacker] " : "[Defender] ") + info;
//                }
//                return info;
//            }
//            if (node.NodeType == Rm_NodeType.End_Dialog)
//            {
//                return "End Dialog";
//            }
//            if (node.NodeType == Rm_NodeType.End_Dealt_Damage)
//            {
//                var eleDamage = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(e => e.ID ==  node.Identifier);
//                var damageName = eleDamage == null ? "<color=red>NULL</color>" : eleDamage.Name;
//                if (node.Identifier == "Physical") damageName = "Physical";
//                var linkInfo = "Dealt " + damageName + " Damage";
//                return linkInfo;
//            }
//            if (node.NodeType == Rm_NodeType.Start_Crit_Chance)
//            {
//                return "Start Crit Chance";
//            }
//            if (node.NodeType == Rm_NodeType.End_Crit_Chance)
//            {
//                return "End Crit Chance";
//            }
//            if (node.NodeType == Rm_NodeType.End_Combat_Evade)
//            {
//                return "Attack Evaded";
//            }
//            if (node.NodeType == Rm_NodeType.End_Combat_Miss)
//            {
//                return "Attack Missed";
//            }
//            if (node.NodeType == Rm_NodeType.End_Dialog_Vendor)
//            {
//                return "Open Vendor";
//            }
//            if (node.NodeType == Rm_NodeType.End_Dialog_Crafting)
//            {
//                return "Open Crafting";
//            }
//            if (node.NodeType == Rm_NodeType.Begin_Quest)
//            {
//                return "Begin Quest";
//            }
//            if (node.NodeType == Rm_NodeType.Complete_Quest)
//            {
//                return "Complete Quest";
//            }
//            if (node.NodeType == Rm_NodeType.Result_Node)
//            {
//                return node.Identifier;
//            }
//            if (node.NodeType == Rm_NodeType.Random)
//            {
//                var randomNode = node as Rm_RandomNode;
//                var equalsTo = randomNode.OrEqualsTo ? "=" : "";
//                var value = randomNode.WholeNumberResult ? (int)randomNode.GreaterThanValue : randomNode.GreaterThanValue;
//                return "[RandomValue] >" + equalsTo + " " + value;
//            }
//            if (node.NodeType == Rm_NodeType.Condition)
//            {
//                var conditionNode = node as Rm_ConditionNode;
//                var info = conditionNode.ConditionVarType == Rm_ConditionVarType.AtOrAboveLevel
//                               ? conditionNode.IntVar.ToString()
//                               : conditionNode.StringVar;
//                return "If (" + conditionNode.ConditionVarType + " [" + info + "] )";
//            }
//            if (node.NodeType == Rm_NodeType.Comparison)
//            {
//                var randomNode = node as Rm_ComparisonNode;
//                string comparisonSymbol;
//                switch (randomNode.Comparison)
//                {
//                    case Rm_ComparisonOperator.Equals:
//                        comparisonSymbol = "==";
//                        break;
//                    case Rm_ComparisonOperator.NotEqualTo:
//                        comparisonSymbol = "!=";
//                        break;
//                    case Rm_ComparisonOperator.GreaterThan:
//                        comparisonSymbol = ">";
//                        break;
//                    case Rm_ComparisonOperator.GreaterThanOrEqualTo:
//                        comparisonSymbol = ">=";
//                        break;
//                    case Rm_ComparisonOperator.LessThan:
//                        comparisonSymbol = "<";
//                        break;
//                    case Rm_ComparisonOperator.LessThanOrEqualTo:
//                        comparisonSymbol = "<=";
//                        break;
//                    default:
//                        throw new ArgumentOutOfRangeException();
//                }
//                return "[ValueA] " + comparisonSymbol + " [ValueB] ";
//            }
//
//            if (node.NodeType == Rm_NodeType.Calculate)
//            {
//                var calcNode = node as Rm_CalcNode;
//                var s = calcNode.CalcType.ToString();
//
//                if (calcNode.CalcType == Rm_CalcNodeType.Add)
//                    s += " to";
//                if (calcNode.CalcType == Rm_CalcNodeType.Subtract)
//                    s += " from";
//
//                s += calcNode.IsCombatNode ? string.Format(" [{0}]",calcNode.CalcNodeTarget) : " [Value]";
//
//                if (calcNode.CalcType == Rm_CalcNodeType.Multiply ||
//                    calcNode.CalcType == Rm_CalcNodeType.Divide)
//                    s += " by";
//
//                return s;
//            }
//
//            if (node.NodeType == Rm_NodeType.Boolean)
//            {
//                var boolNode = node as Rm_BooleanNode;
//                var boolId = !String.IsNullOrEmpty(boolNode.BoolIdentifier) ? boolNode.BoolIdentifier : null;
//                if (boolId == null)
//                    return "<color=red>Null</color>";
//
//                if (boolNode.BoolType == Rm_BooleanNodeType.ResultNode) return "[" + boolNode.BoolIdentifier + "]";
//
//                var str = "[" + boolId + "]";
//                switch (boolNode.CustomVarType)
//                {
//                    case Rmh_CustomVariableType.Float:
//                        str += " == " + boolNode.EqualsFloat;
//                        break;
//                    case Rmh_CustomVariableType.Int:
//                        str += " == " + boolNode.EqualsInt;
//                        break;
//                    case Rmh_CustomVariableType.Bool:
//                        break;
//                    case Rmh_CustomVariableType.String:
//                        str += " == " + "\"" + boolNode.EqualsString + "\"";
//                        break;
//                    default:
//                        throw new ArgumentOutOfRangeException();
//                }
//
//                return str;
//            }
//
//            if (node.NodeType == Rm_NodeType.Run_Event)
//            {
//                var eventNode = node as Rm_RunEventNode;
//                var eventInfo = !String.IsNullOrEmpty(eventNode.EventName) ? eventNode.EventName : null;
//                if (eventInfo == null)
//                    eventInfo = "<color=red>Null</color>";
//
//                return "Play Event [" + eventInfo + "]";
//            }
//
//            if(node.NodeType == Rm_NodeType.VarSetter)
//            {
//                var varNode = node as Rm_VarSetterNode;
//                var varId = !String.IsNullOrEmpty(varNode.CustomVarName) ? varNode.CustomVarName : null;
//                if (varId == null)
//                    return "<color=red>Null</color>";
//
//                var str = "Set [" + varId + "] to ";
//                if(varNode.SetToType == Rm_VarSetterType.Value)
//                {
//                    switch (varNode.CustomVarType)
//                    {
//                        case Rmh_CustomVariableType.Float:
//                            str += varNode.SetToFloat;
//                            break;
//                        case Rmh_CustomVariableType.Int:
//                            str += varNode.SetToInt;
//                            break;
//                        case Rmh_CustomVariableType.Bool:
//                            str += varNode.SetToBool;
//                            break;
//                        case Rmh_CustomVariableType.String:
//                            str += "\"" + varNode.SetToString + "\"";
//                            break;
//                        default:
//                            throw new ArgumentOutOfRangeException();
//                    }
//                }
//                else
//                {
//                    str += "[" + varNode.SetToResultNodeName + "]";
//                }
//            
//                return str;
//            }
//
//            return "Node";
//        }
//
//        private string GetLinkInfo(Rm_NodeLink link)
//        {
//            if(!string.IsNullOrEmpty(link.Label))
//            {
//                return link.Label;
//            }
//
//            var varNode = link.ParentNode as Rm_VariableNode;
//
//            var calcNode = link.ParentNode as Rm_CalcNode;
//            if(calcNode != null)
//            {
//                if (calcNode.CalcType == Rm_CalcNodeType.Square || calcNode.CalcType == Rm_CalcNodeType.Square)
//                {
//                    return "";
//                }
//            }
//
//            if (varNode != null)
//            {
//                var isAttacker = varNode.IsAttacker ? " of Attacker" : " of Defender";
//                if (varNode.VarType == Rm_NodeVarType.Number)
//                {
//                    return "Number:" + varNode.NumberValue;
//                }
//            
//                if (varNode.VarType == Rm_NodeVarType.Random)
//                {
//                    return "[Random Value]";
//                }
//
//                if(varNode.VarType == Rm_NodeVarType.Attribute)
//                {
//                    var text = ASVT.AttributesDefinitions.FirstOrDefault(c => c.ID == varNode.StringValue) != null
//                        ? ASVT.AttributesDefinitions.FirstOrDefault(c => c.ID == varNode.StringValue).Name
//                        : "<color=red>Null</color>";
//                    return "Total [" + text + "]" + isAttacker; 
//                }
//
//                if(varNode.VarType == Rm_NodeVarType.Statistic)
//                {
//                    
//                    var text = ASVT.StatisticDefinitions.FirstOrDefault(c => c.ID == varNode.StringValue) != null
//                        ? ASVT.StatisticDefinitions.FirstOrDefault(c => c.ID == varNode.StringValue).Name
//                        : "<color=red>Null</color>" ;
//                    return "Total [" + text + "]" + isAttacker; 
//                }
//
//                if(varNode.VarType == Rm_NodeVarType.Vital)
//                {
//                    var text = ASVT.VitalDefinitions.FirstOrDefault(c => c.ID == varNode.StringValue) != null
//                        ? ASVT.VitalDefinitions.FirstOrDefault(c => c.ID == varNode.StringValue).Name
//                        : "<color=red>Null</color>";
//                    return "Current [" + text + "]" + isAttacker;  
//                }
//
//                if(varNode.VarType == Rm_NodeVarType.Trait)
//                {
//                    var text = ASVT.TraitDefinitions.FirstOrDefault(c => c.ID == varNode.StringValue) != null
//                        ? ASVT.TraitDefinitions.FirstOrDefault(c => c.ID == varNode.StringValue).Name
//                        : "<color=red>Null</color>";
//                    return "[" + text + "] Level"; 
//                }
//
//                if(varNode.VarType == Rm_NodeVarType.ResultNode)
//                {
//                    var text = GetResultNodes(false).Concat(GetResultNodes(true)).FirstOrDefault(c => c.ID == varNode.StringValue) != null
//                        ? GetResultNodes(false).Concat(GetResultNodes(true)).FirstOrDefault(c => c.ID == varNode.StringValue).Identifier
//                        : "<color=red>Null</color>";
//                    return "Value of [" + text + "]";
//                }
//
//                if(varNode.VarType == Rm_NodeVarType.CustomVariable)
//                {
//                    var text = Rm_RPGHandler.Instance.DefinedVariables.Vars.FirstOrDefault(c => c.ID == varNode.StringValue) != null
//                        ? Rm_RPGHandler.Instance.DefinedVariables.Vars.FirstOrDefault(c => c.ID == varNode.StringValue).Name
//                        : "<color=red>Null</color>";
//                    return "Value of [" + text + "]"; 
//                }
//            }
//
//            return "";
//        }
//
//        public GenericMenu.MenuFunction2 AddNode()
//        {
//            return AddNode;
//        }
//
//        public void AddNode(object userData)
//        {
//            var data = (object[])userData;
//
//            var nodeType = (Rm_NodeType)data[0];
//            var mousePos = (Vector2)data[1];
//
//            Rm_Node node = null;
//
//            if (nodeType == Rm_NodeType.Calculate)
//            {
//                node = new Rm_CalcNode();
//                var calcNode = node as Rm_CalcNode;
//                calcNode.CalcNodeTarget = PrimaryTrees.All(p => SelectedNodeTree.Name != p) ? Rm_CalcNodeTarget.Value : Rm_CalcNodeTarget.AllDamage;
//                calcNode.IsCombatNode = PrimaryTrees.Any(p => SelectedNodeTree.Name == p);
//            }
//            else if (nodeType == Rm_NodeType.NPC_Response)
//            {
//                node = new Rm_ResponseNode(true);
//                node.NextPoints = new List<Rm_NodeLink>
//                                      {
//                                          new Rm_NodeLink(node) {Label = ">"}
//                                      };
//            }
//            else if (nodeType == Rm_NodeType.Player_Response)
//            {
//                node = new Rm_ResponseNode(false);
//                node.NextPoints = new List<Rm_NodeLink>
//                                      {
//                                          new Rm_NodeLink(node) {Label = ">"}
//                                      };
//            }
//            else if (nodeType == Rm_NodeType.MinMaxNode)
//            {
//                node = new Rm_Node();
//                node.NodeType = Rm_NodeType.MinMaxNode;
//                node.NextPoints = new List<Rm_NodeLink>
//                                      {
//                                          new Rm_NodeLink(node) {Label = "Min"},
//                                          new Rm_NodeLink(node) {Label = "Max"}
//                                      };
//            }
//            else if (nodeType == Rm_NodeType.IsPlayerNode)
//            {
//                node = new Rm_IsPlayerNode();
//                node.NodeType = Rm_NodeType.IsPlayerNode;
//                node.NextPoints = new List<Rm_NodeLink>
//                                      {
//                                          new Rm_NodeLink(node) {Label = "True"},
//                                          new Rm_NodeLink(node) {Label = "False"}
//                                      };
//            }
//            else if (nodeType == Rm_NodeType.Random)
//            {
//                node = new Rm_RandomNode();
//                node.NextPoints = new List<Rm_NodeLink>
//                                      {
//                                          new Rm_NodeLink(node) {Label = "True"}, 
//                                          new Rm_NodeLink(node) {Label = "False"}
//                                      };
//            }
//            else if (nodeType == Rm_NodeType.Run_Event)
//            {
//                node = new Rm_RunEventNode();
//                node.NextPoints = new List<Rm_NodeLink>
//                                      {
//                                          new Rm_NodeLink(node) {Label = "Next"}
//                                      };
//            }
//            else if (nodeType == Rm_NodeType.VarSetter)
//            {
//                node = new Rm_VarSetterNode();
//                node.NextPoints = new List<Rm_NodeLink>
//                                      {
//                                          new Rm_NodeLink(node) {Label = "Next"}
//                                      };
//            }
//            else if (nodeType == Rm_NodeType.Boolean)
//            {
//                node = new Rm_BooleanNode();
//                node.NextPoints = new List<Rm_NodeLink>
//                                      {
//                                          new Rm_NodeLink(node) {Label = "True"}, 
//                                          new Rm_NodeLink(node) {Label = "False"}
//                                      };
//            }
//            else if (nodeType == Rm_NodeType.Condition)
//            {
//                node = new Rm_ConditionNode();
//                node.NextPoints = new List<Rm_NodeLink>
//                                      {
//                                          new Rm_NodeLink(node) {Label = "True"}, 
//                                          new Rm_NodeLink(node) {Label = "False"}
//                                      };
//            }
//            else if (nodeType == Rm_NodeType.Start_Combat_Var || nodeType == Rm_NodeType.Result_Node)
//            {
//                SelectedNodeTree.AddStartEndNode(nodeType, mousePos, GetNextID(NodeBank));
//            }
//            else if (nodeType == Rm_NodeType.Comparison)
//            {
//                node = new Rm_ComparisonNode();
//                node.NextPoints = new List<Rm_NodeLink>
//                                      {
//                                          new Rm_NodeLink(node) {Label = "True"}, 
//                                          new Rm_NodeLink(node) {Label = "False"}
//                                      };
//            }
//            else if (nodeType == Rm_NodeType.End_Dialog_Crafting)
//            {
//                node = new Rm_Node {NodeType = Rm_NodeType.End_Dialog_Crafting, NextPoints = new List<Rm_NodeLink>()};
//            }
//            else if (nodeType == Rm_NodeType.End_Dialog_Vendor)
//            {
//                node = new Rm_Node {NodeType = Rm_NodeType.End_Dialog_Vendor, NextPoints = new List<Rm_NodeLink>()};
//            }
//            else if (nodeType == Rm_NodeType.Begin_Quest)
//            {
//                node = new Rm_Node {NodeType = Rm_NodeType.Begin_Quest};
//                node.NextPoints = new List<Rm_NodeLink>
//                                      {
//                                          new Rm_NodeLink(node) {Label = ">"}
//                                      };
//            }
//            else if (nodeType == Rm_NodeType.Complete_Quest)
//            {
//                node = new Rm_Node {NodeType = Rm_NodeType.Complete_Quest};
//                node.NextPoints = new List<Rm_NodeLink>
//                                      {
//                                          new Rm_NodeLink(node) {Label = ">"}
//                                      };
//            }
//
//            if (node != null)
//            {
//                node.WindowID = GetNextID(NodeBank);
//                if (mousePos != Vector2.zero)
//                {
//                    node.Rect.center = mousePos;
//                }
//                Nodes.Add(node);
//            }
//        }
//    }
//}