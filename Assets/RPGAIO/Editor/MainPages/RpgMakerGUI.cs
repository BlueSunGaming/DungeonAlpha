#pragma warning disable 0168

using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEditor;
using UnityEngine;
using Material = UnityEngine.Material;
using Object = UnityEngine.Object;

namespace LogicSpawn.RPGMaker.Editor
{
    public class RPGMakerGUI
    {
        private static GUIStyle richTextAreaStyle;
        public static ShaderLerpInfo LastSelectedShaderInfo;
        public static void IndentAndPrefix(int indent, string prefix)
        {
            EditorGUI.indentLevel = 2 * indent;
            EditorGUILayout.BeginHorizontal();
            if(!string.IsNullOrEmpty(prefix))
            {
                if (EditorGUI.indentLevel > 0)
                {
                    prefix = "- " + prefix;
                }
                EditorGUILayout.LabelField(prefix, GUILayout.Width(210 + (9 * EditorGUI.indentLevel)));
            }
        }
        
        public static void EndIndent()
        {
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = 0;
        }

        public static string TextField(string prefix, string stringToSet, params GUILayoutOption[] layout)
        {
            return TextField(prefix, stringToSet, 0, layout);
        }
        public static string TextField(string prefix, string stringToSet, int indent, params GUILayoutOption[] layout)
        {
            IndentAndPrefix(indent,prefix);
            stringToSet = EditorGUILayout.TextField(stringToSet, layout);
            EndIndent();

            return stringToSet;
        }

        public static RPGVector3 Vector3Field(string prefix, RPGVector3 rpgVector3, int indent, params GUILayoutOption[] layout)
        {
            IndentAndPrefix(indent,prefix);
            GUILayout.Label("X:");
            rpgVector3.X = EditorGUILayout.FloatField(rpgVector3.X, layout);
            GUILayout.Label("Y:");
            rpgVector3.Y = EditorGUILayout.FloatField(rpgVector3.Y, layout);
            GUILayout.Label("Z:");
            rpgVector3.Z = EditorGUILayout.FloatField(rpgVector3.Z, layout);
            EndIndent();

            return rpgVector3;
        }

        public static RPGVector2 Vector2Field(string prefix, RPGVector2 rpgVector2, int indent, params GUILayoutOption[] layout)
        {
            IndentAndPrefix(indent,prefix);
            GUILayout.Label("X:");
            rpgVector2.X = EditorGUILayout.FloatField(rpgVector2.X, layout);
            GUILayout.Label("Y:");
            rpgVector2.Y = EditorGUILayout.FloatField(rpgVector2.Y, layout);
            EndIndent();

            return rpgVector2;
        }

        public static string TextArea(string prefix, string stringToSet, float height = 200, int indent = 0)
        {
            IndentAndPrefix(indent,prefix);
            stringToSet = EditorGUILayout.TextArea(stringToSet, "textArea", GUILayout.Height(height));
            EndIndent();

            return stringToSet;
        }

        public static string RichTextArea(string prefix, string stringToSet, float height = 200, int indent = 0)
        {
            if(richTextAreaStyle == null)
            {
                richTextAreaStyle = new GUIStyle(GUI.skin.GetStyle("textArea")) {richText = true};
            }
            IndentAndPrefix(indent,prefix);
            stringToSet = EditorGUILayout.TextArea(stringToSet, richTextAreaStyle, GUILayout.Height(height));
            EndIndent();

            return stringToSet;
        }

        public static int IntField(string prefix, int intToSet, params GUILayoutOption[] layouts)
        {
            return IntField(prefix, intToSet, 0, layouts);
        }
        public static int IntField(string prefix, int intToSet, int indent, params GUILayoutOption[] layouts)
        {
            IndentAndPrefix(indent, prefix);
            intToSet = EditorGUILayout.IntField(intToSet,layouts);
            EndIndent();

            return intToSet;
        }

        public static float FloatField(string prefix, float floatToSet, params GUILayoutOption[] layouts)
        {
            return FloatField(prefix, floatToSet, 0, layouts);
        }
        public static float FloatField(string prefix, float floatToSet, int indent, params GUILayoutOption[] layouts)
        {
            IndentAndPrefix(indent, prefix);
            floatToSet = EditorGUILayout.FloatField(floatToSet, layouts);
            EndIndent();

            return floatToSet;
        }
                                    
        public static bool Toggle(bool boolean, params GUILayoutOption[] layout)
        {
            return Toggle("", 0, ref boolean, layout);
        }
        public static bool Toggle(string prefix, bool boolean, params GUILayoutOption[] layout)
        {
            return Toggle(prefix, 0, ref boolean, layout);
        }
        public static bool Toggle(string prefix, ref bool boolean, params GUILayoutOption[] layout)
        {
            return Toggle(prefix, 0, ref boolean, layout);
        }
        public static bool Toggle(string prefix, int indent, ref bool boolean, params GUILayoutOption[] layout)
        {
            IndentAndPrefix(indent, prefix);
            boolean = EditorGUILayout.Toggle(boolean,layout);
            EndIndent();

            return boolean;
        }

        public static Enum EnumPopup(string prefix, Enum selected, params GUILayoutOption[] layout)
        {
            return EnumPopup(prefix, 0, selected, layout);
        }

        public static Enum EnumPopup(Enum selected, params GUILayoutOption[] layout)
        {
            return EnumPopup("", 0, selected, layout);
        }

        public static Enum EnumPopup(string prefix, int indent, Enum selected, params GUILayoutOption[] layout)
        {
            IndentAndPrefix(indent, prefix);
            selected = EditorGUILayout.EnumPopup(selected, layout);
            EndIndent();

            return selected;
        }

        public static void Label(string text, string style, params GUILayoutOption[] layouts)
        {
            if(EditorGUI.indentLevel > 0)
            {
                text = "- " + text;
            }

            if (layouts.Length > 0)
            {
                GUILayout.Label(text,style, layouts);
            }
            else
            {
                GUILayout.Label(text, style, GUILayout.Width(210 + (9 * EditorGUI.indentLevel)));
            }
            
        }  
        
        public static void Label(string text, params GUILayoutOption[] layouts)
        {
            if(EditorGUI.indentLevel > 0)
            {
                text = "- " + text;
            }

            if (layouts.Length > 0)
            {
                EditorGUILayout.LabelField(text, "", layouts);
            }
            else
            {
                EditorGUILayout.LabelField(text, "", GUILayout.Width(210 + (9 * EditorGUI.indentLevel)));
            }
        }

        public static void Help(string text, int indent, params GUILayoutOption[] layouts)
        {
            EditorGUI.indentLevel = 2 * indent;
            EditorGUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(text))
            {
                EditorGUILayout.LabelField(text, GUI.skin.GetStyle("helpLabel"));
            }
            EndIndent();
        }

        public static void HelpNonRichText(string text, int indent, params GUILayoutOption[] layouts)
        {
            EditorGUI.indentLevel = 2 * indent;
            EditorGUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(text))
            {
                var s = new GUIStyle(GUI.skin.GetStyle("helpLabel"));
                s.richText = false;
                EditorGUILayout.LabelField(text, s);
            }
            EndIndent();
        }

        public static void Title(string title)
        {
            GUILayout.Box(title,"mainTitle");
        }

        public static void SubTitle(string title)
        {
            GUILayout.Box(title, "subTitle");
            
        }

        public static Texture2D ImageSelector(string imageName, Texture2D imageTexture2D, ref string imagePath, bool horizontal = false, params GUILayoutOption[] layout)
        {
            var oldSkin = GUI.skin;
            

            try
            {
                if (horizontal)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(4);
                    if (imageName != "") Label(imageName);
                }

                
                GUI.skin = null;

                imageTexture2D = Resources.Load(imagePath) as Texture2D;

                GUILayout.BeginVertical();
                GUILayout.Space(5);
                if(!horizontal)
                {
                    if (imageName != "") EditorGUILayout.PrefixLabel(imageName);
                }
                if(layout.Length > 0)
                {
                    imageTexture2D =
                    (Texture2D)
                    EditorGUILayout.ObjectField(imageTexture2D, typeof(Texture2D), false, layout);
                }
                else
                {
                    imageTexture2D =
                    (Texture2D)
                    EditorGUILayout.ObjectField(imageTexture2D, typeof(Texture2D), false, GUILayout.Width(80),
                                                GUILayout.Height(80));
                }
                
                var correct = Rme_Main.ValidateImagePath(imageTexture2D, ref imagePath);
                if (!correct) imageTexture2D = null;
                //GUILayout.Label(imageName);
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    imagePath = "";
                    imageTexture2D = null;
                }

                GUILayout.EndVertical();
                if(horizontal)
                {
                    GUILayout.EndHorizontal();    
                }
            }
            catch(ExitGUIException e)
            {
            }

            GUI.skin = oldSkin;
            return imageTexture2D;
        }

        public static AudioClip AudioClipSelector(string prefix, AudioClip music, ref string musicPath,params GUILayoutOption[] layouts)
        {
            return AudioClipSelector(prefix, music, ref musicPath, 0, layouts);
        }
        public static AudioClip AudioClipSelector(string prefix, AudioClip music, ref string musicPath, int indent = 0, params GUILayoutOption[] layouts)
        {
            var oldSkin = GUI.skin;
            try
            {
                GUI.skin = null;
                music = Resources.Load(musicPath) as AudioClip;
                IndentAndPrefix(indent, prefix);
                music =
                    (AudioClip)
                    EditorGUILayout.ObjectField(music, typeof(AudioClip), false,layouts);
                EndIndent();
                var correct = Rme_Main.ValidateAudioPath( music, ref musicPath);
                if (!correct) music = null;
            }
            catch (ExitGUIException e)
            {
            }

            GUI.skin = oldSkin;
            if (music == null) musicPath = "";
            return music;
        }

        public static GameObject PrefabSelector(string prefix, GameObject gameObject, ref string prefabPath, int indent = 0)
        {
            var oldSkin = GUI.skin;
            try
            {
                GUI.skin = null;
                gameObject = Resources.Load(prefabPath) as GameObject;
                IndentAndPrefix(indent, prefix);

                gameObject =
                    (GameObject)
                    EditorGUILayout.ObjectField(gameObject, typeof(GameObject), false);
                EndIndent();
                var correct = Rme_Main.ValidatePath(gameObject, ref prefabPath);
                if (!correct) gameObject = null;
            }
            catch (ExitGUIException e)
            {
            }

            GUI.skin = oldSkin;
            if (gameObject == null) prefabPath = "";
            GUILayout.Space(5);
            return gameObject;
        }

        public static Material MaterialSelector(string prefix, Material material, ref string prefabPath, int indent = 0)
        {
            var oldSkin = GUI.skin;
            try
            {
                GUI.skin = null;
                material = Resources.Load(prefabPath) as Material;
                IndentAndPrefix(indent, prefix);

                material =
                    (Material)
                    EditorGUILayout.ObjectField(material, typeof(Material), false);
                EndIndent();
                var correct = Rme_Main.ValidatePath(material, ref prefabPath);
                if (!correct) material = null;
            }
            catch (ExitGUIException e)
            {
            }

            GUI.skin = oldSkin;
            if (material == null) prefabPath = "";
            GUILayout.Space(5);
            return material;
        }

        public static Sprite SpriteSelector(string prefix, Sprite sprite, ref string prefabPath, int indent = 0)
        {
            var oldSkin = GUI.skin;
            //try
            //{
                GUI.skin = null;
                sprite = Resources.Load(prefabPath) as Sprite;
                IndentAndPrefix(indent, prefix);

                try
                {
                    sprite =
                        (Sprite)
                        EditorGUILayout.ObjectField(sprite, typeof (Sprite), false);
                }
                catch
                {
                }
            EndIndent();
                var correct = Rme_Main.ValidatePath(sprite, ref prefabPath);
                if (!correct) sprite = null;
            //}
            //catch (ExitGUIException e)
            //{
            //    Debug.Log(e);
            //}

            GUI.skin = oldSkin;
            if (sprite == null) prefabPath = "";
            GUILayout.Space(5);
            return sprite;
        }

        public static void ShaderSelector(string prefix, string ShaderName, EditorWindow mainWindow, int indent=0)
        {
            var oldSkin = GUI.skin;
            try
            {
                GUI.skin = null;
                IndentAndPrefix(indent, prefix);
                GUI.enabled = false;
                EditorGUILayout.TextField(ShaderName);
                GUI.enabled = true;
                if (GUILayout.Button("Select shader"))
                {
                    LastSelectedShaderInfo = Rm_RPGHandler.Instance.Combat.ShadersToLerp.FirstOrDefault(s => s.ShaderName == ShaderName);
                    DisplayShaderContext(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.popup), mainWindow);
                }
                EndIndent();
            }
            catch (ExitGUIException e)
            {
            }

            GUI.skin = oldSkin;
            GUILayout.Space(5);
        }
        static MenuCommand mc;
        private static void DisplayShaderContext(Rect r, EditorWindow window)
        {
            if (mc == null)
                mc = new MenuCommand(window, 0);

            string tmpStr = "Shader \"Hidden/tmp_shdr\"{SubShader{Pass{}}}";
            Material temp = new Material(tmpStr); 
            // Rebuild shader menu:
            UnityEditorInternal.InternalEditorUtility.SetupShaderMenu(temp);

            // Destroy temporary shader and material:
            Object.DestroyImmediate(temp.shader, true); 
            Object.DestroyImmediate(temp, true);

            // Display shader popup:
            EditorUtility.DisplayPopupMenu(r, "CONTEXT/ShaderPopup", mc);
        }


        public static bool DeleteButton(float lengths)
        {
            return GUILayout.Button(DelIcon, "genericButton", GUILayout.Width(lengths), GUILayout.Height(lengths));
        }

        public static bool DeleteButton()
        {
            if(GUILayout.Button(DelIcon, "genericButton", GUILayout.Width(30), GUILayout.Height(30)))
            {
                GUI.FocusControl("");
                return true;
            }
            
            return false;
        }

        public static GameObject PrefabGeneratorButton(Rmh_PrefabType prefabType, GameObject gameObject, ref string prefabPath, SkillType? skilltype = null, string param = "")
        {
            if(!string.IsNullOrEmpty(prefabPath))
            {
                gameObject = Resources.Load(prefabPath) as GameObject;
            }

            if(gameObject == null)
            {
                if (GUILayout.Button("Generate Prefab","genericButton", GUILayout.MaxHeight(30)))
                {
                    prefabPath = Rme_PrefabGenerator.GeneratePrefab(prefabType, skilltype, param);
                }
            }
            else
            {
                if (GUILayout.Button("Spawn Prefab To Scene", "genericButton", GUILayout.MaxHeight(30)))
                {
                    var view = SceneView.currentDrawingSceneView;
                    if(view != null)
                    {
                        var target = GeneralMethodsEditor.SpawnLinkedPrefab(prefabPath, view.camera.transform.position + view.camera.transform.forward * 5, Quaternion.identity, null);
                        view.LookAt(target.transform.position);
                        Selection.activeGameObject = target;
                    }
                    else
                    {
                        GeneralMethodsEditor.SpawnLinkedPrefab(prefabPath, Camera.main.transform.position + Camera.main.transform.forward * 5, Quaternion.identity, null);
                    }
                }
            }

            return gameObject;
        }

        public static RuntimeAnimatorController AnimControllerSelector(string prefix, RuntimeAnimatorController controller, ref string controllerPath)
        {
            var oldSkin = GUI.skin;
            try
            {
                GUI.skin = null;
                controller = Resources.Load(controllerPath) as RuntimeAnimatorController;
                controller =
                    (RuntimeAnimatorController)
                    EditorGUILayout.ObjectField(prefix, controller, typeof(RuntimeAnimatorController), false);
                var correct = Rme_Main.ValidatePath(controller, ref controllerPath);
                if (!correct) controller = null;
            }
            catch (ExitGUIException e)
            {
            }

            GUI.skin = oldSkin;
            if (controller == null) controllerPath = "";
            GUILayout.Space(5);
            return controller;
        }

        public static void MinMaxSlider(GUIContent prefix, ref float sliderMin, ref float sliderMax, float leftValue, float rightValue  )
        {
            MinMaxSlider(prefix.text, ref sliderMin, ref sliderMax, leftValue, rightValue);
        }
        public static void MinMaxSlider(ref float sliderMin, ref float sliderMax, float leftValue, float rightValue  )
        {
            MinMaxSlider("", ref sliderMin, ref sliderMax, leftValue, rightValue);
        }

        public static void MinMaxSlider(string prefix, ref float sliderMin, ref float sliderMax, float leftValue, float rightValue  )
        {
            try
            {
                var oldSkin = GUI.skin;
                GUI.skin = null;
                EditorGUILayout.MinMaxSlider(new GUIContent(prefix), ref sliderMin, ref sliderMax, leftValue, rightValue,GUILayout.ExpandWidth(true));
                GUI.skin = oldSkin;
            }
            catch (ExitGUIException e)
            {
            }
        }

        #region Foldout

        public static bool Foldout(ref bool showContents, string title, bool indent = false, string customTitleStyle = "")
        {
            var isOpen = showContents ? "> " : "- "; 
            isOpen = indent ? "\t" + isOpen : isOpen;

            if (GUILayout.Button(isOpen + title, (string.IsNullOrEmpty(customTitleStyle) ? "subTitle" : customTitleStyle)))
            {
                showContents = !showContents;
            }
            if (showContents)
            {
                GUILayout.BeginVertical("foldoutBox");
            }
            return showContents;
        }

        // return -1 for closed, 0-N for button press (any button press means open)
        public static int FoldoutToolBar(ref bool showContents, string title, string button, bool indent = false, bool useStyle = true)
        {
            return FoldoutToolBar(ref showContents, title, new string[] {button},indent,useStyle);
        }

        public static int FoldoutToolBar(ref bool showContents, string title, string[] buttons, bool indent = false, bool useStyle = true)
        {
            var selection = -1;
            GUILayout.BeginHorizontal();
            var isOpen = showContents ? "> " : "- ";
            isOpen = indent ? "\t" + isOpen : isOpen;
            if (GUILayout.Button(isOpen + title, "subTitle"))
            {
                showContents = !showContents;
            }

            if(showContents)
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (GUILayout.Button(buttons[i], "toolBarButtonMini"))
                    {
                        selection = i;
                    }
                }
            }
            
            GUILayout.EndHorizontal();
            if (showContents)
            {
                if(useStyle)
                {
                    GUILayout.BeginVertical("foldoutBox");
                }
                else
                {
                    GUILayout.BeginVertical();
                }
            }
            return selection;
        }

        public static int ToolBar(string title, string[] buttons)
        {
            var selection = -1;
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, "subTitle");
            for (int i = 0; i < buttons.Length; i++)
            {
                if (GUILayout.Button(buttons[i], "toolBarButtonMini"))
                {
                    selection = i;
                }
            }
            GUILayout.EndHorizontal();
            return selection;
        }

        public static void EndFoldout()
        {
            GUILayout.EndVertical();
        }

        #endregion

        #region RPGMakerGUI.ListArea

        public static Texture2D AddIcon = AddIcon ?? Resources.Load("RPGMakerAssets/addIcon") as Texture2D;
        public static Texture2D RPGMakerIcon = RPGMakerIcon ?? Resources.Load("RPGMakerAssets/RPGMakerIcon") as Texture2D;
        public static Texture2D DelIcon = DelIcon ?? Resources.Load("RPGMakerAssets/deleteIcon") as Texture2D;
        public static Texture2D UpIcon = UpIcon ?? Resources.Load("RPGMakerAssets/upIcon") as Texture2D;
        public static Texture2D DownIcon = DownIcon ?? Resources.Load("RPGMakerAssets/downIcon") as Texture2D;
        public static Texture2D CopyIcon = CopyIcon ?? Resources.Load("RPGMakerAssets/copyIcon") as Texture2D;
        public static Texture2D HelpIcon = HelpIcon ?? Resources.Load("RPGMakerAssets/helpIcon") as Texture2D;

        public static Texture2D WarningIcon = WarningIcon ?? Resources.Load("RPGMakerAssets/helpIcon") as Texture2D;
        public static Texture2D InfoIcon = InfoIcon ?? Resources.Load("RPGMakerAssets/helpIcon") as Texture2D;
        public static Texture2D ErrorIcon = ErrorIcon ?? Resources.Load("RPGMakerAssets/helpIcon") as Texture2D;
        public static Texture2D LoadingIcon = LoadingIcon ?? Resources.Load("RPGMakerAssets/ToolbarIcons/saveIcon") as Texture2D;


        private const string buttonStyle = "toolBarButton";
        private const string flexibleButtonStyle = "toolBarButtonFlex";
        private static Vector2 listAreaScrollPos = Vector2.zero;
        private static Vector2 listAreaScrollPosAlt = Vector2.zero;

        public static Rect ListArea<T>(List<T> list, ref T selectedInfo, Rm_ListAreaType listType, bool customAddItem,
                                       bool allowDuplicating,
                                       Rme_ListButtonsToShow buttonsToShow = Rme_ListButtonsToShow.All, bool useAltScroll = false, string additionalInfo = "") where T : class
        {
            var rectForCustomAdd = new Rect();
            if (!list.Contains(selectedInfo)) selectedInfo = null;

            var showNumbers = (listType != Rm_ListAreaType.TierCraftList &&
                                 listType != Rm_ListAreaType.DismantleDefinition);

            if (listType != Rm_ListAreaType.TierCraftList && 
                listType != Rm_ListAreaType.DismantleDefinition 
                ||
                (listType == Rm_ListAreaType.TierCraftList && selectedInfo != null) ||
                (listType == Rm_ListAreaType.DismantleDefinition && selectedInfo != null))
            {
                GUILayout.BeginHorizontal("toolBarBackground", GUILayout.Height(30));
            }
            else
            {
                GUILayout.BeginHorizontal(GUILayout.Height(0));
            }
            
            GUILayout.Space(1);
            if (buttonsToShow == Rme_ListButtonsToShow.All || buttonsToShow == Rme_ListButtonsToShow.AllExceptHelp)
            {
                if (customAddItem)
                {
                    if (selectedInfo != null)
                    {
                        GUILayout.Label(new GUIContent(AddIcon,"Add New"), buttonStyle, GUILayout.Width(30));
                        rectForCustomAdd = GUILayoutUtility.GetLastRect();
                    }
                    else
                    {
                        GUILayout.Label(new GUIContent(AddIcon, "Add New"), flexibleButtonStyle, GUILayout.Width(178));
                        rectForCustomAdd = GUILayoutUtility.GetLastRect();
                    }
                }
                else
                {
                    if (selectedInfo != null)
                    {
                        if (GUILayout.Button(new GUIContent(AddIcon, "Add New"), buttonStyle, GUILayout.Width(30)))
                        {
                            AddItem(listType, ref list, additionalInfo);
                            selectedInfo = list[list.Count - 1];
                            GUI.FocusControl("");
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(new GUIContent(AddIcon, "Add New"), flexibleButtonStyle, GUILayout.Width(178)))
                        {
                            AddItem(listType, ref list, additionalInfo);
                            selectedInfo = list[list.Count - 1];
                            GUI.FocusControl("");
                        }
                    }
                }   

                if (selectedInfo != null)
                {
                    var indexOfSelected = list.IndexOf(selectedInfo);
                    if (GUILayout.Button(new GUIContent(CopyIcon, "Duplicate"), buttonStyle, GUILayout.Width(30)))
                    {
                        var copy = GeneralMethods.CopyObject(selectedInfo);
                        copy = (T) GenerateNewGUID(copy, listType);
                        list.Add(copy);
                        GUI.FocusControl("");
                    }
                    if (GUILayout.Button(new GUIContent(UpIcon, "Move Up"), buttonStyle, GUILayout.Width(30)) && indexOfSelected - 1 >= 0)
                    {
                        var temp = list[indexOfSelected - 1];
                        list[indexOfSelected - 1] = selectedInfo;
                        list[indexOfSelected] = temp;
                        GUI.FocusControl("");
                    }
                    if (GUILayout.Button(new GUIContent(DownIcon, "Move Down"), buttonStyle, GUILayout.Width(30)) &&
                        indexOfSelected + 1 <= list.Count - 1)
                    {
                        var temp = list[indexOfSelected + 1];
                        list[indexOfSelected + 1] = selectedInfo;
                        list[indexOfSelected] = temp;
                        GUI.FocusControl("");
                    }

                    var isDefault = false;
                    var defaultField = selectedInfo.GetType().GetField("IsDefault");
                    if (defaultField != null)
                    {
                        var val = (bool)defaultField.GetValue(selectedInfo);
                        if (val) isDefault = true;
                    }

                    GUI.enabled = !isDefault;
                    if (GUILayout.Button(new GUIContent(DelIcon, "Delete"), buttonStyle, GUILayout.Width(30)))
                    {
                        if(!isDefault)
                        {
                            list.Remove(selectedInfo);
                            selectedInfo = (indexOfSelected - 1) > -1 ? list[indexOfSelected - 1] : default(T);
                            GUI.FocusControl("");
                        }
                    }
                    GUI.enabled = true;
                }
                else
                {
                    //GUILayout.FlexibleSpace();
                }

                }
                else
                {
                    // GUILayout.FlexibleSpace();
                }

                if (selectedInfo != null)
                {
                    if (buttonsToShow != Rme_ListButtonsToShow.None &&
                        buttonsToShow != Rme_ListButtonsToShow.AllExceptHelp)
                    {
                        if (listType == Rm_ListAreaType.TierCraftList || listType == Rm_ListAreaType.DismantleDefinition)
                        {
                            if (GUILayout.Button(new GUIContent(HelpIcon, "Show Help"), flexibleButtonStyle, GUILayout.Width(178)))
                            {
                                selectedInfo = default(T);
                                GUI.FocusControl("");
                            }
                        }
                        else
                        {
                            if (GUILayout.Button(new GUIContent(HelpIcon, "Show Help"), buttonStyle, GUILayout.Width(30)))
                            {
                                selectedInfo = default(T);
                                GUI.FocusControl("");
                            }
                        }

                    }
                }


                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            if (useAltScroll) listAreaScrollPosAlt = GUILayout.BeginScrollView(listAreaScrollPosAlt, false, false);
            else listAreaScrollPos = GUILayout.BeginScrollView(listAreaScrollPos, false, false);
            
            for (int i = 0; i < list.Count; i++)
            {
                var listItem = list[i];
                var indexString = showNumbers ? i + ". " : "";
                GUILayout.BeginHorizontal();
                var ifSelected = selectedInfo == listItem ? "listItemSelected" : "listItem";
                if (GUILayout.Button(indexString + listItem.ToString(), ifSelected, GUILayout.Height(20)))
                {
                    selectedInfo = listItem;
                    GUI.FocusControl("");
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            GUILayout.Space(1);
            return rectForCustomAdd;
        }

        private static object GenerateNewGUID(object copy, Rm_ListAreaType listType)
        {
            switch (listType)
            {

                case Rm_ListAreaType.CreditsInfo:
                    break;
                case Rm_ListAreaType.Attributes:
                    var attr = copy as Rm_AttributeDefintion;
                    attr.ID = Guid.NewGuid().ToString();
                    return attr;
                case Rm_ListAreaType.Statistics:
                    var stat = copy as Rm_StatisticDefintion;
                    stat.ID = Guid.NewGuid().ToString();
                    return stat;
                case Rm_ListAreaType.Vitals:
                    var Vitals = copy as Rm_VitalDefinition;
                    Vitals.ID = Guid.NewGuid().ToString();
                    return Vitals;
                case Rm_ListAreaType.Traits:
                    var Traits = copy as Rm_TraitDefintion;
                    Traits.ID = Guid.NewGuid().ToString();
                    return Traits;
                case Rm_ListAreaType.Classes:
                    var Classes = copy as Rm_ClassDefinition;
                    Classes.ID = Guid.NewGuid().ToString();
                    return Classes;
                case Rm_ListAreaType.Pets:
                    var pets = copy as Rm_PetDefinition;
                    pets.ID = Guid.NewGuid().ToString();
                    return pets;
                case Rm_ListAreaType.Enemies:
                    var Enemies = copy as CombatCharacter;
                    Enemies.ID = Guid.NewGuid().ToString();
                    return Enemies;
                case Rm_ListAreaType.NPCs:
                    var NPCs = copy as NonPlayerCharacter;
                    var trees = Rm_RPGHandler.Instance.Nodes.DialogNodeBank.NodeTrees;
                    var newId = Guid.NewGuid().ToString();
                    var existingTree = trees.FirstOrDefault(t => t.ID == NPCs.ID);
                    if (existingTree != null)
                    {
                        var treeCopy = GeneralMethods.CopyObject(existingTree);
                        treeCopy.ID = newId;
                        trees.Add(treeCopy);
                    }
                    NPCs.ID = newId;
                    NPCs.Interaction.ConversationNodeId = newId;
                    return NPCs;
                case Rm_ListAreaType.VendorShops:
                    var VendorShops = copy as VendorShop;
                    VendorShops.ID = Guid.NewGuid().ToString();
                    return VendorShops;
                case Rm_ListAreaType.VendorShopItem:
                    break;
                case Rm_ListAreaType.CraftList:
                    break;
                case Rm_ListAreaType.CraftListItem:
                    break;
                case Rm_ListAreaType.QuestItems:
                    var QuestItems = copy as QuestItem;
                    QuestItems.ID = Guid.NewGuid().ToString();
                    return QuestItems;
                case Rm_ListAreaType.LootTables:
                    var LootTables = copy as Rm_LootTable;
                    LootTables.ID = Guid.NewGuid().ToString();
                    return LootTables;
                case Rm_ListAreaType.LootTableItem:
                    break;
                case Rm_ListAreaType.TierCraftList:
                    break;
                case Rm_ListAreaType.DismantleDefinition:
                    break;
                case Rm_ListAreaType.DismantleItem:
                    break;
                case Rm_ListAreaType.WorldAreas:
                    var worldArea = copy as WorldArea;
                    worldArea.ID = Guid.NewGuid().ToString();
                    break;
                case Rm_ListAreaType.Location:
                    var location = copy as Location;
                    location.ID = Guid.NewGuid().ToString();
                    break;
                case Rm_ListAreaType.Talents:
                    var Talents = copy as Talent;
                    Talents.ID = Guid.NewGuid().ToString();
                    return Talents;
                case Rm_ListAreaType.TalentGroup:
                    var group = copy as Talent;
                    group.ID = Guid.NewGuid().ToString();
                    return group;
                case Rm_ListAreaType.StatusEffect:
                    var StatusEffect = copy as StatusEffect;
                    StatusEffect.ID = Guid.NewGuid().ToString();
                    return StatusEffect;
                case Rm_ListAreaType.QuestChains:
                    var QuestChains = copy as QuestChain;
                    QuestChains.ID = Guid.NewGuid().ToString();
                    return QuestChains;
                case Rm_ListAreaType.Quests:
                    var Quests = copy as Quest;
                    Quests.ID = Guid.NewGuid().ToString();
                    return Quests;
                case Rm_ListAreaType.Reputations:
                    var Reputations = copy as ReputationDefinition;
                    Reputations.ID = Guid.NewGuid().ToString();
                    return Reputations;
                case Rm_ListAreaType.Interactables:
                    var Interactables = copy as Interactable;
                    var trees2 = Rm_RPGHandler.Instance.Nodes.DialogNodeBank.NodeTrees;
                    var newId2 = Guid.NewGuid().ToString();
                    var existingTree2 = trees2.FirstOrDefault(t => t.ID == Interactables.ID);
                    if (existingTree2 != null)
                    {
                        var treeCopy = GeneralMethods.CopyObject(existingTree2);
                        treeCopy.ID = newId2;
                        trees2.Add(treeCopy);
                    }
                    Interactables.ID = newId2;
                    Interactables.ConversationNodeId = newId2;
                    Interactables.ID = Guid.NewGuid().ToString();
                    return Interactables;
                case Rm_ListAreaType.Harvestables:
                    var Harvestables = copy as Harvestable;
                    Harvestables.ID = Guid.NewGuid().ToString();
                    return Harvestables;
                case Rm_ListAreaType.CustomVaraibles:
                    var CustomVaraibles = copy as Rmh_CustomVariable;
                    CustomVaraibles.ID = Guid.NewGuid().ToString();
                    return CustomVaraibles;
                case Rm_ListAreaType.Skills:
                    var Skills = copy as Skill;
                    Skills.ID = Guid.NewGuid().ToString();
                    return Skills;
                case Rm_ListAreaType.Items:
                    var Items = copy as Item;
                    Items.ID = Guid.NewGuid().ToString();
                    return Items;
                case Rm_ListAreaType.Races:
                    var races = copy as Rm_RaceDefinition;
                    races.ID = Guid.NewGuid().ToString();
                    return races;
                case Rm_ListAreaType.SubRaces:  
                    var SubRaces = copy as Rm_SubRaceDefinition;
                    SubRaces.ID = Guid.NewGuid().ToString();
                    return SubRaces;
                case Rm_ListAreaType.MetaDatas:
                    var metadatas = copy as Rm_MetaDataDefinition;
                    metadatas.ID = Guid.NewGuid().ToString();
                    return metadatas;
                case Rm_ListAreaType.Genders:
                    var Genders = copy as StringDefinition;
                    Genders.ID = Guid.NewGuid().ToString();
                    return Genders;
                case Rm_ListAreaType.ClassName:
                    var ClassName = copy as Rm_ClassNameDefinition;
                    ClassName.ID = Guid.NewGuid().ToString();
                    return ClassName;
                default:
                    throw new ArgumentOutOfRangeException("listType");
            }

            return copy;
        }

        private static void AddItem<T>(Rm_ListAreaType listType, ref List<T> list, string additionalInfo = "") where T : class
        {
            switch (listType)
            {
                case Rm_ListAreaType.CreditsInfo:
                    list.Add(new Rm_CreditsInfo() as T);
                    break;
                case Rm_ListAreaType.Attributes:
                    list.Add(new Rm_AttributeDefintion() as T);
                    break;
                case Rm_ListAreaType.Statistics:
                    list.Add(new Rm_StatisticDefintion() as T);
                    break;
                case Rm_ListAreaType.Vitals:
                    list.Add(new Rm_VitalDefinition() as T);
                    break;
                case Rm_ListAreaType.Traits:
                    list.Add(new Rm_TraitDefintion() as T);
                    break;
                case Rm_ListAreaType.Classes:
                    list.Add(new Rm_ClassDefinition() as T);
                    break;
                case Rm_ListAreaType.Pets:
                    list.Add(new Rm_PetDefinition() as T);
                    break;
                case Rm_ListAreaType.Enemies:
                    var newEnemy = new CombatCharacter() as T;
                    var x = newEnemy as CombatCharacter;
                    x.Init();
                    list.Add(newEnemy);
                    break;
                case Rm_ListAreaType.NPCs:
                    var newNpc = new NonPlayerCharacter() as T;
                    var n = newNpc as NonPlayerCharacter;
                    n.Init();
                    list.Add(newNpc);
                    break;
                case Rm_ListAreaType.VendorShops:
                    list.Add(new VendorShop() as T);
                    break;
                case Rm_ListAreaType.VendorShopItem:
                    list.Add(new VendorShopItem() as T);
                    break;
                case Rm_ListAreaType.WorldAreas:
                    list.Add(new WorldArea() as T);
                    break;
                case Rm_ListAreaType.Location:
                    var l = new Location() as T;
                    var loc = l as Location;
                    if(!string.IsNullOrEmpty(additionalInfo))
                    {
                        loc.WorldAreaID = additionalInfo;
                    }
                    list.Add(l);
                    break;
                case Rm_ListAreaType.CraftList:
                    list.Add(new Rm_CustomCraftList() as T);
                    break;
                case Rm_ListAreaType.CraftListItem:
                    list.Add(new CraftListItem() as T);
                    break;
                case Rm_ListAreaType.QuestItems:
                    list.Add(new QuestItem() as T);
                    break;
                case Rm_ListAreaType.LootTables:
                    list.Add(new Rm_LootTable() as T);
                    break;
                case Rm_ListAreaType.LootTableItem:
                    list.Add(new Rm_LootTableItem() as T);
                    break;
                case Rm_ListAreaType.DismantleDefinition:
                    break;
                case Rm_ListAreaType.DismantleItem:
                    list.Add(new DismantleItem() as T);
                    break;
                case Rm_ListAreaType.TierCraftList:
                    break;
                case Rm_ListAreaType.Talents:
                    list.Add(new Talent() as T);
                    break;
                case Rm_ListAreaType.TalentGroup:
                    list.Add(new TalentGroup() as T);
                    break;
                case Rm_ListAreaType.StatusEffect:
                    list.Add(new StatusEffect() as T);
                    break;
                case Rm_ListAreaType.QuestChains:
                    list.Add(new QuestChain() as T);
                    break;
                case Rm_ListAreaType.Quests:
                    var q = new Quest() as T;
                    var quest = q as Quest;
                    if(!string.IsNullOrEmpty(additionalInfo))
                    {
                        quest.QuestChainId = additionalInfo;
                    }
                    list.Add(q);
                    break;
                case Rm_ListAreaType.Reputations:
                    list.Add(new ReputationDefinition() as T);
                    break;
                case Rm_ListAreaType.Interactables:
                    list.Add(new Interactable() as T);
                    break;
                case Rm_ListAreaType.Harvestables:
                    list.Add(new Harvestable() as T);
                    break;
                case Rm_ListAreaType.CustomVaraibles:
                    list.Add(new Rmh_CustomVariable() as T);
                    break;
                case Rm_ListAreaType.Races:
                    list.Add(new Rm_RaceDefinition() as T);
                    break;
                case Rm_ListAreaType.SubRaces:
                    list.Add(new Rm_SubRaceDefinition() as T);
                    break;
                case Rm_ListAreaType.MetaDatas:
                    list.Add(new Rm_MetaDataDefinition() as T);
                    break;
                case Rm_ListAreaType.Genders:
                    list.Add(new StringDefinition() as T);
                    break;
                case Rm_ListAreaType.ClassName:
                    list.Add(new Rm_ClassNameDefinition() as T);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("listType");
            }
        }

        

        #endregion

        #region ScrollView

        private static Vector2 ScrollViewScrollPos = Vector2.zero;

        public static void BeginScrollView()
        {
            ScrollViewScrollPos = GUILayout.BeginScrollView(ScrollViewScrollPos);
        }

        public static void EndScrollView()
        {
            GUILayout.EndScrollView();
            GUILayout.Space(1);
        }

        #endregion

        public static void ResetScrollPositions()
        {
            listAreaScrollPos = Vector2.zero;
            ScrollViewScrollPos = Vector2.zero;
        }


        private static int[] selectedArrayIndex = new int[999];
        public static int selectedVarSetterBoolResult;
        public static void FoldoutList<T,V>(ref bool showFoldout, string title, List<T> list, List<V> allList,  string addButtonName, string type = "", string helpString = "",
            string listIDName = "ID", string listNameName = "Name", string allListIDName = "ID", string allListNameName = "Name", bool indent = false, string param = "") where T : class, new()
        {
            var vitBuffResult = FoldoutToolBar(ref showFoldout, title, addButtonName, indent);
            if (showFoldout)
            {
                for (int index = 0; index < list.Count; index++)
                {
                    var entry = list[index];

                    var idProperty = entry.GetType().GetField(listIDName);
                    var idValue = (string) idProperty.GetValue(entry);

                    if (String.IsNullOrEmpty(idValue)) continue;

                    var stillExists =
                        allList.FirstOrDefault(a => (string)a.GetType().GetField(allListIDName).GetValue(a) == idValue);

                    if (stillExists == null)
                    {
                        list.Remove(entry);
                        index--;
                    }
                }

                if (allList.Count > 0)
                {
                    if (list.Count == 0)
                    {
                        EditorGUILayout.HelpBox(helpString, MessageType.Info);
                    }

                    GUILayout.Space(5);
                    for (int index = 0; index < list.Count; index++)
                    {
                        GUILayout.BeginHorizontal();
                        var currentListItem = list[index];
                        var currentListItemId =
                            (string)currentListItem.GetType().GetField(listIDName).GetValue(list[index]);
                        if (string.IsNullOrEmpty(currentListItemId))
                        {
                            selectedArrayIndex[index] = 0;
                        }
                        else
                        {
                            var stillExists = allList.FirstOrDefault(a => (string)a.GetType().GetField(allListIDName).GetValue(a) == currentListItemId);
                            selectedArrayIndex[index] = stillExists != null ? allList.IndexOf(stillExists) : 0;
                        }
                        GUILayout.BeginHorizontal();
                        var prevSelIndex = selectedArrayIndex[index];

                        var prefix = type != "" ? type + " Name:" : "";
                        selectedArrayIndex[index] = EditorGUILayout.Popup(prefix, selectedArrayIndex[index],
                                                                         allList.Select((q, indexOf) => indexOf + ". " + q.GetType().GetField(allListNameName).GetValue(q)).
                                                                             ToArray());
                        if (prevSelIndex != selectedArrayIndex[index])
                        {
                            GUI.FocusControl("");
                        }

                        var property = list[index].GetType().GetField(listIDName);
                        var allListItemId = (string)allList[selectedArrayIndex[index]].GetType().GetField(allListIDName).GetValue(allList[selectedArrayIndex[index]]);
                        property.SetValue(list[index], allListItemId);

                        //Class specific property editing
                        if(typeof(T) == typeof(AttributeBuff))
                        {
                            var attrBuff = list[index] as AttributeBuff;
                            attrBuff.Amount = IntField("Amount:", attrBuff.Amount);

                        }
                        else if(typeof(T) == typeof(StatisticBuff))
                        {
                            var attrBuff = list[index] as StatisticBuff;
                            attrBuff.Amount = FloatField("Amount:", attrBuff.Amount);
                        }
                        else if(typeof(T) == typeof(VitalBuff))
                        {
                            var attrBuff = list[index] as VitalBuff;
                            attrBuff.Amount = IntField("Amount:", attrBuff.Amount);
                        }
                        else if(typeof(T) == typeof(VitalRegenBonus))
                        {
                            var attrBuff = list[index] as VitalRegenBonus;
                            attrBuff.RegenBonus = FloatField("Regen Bonus:", attrBuff.RegenBonus);
                        }
                        else if(typeof(T) == typeof(ReduceStatusDuration))
                        {
                            var attrBuff = list[index] as ReduceStatusDuration;
                            attrBuff.IsPercentageDecrease = EditorGUILayout.Toggle("Is Percentage?", attrBuff.IsPercentageDecrease);
                            attrBuff.DecreaseAmount = FloatField("Decrease Amount:", attrBuff.DecreaseAmount);
                        }
                        else if(typeof(T) == typeof(ItemReward))
                        {
                            var attrBuff = list[index] as ItemReward;
                            //Check items, then craftables, then quest items to try find it
                            var item = Rm_RPGHandler.Instance.Repositories.Items.AllItems.FirstOrDefault(i => i.ID == attrBuff.ItemID)
                                ?? Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems.FirstOrDefault(i => i.ID == attrBuff.ItemID)
                                ?? Rm_RPGHandler.Instance.Repositories.QuestItems.AllItems.FirstOrDefault(i => i.ID == attrBuff.ItemID);
                            
                            var stackable = item as IStackable;
                            if(stackable != null)
                                attrBuff.Amount = IntField("Amount:", attrBuff.Amount);
                        }
                        else if(typeof(T) == typeof(Rm_CustomVariableGetSet))
                        {
                            var attrBuff = list[index] as Rm_CustomVariableGetSet;
                            var foundCvar = allList[selectedArrayIndex[index]] as Rmh_CustomVariable;
                            var prefixLabel = param == "Value" ? "Equals: " : "Set To:";
                            GUILayout.Label(prefixLabel,GUILayout.Width(100));
                            switch (foundCvar.VariableType)
                            {
                                case Rmh_CustomVariableType.Float:
                                    attrBuff.FloatValue = FloatField("", attrBuff.FloatValue);
                                    break;
                                case Rmh_CustomVariableType.Int:
                                    attrBuff.IntValue = IntField("", attrBuff.IntValue);
                                    break;
                                case Rmh_CustomVariableType.String:
                                    attrBuff.StringValue = TextField("", attrBuff.StringValue);
                                    break;
                                case Rmh_CustomVariableType.Bool:
                                    selectedVarSetterBoolResult = attrBuff.BoolValue ? 0 : 1;
                                    selectedVarSetterBoolResult = EditorGUILayout.Popup(prefixLabel,
                                                                                        selectedVarSetterBoolResult,
                                                                                        new[] { "True", "False" });
                                    attrBuff.BoolValue = selectedVarSetterBoolResult == 0;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
  
                        if (DeleteButton())
                        {
                            list.Remove(list[index]);
                            index--;
                            return;
                        }
                        GUILayout.EndHorizontal();


                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No " + type +  "s Found.", MessageType.Warning);
                    list = new List<T>();
                }

                if (vitBuffResult == 0)
                {
                    list.Add(new T());
                    GUI.FocusControl("");
                }
                EndFoldout();
            }
        }

        private static int[] selectedPopupArrayIndex = new int[999];
        public static int selectedPopupResult;
        private static int selectedIndex;

        private static List<NodeChain> nodeCache = null;
        private static int oldNodeCount = -1;
        private static int cacheTick = 0;

        public static void PopupID<T>(string prefix, ref string idField, int indent) where T : class
        {
            PopupID<T>(prefix, ref idField, "ID","Name","",null,indent);
        }
        public static void PopupID<T> (string prefix, ref string idField, string allListIDName = "ID", string allListNameName = "Name", string param = "", List<T> customList = null, int indent = 0 ) where T : class 
        {

            List<T> allList = null;
            if (typeof(T) == typeof (StatusEffect))
            {
                allList = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects as List<T>;
            }
            else if (typeof(T) == typeof (Rm_AttributeDefintion))
            {
                allList = Rm_RPGHandler.Instance.ASVT.AttributesDefinitions as List<T>;
            }
            else if (typeof(T) == typeof(Rm_RaceDefinition))
            {
                allList = Rm_RPGHandler.Instance.Player.RaceDefinitions as List<T>;
            }
            else if (typeof(T) == typeof(Rm_SubRaceDefinition))
            {
                allList = Rm_RPGHandler.Instance.Player.SubRaceDefinitions as List<T>;
            }
            else if (typeof(T) == typeof(Rm_ClassNameDefinition))
            {
                allList = Rm_RPGHandler.Instance.Player.ClassNameDefinitions as List<T>;
            }
            else if (typeof(T) == typeof (Rm_VitalDefinition))
            {
                allList = Rm_RPGHandler.Instance.ASVT.VitalDefinitions as List<T>;
            }
            else if (typeof(T) == typeof (Rm_StatisticDefintion))
            {
                allList = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions as List<T>;
            }
            else if (typeof(T) == typeof (Rm_TraitDefintion))
            {
                allList = Rm_RPGHandler.Instance.ASVT.TraitDefinitions as List<T>;
            }
            else if (typeof(T) == typeof (WorldArea))
            {
                allList = Rm_RPGHandler.Instance.Customise.WorldMapLocations as List<T>;
            }
            else if (typeof(T) == typeof (Location))
            {
                allList = Rm_RPGHandler.Instance.Customise.WorldMapLocations.SelectMany(w => w.Locations).ToList() as List<T>;
            }
            else if (typeof(T) == typeof (Skill))
            {
                allList = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills as List<T>;
            }
            else if (typeof(T) == typeof (Talent))
            {
                allList = Rm_RPGHandler.Instance.Repositories.Talents.AllTalents as List<T>;
            }
            else if (typeof(T) == typeof (VendorShop))
            {
                allList = Rm_RPGHandler.Instance.Repositories.Vendor.AllVendors as List<T>;
            }
            else if (typeof(T) == typeof (ExpDefinition))
            {
                if (param == "Trait")
                {
                    allList = Rm_RPGHandler.Instance.Experience.AllExpDefinitions.Where(a => a.ExpUse == ExpUse.TraitLevelling).ToList() as List<T>;    
                }
                else if(param == "ExpGained")
                {
                    allList = Rm_RPGHandler.Instance.Experience.AllExpDefinitions.Where(a => a.ExpUse == ExpUse.ExpGained).ToList() as List<T>;    
                }
                else
                {
                    allList = Rm_RPGHandler.Instance.Experience.AllExpDefinitions.Where(a => a.ExpUse == ExpUse.PlayerLevelling).ToList() as List<T>;    
                }
            }
            else if (typeof(T) == typeof (TalentGroup))
            {
                allList = Rm_RPGHandler.Instance.Repositories.Talents.AllTalentGroups as List<T>;
            }
            else if (typeof(T) == typeof(Item))
            {
                if (param == "Craft")
                {
                    allList = Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems as List<T>;
                }
                else if (param == "Quest")
                {
                    allList = Rm_RPGHandler.Instance.Repositories.QuestItems.AllItems as List<T>;
                }
                else if (param == "CraftAndItem")
                {
                    allList = Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems.Concat(Rm_RPGHandler.Instance.Repositories.Items.AllItems).ToList() as List<T>;
                }
                else if (param == "CraftItemAndQuest")
                {
                    allList = Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems.Concat(Rm_RPGHandler.Instance.Repositories.Items.AllItems)
                        .Concat(Rm_RPGHandler.Instance.Repositories.QuestItems.AllItems).ToList() as List<T>;
                }
                else
                {
                    allList = Rm_RPGHandler.Instance.Repositories.Items.AllItems as List<T>;
                }
            }
            else if (typeof(T) == typeof(Rm_ClassDefinition))
            {
                allList = Rm_RPGHandler.Instance.Player.CharacterDefinitions as List<T>;
            }
            else if (typeof(T) == typeof(Rm_PetDefinition))
            {
                allList = Rm_RPGHandler.Instance.Player.PetDefinitions as List<T>;
            }
            else if (typeof(T) == typeof(Rmh_CustomVariable))
            {
                allList = Rm_RPGHandler.Instance.DefinedVariables.Vars as List<T>;
            }
            else if (typeof(T) == typeof(ReputationDefinition))
            {
                allList = Rm_RPGHandler.Instance.Repositories.Quests.AllReputations as List<T>;
            }
            else if (typeof(T) == typeof(SkillMetaDefinition))
            {
                allList = Rm_RPGHandler.Instance.Combat.SkillMeta as List<T>;
            }
            else if (typeof(T) == typeof(Quest))
            {
                allList = Rm_RPGHandler.Instance.Repositories.Quests.AllQuests as List<T>;
            }
            else if (typeof(T) == typeof(NonPlayerCharacter))
            {
                allList = Rm_RPGHandler.Instance.Repositories.Interactable.AllNpcs as List<T>;
            }
            else if (typeof(T) == typeof(Interactable))
            {
                allList = Rm_RPGHandler.Instance.Repositories.Interactable.AllInteractables as List<T>;
            }
            else if (typeof(T) == typeof(SlotDefinition))
            {
                allList = Rm_RPGHandler.Instance.Items.ApparelSlots as List<T>;
            }
            else if (typeof(T) == typeof(WeaponTypeDefinition))
            {
                allList = Rm_RPGHandler.Instance.Items.WeaponTypes as List<T>;
            }
            else if (typeof(T) == typeof(Rm_MetaDataDefinition))
            {
                allList = Rm_RPGHandler.Instance.Player.MetaDataDefinitions as List<T>;
            }
            else if (typeof(T) == typeof(DifficultyDefinition))
            {
                allList = Rm_RPGHandler.Instance.Player.Difficulties as List<T>;
            }
            else if (typeof(T) == typeof(RarityDefinition))
            {
                allList = Rm_RPGHandler.Instance.Items.ItemRarities as List<T>;
            }
            else if (typeof(T) == typeof(CombatCharacter))
            {
                allList = Rm_RPGHandler.Instance.Repositories.Enemies.AllEnemies as List<T>;
            }
            else if (typeof(T) == typeof(Rm_LootTable))
            {
                allList = Rm_RPGHandler.Instance.Repositories.LootTables.AllTables as List<T>;
            }
            else if (typeof(T) == typeof(Achievement))
            {
                allList = Rm_RPGHandler.Instance.Repositories.Achievements.AllAchievements as List<T>;
            }
            else if (typeof(T) == typeof(MonsterTypeDefinition))
            {
                allList = Rm_RPGHandler.Instance.Enemy.MonsterTypes as List<T>;
            }
            else if (typeof(T) == typeof(NodeChain))
            {
                var trees = Rm_RPGHandler.Instance.Nodes.EventNodeTrees;
                cacheTick++;

                var getList = true;
                if(nodeCache != null)
                {                
                    var nodeCount = trees.SelectMany(t => t.Nodes).Count();
                    getList = oldNodeCount != nodeCount;
                    if(getList)
                    {
                        oldNodeCount = nodeCount;
                    }
                }

                if(cacheTick > 50)
                {
                    getList = true;
                    cacheTick = 0;
                }

                if(getList)
                {                    
                    nodeCache = new List<NodeChain>();
                    nodeCache = Rm_RPGHandler.Instance.Nodes.EventNodeChains;
                }

                allList = nodeCache as List<T>;
            }
            else if (typeof(T) == typeof(NodeTree))
            {
                if (param == "Dialog")
                {
                    allList = Rm_RPGHandler.Instance.Nodes.DialogNodeBank.NodeTrees as List<T>;
                }
                else
                {
                    allList = Rm_RPGHandler.Instance.Nodes.EventNodeTrees as List<T>;
                }
            }

            if (customList != null) allList = customList;

            var currentID = idField; 
            if (string.IsNullOrEmpty(idField))
            {
                selectedIndex = 0;
            }
            else
            {
                T stillExists;
                if (typeof(T) == typeof(NodeChain))
                {
                    var allNodeChains = allList as List<NodeChain>;
                    stillExists = allNodeChains.FirstOrDefault(a => a.CurrentNode.ID == currentID) as T;
                }
                else
                {
                    stillExists = allList.FirstOrDefault(a => (string)a.GetType().GetField(allListIDName).GetValue(a) == currentID);
                }

                selectedIndex = stillExists != null ? allList.IndexOf(stillExists) : 0;
            }

            if(allList == null)
            {
                Debug.Log("allList is null!" + typeof(T));
            }

            if (allList.Count > 0)
            {
                var prevSelIndex = selectedIndex;

                String[] popupList;
                if(typeof(T) == typeof(NodeChain))
                {
                    var allNodeChains = allList as List<NodeChain>;
                    popupList = allNodeChains.Select((q, indexOf) => indexOf + ". " + q.NodeTreeName + " - " +  q.Name).ToArray();
                }
                else
                {
                    popupList = allList.Select((q, indexOf) => indexOf + ". " + q.GetType().GetField(allListNameName).GetValue(q)).ToArray();
                }

                selectedIndex = Popup(prefix, selectedIndex, popupList,indent);
                if (prevSelIndex != selectedIndex)
                {
                    GUI.FocusControl("");
                }

                string allListItemId;
                if (typeof(T) == typeof(NodeChain))
                {
                    var allNodeChains = allList as List<NodeChain>;
                    allListItemId = allNodeChains[selectedIndex].CurrentNode.ID;
                }
                else
                {
                    allListItemId = (string)allList[selectedIndex].GetType().GetField(allListIDName).GetValue(allList[selectedIndex]);
                }

                
                idField = allListItemId;
            }
            else
            {
                EditorGUI.indentLevel = 2 * indent;

                EditorGUILayout.BeginHorizontal();
                if(!string.IsNullOrEmpty(prefix))
                {
                    TextField(prefix, "None Found",1);
                }
                else
                {
                    TextField("", "None Found",1);
                }
                idField = "";
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel = 0;
            }
        }

        public static int Popup(string prefix,  int selected, string[] popupList, int indent = 0)
        {
            IndentAndPrefix(indent, prefix);
            selected = EditorGUILayout.Popup(selected, popupList);
            EndIndent();
            return selected;
        }

        public static int selectedScene;

        public static string SceneSelector(string prefix, ref string sceneName)
        {
            IndentAndPrefix(0, prefix);
            var scenes = ReadSceneNames.scenes;
            if (scenes.Length > 0)
            {
                selectedScene = Array.IndexOf(scenes, sceneName);
                if (selectedScene == -1) selectedScene = 0;

                selectedScene = EditorGUILayout.Popup(selectedScene, scenes);
                sceneName = scenes[selectedScene];
            }
            else
            {
                sceneName = "";
                GUILayout.Box(" None Found. ", "genericButton", GUILayout.ExpandWidth(false));
            }
            EndIndent();
            return sceneName;
        }

        
    }

    public enum Rme_ListButtonsToShow
    {
        All,
        AllExceptHelp,
        HelpOnly,
        None
    }

    public enum Rm_ListAreaType
    {
        CreditsInfo,
        Attributes,
        Statistics,
        Vitals,
        Traits,
        Classes,
        Enemies,
        NPCs,
        VendorShops,
        CraftList,
        TierCraftList,
        QuestItems,
        LootTables,
        Skills,
        Talents,
        TalentGroup,
        StatusEffect,
        QuestChains,
        Quests,
        Reputations,
        Interactables,
        Harvestables,
        CustomVaraibles,
        Items //Custom Add
        ,
        VendorShopItem,
        CraftListItem,
        LootTableItem,
        DismantleDefinition,
        DismantleItem,
        WorldAreas,
        Location,
        Pets,
        Races,
        SubRaces,
        Genders,
        ClassName,
        MetaDatas,
    }
}