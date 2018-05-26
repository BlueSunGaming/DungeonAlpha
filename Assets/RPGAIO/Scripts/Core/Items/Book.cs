using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class Book : Item
    {
        public BookType BookType;
        public string Title ;
        public string Author ;
        public List<string> PageText ;
        public int Pages
        {
            get
            {
                switch(BookType)
                {
                    case BookType.Text:
                        return PageText.Count;
                    case BookType.Picture:
                        return Images.Count;
                    case BookType.Audio:
                        return 1;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public int CurrentPage ;

        public RPGVector2 ImageSize;
        public List<ImageContainer> Images;
        
        public string AudioPath;
        [JsonIgnore]
        public AudioClip _audio ;
        [JsonIgnore]
        public AudioClip Audio
        {
            get { return _audio ?? (_audio = Resources.Load(AudioPath) as AudioClip); }
            set { _audio = value; }
        }

        public Book()
        {
            ImageSize = new RPGVector2(720,480);
            PageText = new List<string>(){"Page 1"};
            Images = new List<ImageContainer>(){ new ImageContainer()};
            ItemType = ItemType.Book;
            Author = "";
        }
    }

    public enum BookType
    {
        Text,
        Picture,
        Audio
    }
}