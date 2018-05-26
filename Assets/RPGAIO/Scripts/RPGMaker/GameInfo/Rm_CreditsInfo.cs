using System;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Rm_CreditsInfo
    {
        public string ImagePath;
        [JsonIgnore]
        public Texture2D _image ;
        [JsonIgnore]
        public Texture2D Image
        {
            get { return _image ?? (_image = Resources.Load(ImagePath) as Texture2D); }
            set { _image = value; }
        }

        public CreditsType Type = CreditsType.Name;
        public string Text = "Text";
        public string Title = "Title";
        public string Role = "Role";
        public string Name = "Name of Person";
        public int Space = 25;

        public override string ToString()
        {
            switch(Type)
            {
                case CreditsType.Logo:
                    return !String.IsNullOrEmpty(ImagePath) ? "Image - " + ImagePath.Split('/')[ImagePath.Split('/').Length - 1] : "Image";
                case CreditsType.Name:
                    return "Name - " + Name;
                case CreditsType.RoleTitle:
                    return "Role - " + Role;
                case CreditsType.Title:
                    return "Title - " + Title;
                case CreditsType.Space:
                    return Space + "px Space";
                case CreditsType.Text:
                    return Text.Length > 20 ? Text.Substring(0, 20) + "..." : Text;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum CreditsType
    {
        Logo,
        Name,
        RoleTitle,
        Text,
        Title,
        Space
    }
}