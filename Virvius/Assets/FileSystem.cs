using UnityEngine;
using UnityEngine.UI;
using Rewired;
using System.Text;

public class FileSystem : MonoBehaviour
{
    public static FileSystem fileSystem;
    private StringBuilder sb = new StringBuilder();
    private GameSystem gameSystem;
    private Player inputPlayer;
    [SerializeField]
    private Image[] windowTitle = new Image[2];
    [SerializeField]
    private GameObject fileSelection;
    [SerializeField]
    private GameObject fileMenu;
    [SerializeField]
    private GameObject[] fileMenus = new GameObject[2];
    [SerializeField]
    private Selectable[] firstSelection = new Selectable[3];
    private int fileIndex = 0;
    
    public bool[] slotSelected = new bool[9];
    [SerializeField]
    private ButtonAnimate[] buttonSAnimates = new ButtonAnimate[8];
    [SerializeField]
    private ButtonAnimate[] buttonLAnimates = new ButtonAnimate[9];
    [SerializeField]
    private Text[] saveTexts = new Text[8];
    [SerializeField]
    private Text[] loadTexts = new Text[9];
    private void Awake()
    {
        fileSystem = this;
        gameSystem = GameSystem.gameSystem;
    }
    void Start()
    {
        inputPlayer = ReInput.players.GetPlayer(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameSystem.fileMenuOpen) return;
        if (Input.GetKeyUp(KeyCode.Escape) || inputPlayer.GetButtonUp("B")) OpenFileMenu(false);
    }
    private void OpenFile(int ID)
    {
        gameSystem.LoadFileInformation(saveTexts, loadTexts);
        for (int m = 0; m < fileMenus.Length; m++)
        {
            if (m == ID)
            {
                fileMenu.SetActive(true);
                fileMenus[m].SetActive(true);
                windowTitle[m].enabled = true;
                firstSelection[m].Select();
            }
            else { windowTitle[m].enabled = false; fileMenus[m].SetActive(false); }
        }
    }
    public void SetButtonAnimates(int ID)
    {
        if (fileIndex == 0)
        {
            for (int b = 0; b < buttonSAnimates.Length; b++)
            {
                if (b == ID) buttonSAnimates[b].StartAnimation(true);
                else buttonSAnimates[b].StartAnimation(false);
            }
        }
        else
        {
            for (int b = 0; b < buttonLAnimates.Length; b++)
            {
                if (b == ID) buttonLAnimates[b].StartAnimation(true);
                else buttonLAnimates[b].StartAnimation(false);
            }
        }
    }
    private void CloseFile()
    {
        for (int s = 0; s < slotSelected.Length; s++)
            slotSelected[s] = false;
        for (int m = 0; m < fileMenus.Length; m++) fileMenus[m].SetActive(false); 
        fileMenu.SetActive(false); 

        firstSelection[2].Select(); 
    }
    public void HighlightSelectedFile(int index)
    {
        fileIndex = index;
    }
    public void OpenFileMenu(bool active)
    {
        if (gameSystem == null) gameSystem = GameSystem.gameSystem;
        fileSelection.SetActive(!active);
        if (active) OpenFile(fileIndex);
        else CloseFile();
        gameSystem.fileMenuOpen = active;
    }
    public void HighlightSelectedSlot(int index)
    {
        for (int s = 0; s < slotSelected.Length; s++)
            if (s == index) slotSelected[s] = true;
        else slotSelected[s] = false;
    }
    public void FileAccess()
    {
        if (gameSystem == null) gameSystem = GameSystem.gameSystem;
        for(int s = 1; s < slotSelected.Length; s++)
        {
            if (slotSelected[s])
            {
                if (fileIndex == 0)
                {
                    gameSystem.SaveData(s);
                    gameSystem.LoadFileInformation(saveTexts, loadTexts);
                }
                else 
                {
                    OpenFileMenu(false);
                    gameSystem.LoadData(s); 
                }
                return;
            }
        }
    }
    public void LoadAutoSave()
    {
        OpenFileMenu(false);
        gameSystem.LoadData(0);
    }
}
