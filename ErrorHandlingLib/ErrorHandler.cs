using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml.Controls;

namespace ErrorHandlingLib
{
    public partial class ErrorHandler : IErrorHandling
    {
        public event DELErrorOcurrence ErrorOcurred;

        private ObservableCollection<ErrorItem> _items = new ObservableCollection<ErrorItem>();
        public ObservableCollection<ErrorItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<ErrorItem> _topItem = new ObservableCollection<ErrorItem>();
        public ObservableCollection<ErrorItem> TopItems
        {
            get { return this._topItem; }
        }

        public ErrorHandler()
        {
            Items.CollectionChanged += ItemsCollectionChanged;

        }  // of ErrorHandler()

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }

        }  // of ItemsCollectionChanged()

        public void FireErrorOcurred(string p_ExceptionMessage, string p_Location)
        {
            FireErrorOcurred(p_ExceptionMessage, p_Location, ErrorTypes.Error);

        }  // of FireErrorOcurred()

        public void FireErrorOcurred(string p_ExceptionMessage, string p_Location, ErrorTypes p_Type)
        {
            if (ErrorOcurred != null)
            {
                ErrorItem l_ErrorItem = new ErrorItem(p_ExceptionMessage
                                                      , p_Location
                                                      , DateTime.Now
                                                      , p_Type
                                                     );

                ErrorOcurred(l_ErrorItem);
            }

        }  // of FireErrorOcurred()

        public void FireErrorWithNavOcurred(string p_ExceptionMessage, string p_Location, ErrorTypes p_Type, Type p_CurrentPage, Type p_PreviousPage, Dictionary<string, string> p_FirstParamsKeyValueHolder)
        {
            if (ErrorOcurred != null)
            {
                ErrorItem l_ErrorItem = new ErrorItem(p_ExceptionMessage
                                                      , p_Location
                                                      , DateTime.Now
                                                      , p_Type
                                                      , p_CurrentPage
                                                      , p_PreviousPage
                                                      , p_FirstParamsKeyValueHolder
                                                     );

                ErrorOcurred(l_ErrorItem);
            }

        }  // of FireErrorWithNavOcurred()

    }  // of class ErrorHandler

}  // of namespace ErrorHandlingLib
