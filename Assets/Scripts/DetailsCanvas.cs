using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Text;
using Newtonsoft.Json;
using System;

public class DetailsCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Input Field")]
    [SerializeField] private TMP_InputField descriptionInputField;
    [SerializeField] private GameObject scroolBar;
    private float inputFieldTextLength;

    [Space]
    [SerializeField] private Image qrCode;

    [Space]
    [SerializeField] private Image colorImage;

    [Space]
    [SerializeField] private ToogleSwitchButton toogleSwitchButton;

    private bool scroolBarStatus;

    private const string serverURI = "https://pusbkbbia3.execute-api.us-east-1.amazonaws.com/default/get_cat";

    private void Awake()
    {
        inputFieldTextLength = descriptionInputField.gameObject.GetComponent<RectTransform>().rect.height;
    }
    public void OnGetCatButtonClicked()
    {
        StartCoroutine(GetDetailsHandler());
    }
    public IEnumerator GetDetailsHandler()
    {
        //creating a body for the post request
        var catDetailsRequest = new CatDetailsRequest("Jenya");
        var catDetailsRequestJson = JsonConvert.SerializeObject(catDetailsRequest);
        
        /*
         * Creating a Unity Web Request client
         * Setting the content type as JSON
         * Sending the body as raw data to the endpoint
         */
        UnityWebRequest webRequest = new UnityWebRequest(serverURI, "POST");
        webRequest.SetRequestHeader("Content-Type", "application/json");
        byte[] rawCatRequestJson = Encoding.UTF8.GetBytes(catDetailsRequestJson);
        webRequest.uploadHandler = new UploadHandlerRaw(rawCatRequestJson);
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        yield return webRequest.SendWebRequest();
     
        //handling the response status and data
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            webRequest.Dispose();
            Debug.LogError("Error while getting data from the server");
        }
        else
        {
            var response = webRequest.downloadHandler.text;
            webRequest.Dispose();

            try
            {
                //trying to deserialize the json string to CatDetailsResponse
                var catDetailsResponse = JsonConvert.DeserializeObject<CatDetailsResponse>(response);

                //sending the new data that received from the server to update the UI components
                UIUpdate(catDetailsResponse);
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
    private void UIUpdate(CatDetailsResponse catDetailsResponse)
    {
        Color newColor;

        nameText.text = catDetailsResponse.name;

        InputFieldHandler(catDetailsResponse.description);

        QRCodeImageHandler(catDetailsResponse.qr_code);

        if (ColorUtility.TryParseHtmlString($"#{catDetailsResponse.color}", out newColor))
        {
            colorImage.color = newColor;
        }

        toogleSwitchButton.OnButtonToogle(catDetailsResponse.enable);

    }
    private void InputFieldHandler(string receivedDescription)
    {
        scroolBar.GetComponent<Scrollbar>().value = 0;
        
        descriptionInputField.text = receivedDescription;
        scroolBarStatus = descriptionInputField.text.Length > inputFieldTextLength ? true : false;
        scroolBar.SetActive(scroolBarStatus);
    }
    private void QRCodeImageHandler(string receivedQRCode)
    {
        byte[] imageBytes = Convert.FromBase64String(receivedQRCode);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageBytes);
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
        qrCode.sprite = sprite;
        qrCode.color = Color.white;
    }
}
