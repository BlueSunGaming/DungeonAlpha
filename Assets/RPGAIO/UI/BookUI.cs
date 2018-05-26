using System;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookUI : MonoBehaviour
{
    public BookHandler BookHandler
    {
        get { return BookHandler.Instance; }
    }
    public AudioBookModel AudioBookModel;
    public PictureBookModel PictureBookModel;
    public TextBookModel TextBookModel;

    public GameObject AudioPanel;
    public GameObject PictureBookPanel;
    public GameObject TextBookPanel;

    private Book CurrentBook;
    private AudioBase CurrentAudio;

    public void NextPage()
    {
        if(CurrentBook.CurrentPage < CurrentBook.Pages - 1)
        {
            CurrentBook.CurrentPage++;
            UpdateState(CurrentBook);
        }
        
    }

    public void PrevPage()
    {
        if(CurrentBook.CurrentPage > 0)
        {
            CurrentBook.CurrentPage--;
            UpdateState(CurrentBook);
        }
    }

    public void CloseBook()
    {
        BookHandler.CloseBook();
    }

    private void OnEnable()
    {
        RPG.Events.ReadBook += ReadBook;    
        RPG.Events.ClosedBook += ClosedBook;    
    }

    void OnDisable()
    {
        RPG.Events.ReadBook -= ReadBook;
        RPG.Events.ClosedBook -= ClosedBook; 
    }

    private void Update()
    {
        if (CurrentAudio != null)
        {
            AudioBookModel.CurrentTimeImage.fillAmount = CurrentAudio.AudioSource.time/CurrentAudio.AudioSource.clip.length;
            if(!CurrentAudio.AudioSource.isPlaying)
            {
                BookHandler.CloseBook();
            }
        }
    }

    private void ClosedBook(object sender, RPGEvents.ClosedBookEventArgs e)
    {
        AudioPanel.SetActive(BookHandler.Reading && BookHandler.CurrentBook.BookType == BookType.Audio);
        PictureBookPanel.SetActive(BookHandler.Reading && BookHandler.CurrentBook.BookType == BookType.Picture);
        TextBookPanel.SetActive(BookHandler.Reading && BookHandler.CurrentBook.BookType == BookType.Text);
        CurrentAudio = null;
    }

    private void ReadBook(object sender, RPGEvents.ReadBookEventArgs e)
    {
        var book = e.Book;
        CurrentAudio = e.NullableAudio;
        UpdateState(book);
    }

    private void UpdateState(Book book)
    {
        CurrentBook = book;
        switch (book.BookType)
        {
            case BookType.Text:
                TextBookModel.BookName.text = book.Title;
                TextBookModel.BookAuthor.text = book.Author;
                TextBookModel.PageNumber.text = book.CurrentPage +1 + "/" + book.Pages;
                TextBookModel.CurrentPageText.text = book.PageText[book.CurrentPage];
                TextBookModel.NextPageButton.interactable = book.CurrentPage < book.Pages - 1;
                TextBookModel.PrevPageButton.interactable = book.CurrentPage > 0;
                break;
            case BookType.Picture:
                PictureBookModel.BookName.text = book.Title;
                PictureBookModel.BookAuthor.text = book.Author;
                PictureBookModel.CurrentPagePicture.sprite = GeneralMethods.CreateSprite(book.Images[book.CurrentPage].Image);
                PictureBookModel.NextPageButton.interactable = book.CurrentPage < book.Pages - 1;
                PictureBookModel.PrevPageButton.interactable = book.CurrentPage > 0;
                break;
            case BookType.Audio:
                AudioBookModel.BookImage.sprite = GeneralMethods.CreateSprite(book.Image);
                AudioBookModel.BookName.text = book.Title;
                AudioBookModel.BookAuthor.text = book.Author;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        AudioPanel.SetActive(BookHandler.Reading && BookHandler.CurrentBook.BookType == BookType.Audio);
        PictureBookPanel.SetActive(BookHandler.Reading && BookHandler.CurrentBook.BookType == BookType.Picture);
        TextBookPanel.SetActive(BookHandler.Reading && BookHandler.CurrentBook.BookType == BookType.Text);
    }
}
