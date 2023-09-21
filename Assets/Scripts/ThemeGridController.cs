/*
 * * * * This bare-bones script was auto-generated * * * *
 * The code commented with "/ * * /" demonstrates how data is retrieved and passed to the adapter, plus other common commands. You can remove/replace it once you've got the idea
 * Complete it according to your specific use-case
 * Consult the Example scripts if you get stuck, as they provide solutions to most common scenarios
 * 
 * Main terms to understand:
 *		Model = class that contains the data associated with an item (title, content, icon etc.)
 *		Views Holder = class that contains references to your views (Text, Image, MonoBehavior, etc.)
 * 
 * Default expected UI hiererchy:
 *	  ...
 *		-Canvas
 *		  ...
 *			-MyScrollViewAdapter
 *				-Viewport
 *					-Content
 *				-Scrollbar (Optional)
 *				-ItemPrefab (Optional)
 * 
 * Note: If using Visual Studio and opening generated scripts for the first time, sometimes Intellisense (autocompletion)
 * won't work. This is a well-known bug and the solution is here: https://developercommunity.visualstudio.com/content/problem/130597/unity-intellisense-not-working-after-creating-new-1.html (or google "unity intellisense not working new script")
 * 
 * 
 * Please read the manual under "/Docs", as it contains everything you need to know in order to get started, including FAQ
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.DataHelpers;

// You should modify the namespace to your own or - if you're sure there won't ever be conflicts - remove it altogether
namespace Your.Namespace.Here.UniqueStringHereToAvoidNamespaceConflicts.Grids
{
    // There is 1 important callback you need to implement, apart from Start(): UpdateCellViewsHolder()
    // See explanations below
    public class ThemeGridController : GridAdapter<GridParams, ThemeGridController.BackgroundItemViewsHolder>
    {
        // Helper that stores data and notifies the adapter when items count changes
        // Can be iterated and can also have its elements accessed by the [] operator
        private SimpleDataHelper<BackgroundItemModel> Data { get; set; }


        #region GridAdapter implementation

        protected override void Start()
        {
            Data = new SimpleDataHelper<BackgroundItemModel>(this);

            // Calling this initializes internal data and prepares the adapter to handle item count changes
            base.Start();

            RetrieveDataAndUpdate(AssetManager.Instance.GetMapCount());
            Restart();
        }
        
        public void Restart()
        {
            Data.ResetItems(Data.ToList());
            Refresh();
        }

        // This is called anytime a previously invisible item become visible, or after it's created, 
        // or when anything that requires a refresh happens
        // Here you bind the data from the model to the item's views
        // *For the method's full description check the base implementation
        protected override void UpdateCellViewsHolder(BackgroundItemViewsHolder newOrRecycled)
        {
            var model = Data[newOrRecycled.ItemIndex];

            // newOrRecycled.backgroundGridItem.GetComponent<ThemeGridItem>()
            //     .Init(model.index, model.areaName, model.sprite, model.controller);
        }

        // This is the best place to clear an item's views in order to prepare it from being recycled, but this is not always needed, 
        // especially if the views' values are being overwritten anyway. Instead, this can be used to, for example, cancel an image 
        // download request, if it's still in progress when the item goes out of the viewport.
        // <newItemIndex> will be non-negative if this item will be recycled as opposed to just being disabled
        // *For the method's full description check the base implementation
        /*
        protected override void OnBeforeRecycleOrDisableCellViewsHolder(MyGridItemViewsHolder inRecycleBinOrVisible, int newItemIndex)
        {
            base.OnBeforeRecycleOrDisableCellViewsHolder(inRecycleBinOrVisible, newItemIndex);
        }
        */

        #endregion

        // These are common data manipulation methods
        // The list containing the models is managed by you. The adapter only manages the items' sizes and the count
        // The adapter needs to be notified of any change that occurs in the data list. 
        // For GridAdapters, only Refresh and ResetItems work for now

        #region data manipulation

        public void AddItemsAt(int index, IList<BackgroundItemModel> items)
        {
            //Commented: this only works with Lists. ATM, Insert for Grids only works by manually changing the list and calling NotifyListChangedExternally() after
            //Data.InsertItems(index, items);
            Data.List.InsertRange(index, items);
            Data.NotifyListChangedExternally();
        }

        public void RemoveItemsFrom(int index, int count)
        {
            //Commented: this only works with Lists. ATM, Remove for Grids only works by manually changing the list and calling NotifyListChangedExternally() after
            //Data.RemoveRange(index, count);
            Data.List.RemoveRange(index, count);
            Data.NotifyListChangedExternally();
        }

        public void SetItems(IList<BackgroundItemModel> items)
        {
            Data.ResetItems(items);
        }

        #endregion


        // Here, we're requesting <count> items from the data source
        void RetrieveDataAndUpdate(int count)
        {
            StartCoroutine(FetchMoreItemsFromDataSourceAndUpdate(count));
        }

        // Retrieving <count> models from the data source and calling OnDataRetrieved after.
        // In a real case scenario, you'd query your server, your database or whatever is your data source and call OnDataRetrieved after
        IEnumerator FetchMoreItemsFromDataSourceAndUpdate(int count)
        {
            Data.List.Clear();
            // Simulating data retrieving delay
            yield return new WaitForSeconds(.5f);

            var newItems = new BackgroundItemModel[count];

            for (var i = 0; i < count; ++i)
            {
                var map = AssetManager.Instance.GetMapDefinition(i + 1);
                var model = new BackgroundItemModel()
                {
                    index =  i + 1,
                    areaName = map.mapName,
                    sprite = map.background,
                    controller =  this
                };
                newItems[i] = model;


            }
            OnDataRetrieved(newItems);

            void OnDataRetrieved(BackgroundItemModel[] newItems)
            {
                //Commented: this only works with Lists. ATM, Insert for Grids only works by manually changing the list and calling NotifyListChangedExternally() after
                // Data.InsertItemsAtEnd(newItems);

                Data.List.AddRange(newItems);
                Data.NotifyListChangedExternally();
            }
        }


        // Class containing the data associated with an item
        public class BackgroundItemModel
        {
            public int index;
            public string areaName;
            public Sprite sprite;
            public ThemeGridController controller;
        }


        // This class keeps references to an item's views.
        // Your views holder should extend BaseItemViewsHolder for ListViews and CellViewsHolder for GridViews
        // The cell views holder should have a single child (usually named "Views"), which contains the actual 
        // UI elements. A cell's root is never disabled - when a cell is removed, only its "views" GameObject will be disabled
        public class BackgroundItemViewsHolder : CellViewsHolder
        {
            public Component backgroundGridItem;

            public override void CollectViews()
            {
                base.CollectViews();

                views.GetComponentAtPath("ThemeGridItem", out backgroundGridItem);
            }

        }
    }
}