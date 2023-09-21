using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NativeAdPanel : MonoBehaviour
{/*
    [SerializeField] Image adIcon;
    [SerializeField] Image adChoices;
    [SerializeField] Text adHeadline;
    [SerializeField] Text adCallToAction;
    [SerializeField] Button btnAdCallToAction;
    [SerializeField] Text adAdvertiser;
    [SerializeField] Image adImage;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetInitNativeAd(UnifiedNativeAd adNative) {
        gameObject.SetActive(true);
        Texture2D iconTexture = adNative.GetIconTexture();
        Texture2D iconAdChoices = adNative.GetAdChoicesLogoTexture();
        string headline = adNative.GetHeadlineText();
        string cta = adNative.GetCallToActionText();
        string advertiser = adNative.GetBodyText();
        Texture2D iamgeTexture = adNative.GetImageTextures()[0];
        if (iconTexture != null)
        {
            adIcon.sprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height),new Vector2(0.5f, 0.5f));
            adIcon.preserveAspect = true;
            adIcon.gameObject.SetActive(true);
        }
        else {
            adIcon.gameObject.SetActive(false);
        }

        if (iconAdChoices != null)
        {
            adChoices.sprite = Sprite.Create(iconAdChoices, new Rect(0, 0, iconAdChoices.width, iconAdChoices.height), new Vector2(0.5f, 0.5f));
            adChoices.preserveAspect = true;
            adChoices.gameObject.SetActive(true);
        }
        else {
            adChoices.gameObject.SetActive(false);
        }

        adHeadline.text = headline;
        adAdvertiser.text = advertiser;
        adCallToAction.text = cta;

        if (iamgeTexture != null)
        {
            adImage.sprite = Sprite.Create(iamgeTexture, new Rect(0, 0, iamgeTexture.width, iamgeTexture.height), new Vector2(0.5f, 0.5f));
            adImage.preserveAspect = true;
            adImage.gameObject.SetActive(true);
        }
        else
        {
            adImage.gameObject.SetActive(false);
        }


        
        //
        if (adIcon.isActiveAndEnabled)
        {
            adIcon.gameObject.GetComponent<BoxCollider2D>().size = adIcon.gameObject.GetComponent<RectTransform>().sizeDelta;
        }
        if (adChoices.isActiveAndEnabled)
        {
            adChoices.gameObject.GetComponent<BoxCollider2D>().size = adChoices.gameObject.GetComponent<RectTransform>().sizeDelta;
        }
        adHeadline.gameObject.GetComponent<BoxCollider2D>().size = adHeadline.gameObject.GetComponent<RectTransform>().sizeDelta;
        adAdvertiser.gameObject.GetComponent<BoxCollider2D>().size = adAdvertiser.gameObject.GetComponent<RectTransform>().sizeDelta;
        btnAdCallToAction.gameObject.GetComponent<BoxCollider2D>().size = btnAdCallToAction.gameObject.GetComponent<RectTransform>().sizeDelta;
        if (adImage.isActiveAndEnabled)
        {
            adImage.gameObject.GetComponent<BoxCollider2D>().size = adImage.gameObject.GetComponent<RectTransform>().sizeDelta;
        }


        //register gameobjects
        if (adIcon.isActiveAndEnabled)
        {
            if (!adNative.RegisterIconImageGameObject(adIcon.gameObject))
            {
                Debug.Log("Failed to register Icon Image Game Object");
            }
        }
        if (adChoices.isActiveAndEnabled)
        {
            if (!adNative.RegisterAdChoicesLogoGameObject(adChoices.gameObject))
            {
                Debug.Log("Failed to register Ad Choices Logo Game Object");
            }
        }

        if (!adNative.RegisterHeadlineTextGameObject(adHeadline.gameObject)) {
            Debug.Log("Failed to register Headline Text Game Object");
        }

        if (!adNative.RegisterCallToActionGameObject(btnAdCallToAction.gameObject)) {
            Debug.Log("Failed to register Call To Action Game Object");
        }

        if (adNative.RegisterBodyTextGameObject(adAdvertiser.gameObject)) {
            Debug.Log("Failed to register Body Text Game Object");
        }
        if (adImage.isActiveAndEnabled)
        {
            adNative.RegisterImageGameObjects(new List<GameObject>() { adImage.gameObject });
        }

    }*/
}
