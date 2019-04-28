using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class ButtonDescription
{
    public ButtonDescription(string name, UnityAction unityAction)
    {
        Name = name;
        OnClickUnityAction = unityAction;
    }

    public readonly string Name;
    public readonly UnityAction OnClickUnityAction;
}

public class MenuController : MonoBehaviour
{
    public GameObject ButtonExampleGameObject;
    public float ButtonHeightInterval = 0.0f;
    public bool IsDebug = false;

    private readonly List<GameObject> createdButtons = new List<GameObject>();

    private enum MenuState {
        ShowMenu,
        ShowMission,
    }

    private MenuState _menuState = MenuState.ShowMenu;

    private void Awake()
    {
        if (IsDebug)
            OnLoadGameButtonClick();

        ButtonExampleGameObject.SetActive(false);
        CreateButtons();
    }

    private void CreateButtons()
    {
        foreach (var createdButton in createdButtons)
            Destroy(createdButton);
        createdButtons.Clear();

        var buttonDescriptions = new List<ButtonDescription>();

        switch (_menuState) {
            case MenuState.ShowMenu:
                buttonDescriptions.Add(new ButtonDescription("Новая игра", OnNewGameButtonClick));
                buttonDescriptions.Add(new ButtonDescription("Загрузить", OnLoadGameButtonClick));
                buttonDescriptions.Add(new ButtonDescription("Выход", OnExitButtonClick));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var buttonTransform = (RectTransform)ButtonExampleGameObject.transform;
        var initialButtonPosition = buttonTransform.position;
        var buttonCount = buttonDescriptions.Count;
        var baseOffsetY = buttonTransform.rect.height * (buttonCount / 2.0f) + ButtonHeightInterval * (buttonCount - 1) / 2.0f;
        for (var i = 0; i < buttonDescriptions.Count; ++i) {
            var buttonDescription = buttonDescriptions[i];

            var button = Instantiate(ButtonExampleGameObject, gameObject.transform);
            var offsetY = baseOffsetY - i * (buttonTransform.rect.height + ButtonHeightInterval);
            button.transform.position = new Vector3(initialButtonPosition.x, initialButtonPosition.y + offsetY, initialButtonPosition.z);
            button.GetComponent<Button>().onClick.AddListener(delegate { PrepareButtonClick(); buttonDescription.OnClickUnityAction(); });

            var buttonTextComponent = button.GetComponentInChildren<Text>();
            Debug.Assert(buttonTextComponent != null);
            buttonTextComponent.text = buttonDescriptions[i].Name;

            button.SetActive(true);
            createdButtons.Add(button);
        }
    }

    private void PrepareButtonClick()
    {

    }

    private void OnNewGameButtonClick()
    {
        GameState.readInputFileIfExists = false;
        SceneManager.LoadScene("scenes/main");
    }

    private void OnLoadGameButtonClick()
    {
        string filename = null;
        foreach (var path in Directory.GetFiles("."))
            if (path == ".\\input.txt" || path == ".\\input01.txt")
                filename = path;

        if (filename != null) {
            GameState.inputFileName = filename;
            GameState.readInputFileIfExists = true;
        }

        SceneManager.LoadScene("scenes/main");
    }

    private void OnExitButtonClick()
    {
        Application.Quit();
    }
}
