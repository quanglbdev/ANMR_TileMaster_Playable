using System.Collections;
using System.Collections.Generic;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine;

public class ThemeController : MonoBehaviour
{
    [Header("Limit Number")] [SerializeField]
    private int limitItemNumInPage;

    [Header("Content Page")] [SerializeField]
    private Transform contentPage;

    [SerializeField] private Transform contentPageParent;

    [Header("Pagination")] [SerializeField]
    private Transform pageToggle;

    [SerializeField] private Transform pageToggleParent;
    
    [SerializeField] private SimpleScrollSnap _scrollSnap;

    private int _countItemsInScrollSnap;

    private int CountItemsInScrollSnap
    {
        get
        {
            if (_countItemsInScrollSnap == 0)
            {
                _countItemsInScrollSnap = AssetManager.Instance.GetMapCount();
            }

            return _countItemsInScrollSnap;
        }
    }

    private void Start()
    {
        GeneratePagesByCount();
    }

    private void GeneratePagesByCount()
    {
        var countPage = (CountItemsInScrollSnap / 9) + 1;
        var mapData = AssetManager.Instance.mapDefinition.maps;
        List<ThemeGirdData> dataInit = new();
        for (var i = 0; i < countPage; i++)
        {
            var contentPageSpawn = Instantiate(contentPage, contentPageParent);
            contentPageSpawn.name = $"{i + 1}";
            contentPageSpawn.position = Vector3.zero;

            dataInit.Clear();
            var numOfItemInPage = i * limitItemNumInPage + limitItemNumInPage;
            for (var j = i * 9; j < numOfItemInPage; j++)
            {
                if(j >= mapData.Count - 1) continue;
                var data = mapData[j];
                if (data != null)
                {
                    dataInit.Add(new ThemeGirdData(data.mapId, data.mapName, data.background));
                }
            }

            contentPageSpawn.GetComponent<ThemePageController>().Init(dataInit);
            var localPosition = contentPageSpawn.localPosition;
            localPosition.z = 0f;
            contentPageSpawn.localPosition = localPosition;

            var pageToggleSpawn = Instantiate(pageToggle, pageToggleParent);
            pageToggleSpawn.name = $"{i + 1}";
        }

        if (_scrollSnap == null)
            _scrollSnap = GetComponent<SimpleScrollSnap>();
        
        _scrollSnap.Setup();
    }
}