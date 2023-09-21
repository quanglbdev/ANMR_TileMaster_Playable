using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BuildingNotification : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hammer;
    [SerializeField] private GameObject notification;

    private void Start()
    {
        UpdateHammer();
        EventDispatcher.Instance.RegisterListener(EventID.UpdateHammer, _ => { UpdateHammer(); });
    }

    private void UpdateHammer()
    {
        var count = 0;
        var map = AssetManager.Instance.GetMapDefinition(Config.MAP_INDEX);
        var elements = map.elements;
        var listPrice = new List<int>();
        foreach (var element in elements)
        {
            var levelOfElement = Config.GetCurrentElement((int)element.buildingType);
            var price = element.price;
            if (levelOfElement < price.Count)
            {
                if (levelOfElement > 0)
                    listPrice.Add(price[levelOfElement]);
                else if (levelOfElement == 0)
                    listPrice.Add(1);
            }
        }

        foreach (var price in listPrice)
        {
            if (price <= Config.currHammer)
            {
                count++;
            }
        }

        var minValue = listPrice.Min();
        if (Config.currHammer > 0
            && Config.currLevel >= Config.LEVEL_UNLOCK_BUILDING
            && Config.currHammer >= minValue)
        {
            //_hammer.text = $"{count}";
            _hammer.text = "!";
            notification.gameObject.SetActive(true);
        }
        else
            notification.gameObject.SetActive(false);
    }
}