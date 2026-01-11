// InteractionMenuUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InteractionMenuUI : MonoBehaviour
{
    public static InteractionMenuUI Instance;

    [Header("UI refs")]
    public CanvasGroup root; // панель меню (CanvasGroup на панели)
    public TextMeshProUGUI titleText;   // TMP заголовок
    public Button buttonOption1;        // Move Here
    public Button buttonOption2;        // Teleport to Scene (может быть скрыта)
    public Button buttonCancel;

    // internal callbacks
    Action onOption1;
    Action onOption2;
    Action onCancel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (root == null) root = GetComponent<CanvasGroup>();
        SetVisible(false);
    }

    // Показывает меню. Если option2Label==null, вторую кнопку скрываем.
    public void Show(string title, string option1Label, Action option1Callback,
                     string option2Label = null, Action option2Callback = null, Action cancelCallback = null)
    {
        if (titleText != null) titleText.text = title;
        if (buttonOption1 != null) buttonOption1.GetComponentInChildren<TextMeshProUGUI>().text = option1Label;

        onOption1 = option1Callback;

        if (!string.IsNullOrEmpty(option2Label) && option2Callback != null)
        {
            buttonOption2.gameObject.SetActive(true);
            buttonOption2.GetComponentInChildren<TextMeshProUGUI>().text = option2Label;
            onOption2 = option2Callback;
        }
        else
        {
            if (buttonOption2 != null) buttonOption2.gameObject.SetActive(false);
            onOption2 = null;
        }

        onCancel = cancelCallback;

        // clear old listeners to avoid duplication
        buttonOption1.onClick.RemoveAllListeners();
        buttonOption1.onClick.AddListener(() => { onOption1?.Invoke(); Hide(); });

        if (buttonOption2 != null && buttonOption2.gameObject.activeSelf)
        {
            buttonOption2.onClick.RemoveAllListeners();
            buttonOption2.onClick.AddListener(() => { onOption2?.Invoke(); Hide(); });
        }

        buttonCancel.onClick.RemoveAllListeners();
        buttonCancel.onClick.AddListener(() => { onCancel?.Invoke(); Hide(); });

        SetVisible(true);
    }

    public void Hide()
    {
        SetVisible(false);
    }

    void SetVisible(bool v)
    {
        if (root != null)
        {
            root.alpha = v ? 1f : 0f;
            root.interactable = v;
            root.blocksRaycasts = v;
            root.gameObject.SetActive(v);
        }
    }
}