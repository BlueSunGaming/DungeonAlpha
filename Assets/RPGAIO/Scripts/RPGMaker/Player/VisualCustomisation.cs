using System;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class VisualCustomisation
    {
        public string Id;
        public string Identifier;
        public VisualCustomisationType CustomisationType;
        public VisualDisplayType DisplayType;

        public string TargetedGameObjectName;
        public List<string> TargetedGameObjectNames;
        public List<string> MaterialPaths;
        public string StringRef;
        public string StringRefTwo;
        public float MinFloatValue;
        public float MaxFloatValue;
        
        //Label-specific
        public List<string> LabelOptions;

        //Image-select specific
        public List<ImageContainer> ImageOptions;

        //Scaling-child-objects
        public List<VisualCustomisation> ChildCustomisations;

        //Scale-specific
        public bool ScaleX;
        public bool ScaleY;
        public bool ScaleZ;

        //Color-specific
        public List<RPG_Color> ColorOptions;

        //Saved Values
        public float SavedFloatValue;
        public RPG_Color SavedColorValue;
        public string SavedStringValue;

        //Starting Values:
        public float StartingValueFloat;
        public int StartingValueInt;

        public VisualCustomisation()
        {
            Id = Guid.NewGuid().ToString();
            Identifier = "New Customisation";
            CustomisationType = VisualCustomisationType.BlendShape;
            DisplayType = VisualDisplayType.Slider;
            TargetedGameObjectName = "";
            StringRef = "";
            StringRefTwo = "";
            MinFloatValue = 0;
            MaxFloatValue = 1;

            StartingValueFloat = 1.0f;
            StartingValueInt = 0;

            ScaleX = true;
            ScaleY = true;
            ScaleZ = true;

            ChildCustomisations = new List<VisualCustomisation>();
            LabelOptions = new List<string>();
            ImageOptions = new List<ImageContainer>();
            TargetedGameObjectNames = new List<string>();
            MaterialPaths = new List<string>();
            ColorOptions = new List<RPG_Color>();
        }
    }

    public class RPG_Color
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public RPG_Color()
        {
            r = 1;
            g = 1;
            b = 1;
            a = 1;
        }

        public Color ToUnityColor()
        {
            return new Color(r,g,b,a);
        }

        public void SetFromColor(Color color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }
    }

    public enum VisualCustomisationType : int
    {
        BlendShape = 0,

        MaterialColor = 1,

        Scale = 2,
        
        GameObject = 3,
        
        MaterialChange = 4,

        Category = 999,
    }

    public enum VisualDisplayType : int
    {
        Slider = 0,

        Color = 1,

        TextOptions = 2,

        ImageOptions = 3, 

        TextList = 4, 
    }
}