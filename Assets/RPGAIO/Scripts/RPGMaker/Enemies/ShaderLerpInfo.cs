using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class ShaderLerpInfo
    {
        public string ShaderName;
        public List<ShaderPropLerpInfo> PropsToLerp;

        public ShaderLerpInfo()
        {
            ShaderName = "";
            PropsToLerp = new List<ShaderPropLerpInfo>();
        }
    }

    public class ShaderPropLerpInfo
    {
        public bool LerpThisProperty;
        public string PropName;
        public string PropDesc;
        public ShaderType PropType;
        public float LerpTo;
        public bool OnlyLerpAlpha;

        public float[] LerpToColorArray;

        [JsonIgnore]
        private Color _lerpToColor = Color.clear;   

        [JsonIgnore]
        public Color LerpToColor
        {
            get
            {
                if (_lerpToColor != Color.clear)
                {
                    return _lerpToColor;
                }
                return _lerpToColor = new Color(LerpToColorArray[0], LerpToColorArray[1], LerpToColorArray[2], LerpToColorArray[3]);
            }
            set
            {
                _lerpToColor = value;
                LerpToColorArray[0] = _lerpToColor.r;
                LerpToColorArray[1] = _lerpToColor.g;
                LerpToColorArray[2] = _lerpToColor.b;
                LerpToColorArray[3] = _lerpToColor.a;
            }
        }

        public ShaderPropLerpInfo()
        {
            LerpThisProperty = false;
            PropType = ShaderType.Color;
            PropName = "";
            PropDesc = "";
            LerpTo = 0f;
            _lerpToColor = Color.clear;
            LerpToColorArray = new float[] { 0,0,0,0};
            OnlyLerpAlpha = true;
        }

    }

    public enum ShaderType
    {
        Color,
        Float,
        Range
    }
}