﻿using System.Collections.Generic;
using System.ComponentModel;
using Apitron.PDF.Kit;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Navigation;

namespace Apitron.WpfPdfViewer.ViewModels
{
    public class DocumentViewModel : INotifyPropertyChanged
    {
        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        private Document document;
        private FixedDocument fixedDocument;
        private Dictionary<Page, PageViewModel> pageViewCache;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the document object used for rendering.
        /// </summary>
        /// <value> The document. </value>
        public Document Document
        {
            get { return this.document; }
            set
            {
                if (this.document == value)
                {
                    return;
                }

                if (this.document != null)
                {
                    this.document.Navigator.Navigated -= this.OnNavigated;
                }

                this.pageViewCache = new Dictionary<Page, PageViewModel>();

                this.document = value;
                this.document.Navigator.Navigated += this.OnNavigated;
                this.OnPropertyChanged("Document");
                this.OnPropertyChanged("Bookmark");
                this.OnPropertyChanged("Page");
                this.OnPropertyChanged("PageViewModel");
                this.OnPropertyChanged("Pages");
                this.OnPropertyChanged("Title");
                this.OnPropertyChanged("Links");
            }
        }

        /// <summary>
        /// Gets or sets the document object used for manipulation.
        /// </summary>
        public FixedDocument FixedDocument
        {
            get { return this.fixedDocument; }
            set { this.fixedDocument = value; }
        }

        /// <summary>
        /// Gets the page object that can be used for rendering.
        /// </summary>
        public Page Page
        {
            get
            {
                if (this.document == null)
                {
                    return null;
                }
                return this.document.Navigator.CurrentPage;
            }
            set { this.document.Navigator.GoToPage(value); }
        }

        /// <summary>
        /// Gets the page object that can be used for manipulation.
        /// </summary>
        /// <value>
        /// The kit page.
        /// </value>
        public Apitron.PDF.Kit.FixedLayout.Page NativePage
        {
            get
            {
                if (this.Page != null)
                {
                    return this.FixedDocument.Pages[this.Document.Pages.IndexOf(this.Page)];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the page view model(rendering).
        /// </summary>
        /// <value>
        /// The page view model.
        /// </value>
        public PageViewModel PageViewModel
        {
            get
            {
                PageViewModel result = null;
                if (this.Page != null)
                {
                    if (!pageViewCache.TryGetValue(this.Page, out result))
                    {
                        result = new PageViewModel(this.Page, this.NativePage);
                        pageViewCache.Add(this.Page, result);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the collection of pages that can be used for rendering.
        /// </summary>
        public PageCollection Pages
        {
            get
            {
                if (this.document == null)
                {
                    return null;
                }

                return this.document.Pages;
            }
        }

        /// <summary>
        /// Gets the root bookmark of the document.
        /// </summary>
        public Bookmark Bookmark
        {
            get
            {
                if (this.document == null)
                {
                    return null;
                }
                return this.document.Bookmarks;
            }
        }

        /// <summary>
        /// Gets the document's title.
        /// </summary>
        public string Title
        {
            get
            {
                if (this.document == null)
                {
                    return null;
                }
                if (this.document.DocumentInfo.Title == string.Empty)
                {
                    return "WPF Viewer";
                }
                return this.document.DocumentInfo.Title;
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        ///   Navigators the on page changed.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="eventArgs"> The <see cref="NavigatedEventArgs" /> instance containing the event data. </param>
        private void OnNavigated(object sender, NavigatedEventArgs eventArgs)
        {
            this.OnPropertyChanged("Page");
            this.OnPropertyChanged("PageViewModel");
            this.OnPropertyChanged("Links");
        }

        /// <summary>
        ///   Called when [property changed].
        /// </summary>
        /// <param name="propertyName"> Name of the property. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}