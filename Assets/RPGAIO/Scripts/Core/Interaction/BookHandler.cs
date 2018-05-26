using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Testing;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class BookHandler : MonoBehaviour
    {
        public static BookHandler Instance;
        public Book CurrentBook;
        public bool Reading
        {
            get {return CurrentBook != null;}
        }

        private const string AudioBookId = "rpg_audioBook";
        void Awake()
        {
            Instance = this;
        }

      
        void Update()
        {
            
        }

        public void ReadBook(Book book)
        {
            if (book == null) return;

            if (CurrentBook != null)
            {
                CloseBook();
            }

            AudioBase audioObj = null;
            if(book.BookType == BookType.Audio)
            {
                var go = AudioPlayer.Instance.Play(book.Audio, AudioType.Voice, Vector3.zero, null, AudioBookId);
                if(go != null)
                {
                    audioObj = go.GetComponent<AudioBase>();
                }
            }

            CurrentBook = book;
            if(CurrentBook != null)
            {
                CurrentBook.CurrentPage = 0;
            }
            var args = new RPGEvents.ReadBookEventArgs { Book = book, NullableAudio = audioObj };
            RPG.Events.OnReadBook(args);
        }

        public void CloseBook()
        {
            if (CurrentBook == null) return;

            if(CurrentBook.BookType == BookType.Audio)
            {
                AudioPlayer.Instance.StopSoundById(AudioBookId);
            }

            CurrentBook = null;

            var args = new RPGEvents.ClosedBookEventArgs { Book = CurrentBook };
            RPG.Events.OnClosedBook(args);
        }
    }
}