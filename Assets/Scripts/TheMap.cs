using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum LootType : byte
{
    NONE = 0,
    SWORD = 1,
    SCORE = 2,
    PIRATE = 3,
    PIRATE_WITH_SWORD = 4
}

public enum GameMode : byte
{
    NONE = 0,
    MENU = 1,
    GAME = 2,
    INFO = 3
}

[System.Serializable]
public class SpriteSet
{
    public Sprite sprite;
    public bool reverse = false;
}

[System.Serializable]
public class CellData
{
    public string caption;
    public LootType loot = LootType.NONE;
    public Sprite mainSprite;
    public Sprite nextSprite;
    public SpriteSet[] sprites;
    public bool sworded = false;
}

public class TheMap : MonoBehaviour
{
    public Sprite[] data;
    public static TheMap instance;

    /// <summary>
    /// Родительский объект для отображения
    /// </summary>
    [SerializeField] private Transform parentObject;

    /// <summary>
    /// Ячейки карты в текущей ориентации
    /// </summary>
    [NonSerialized] public MapRow[] rows;

    /// <summary>
    /// Ячейки карты для ландшафта
    /// </summary>
    [SerializeField] private MapRow[] landscapeRows;

    /// <summary>
    /// Ечейки карты для портрета
    /// </summary>
    [SerializeField] private MapRow[] portraitRows;

    public CellData[] specCells;
    public Text instrText;
    public Text PlayText;
    public Player player;

    public Text livesCount;
    public Text scoreText;
    public Sword swordPrefab;
    GameMode _gameMode = GameMode.NONE;
    [NonSerialized] public byte[] map;
    static Vector2 _xy = Vector2.zero;
    public GameObject infoPanel;
    public GameObject buttons;
    public GameObject CloserPanel;

    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnInfo;

    public static int cellSize = 120;
    
    /// <summary>
    /// Изменение ориентации экрана
    /// </summary>
    void OnRectTransformDimensionsChange()
    {
        RectTransform mapTr = parentObject.GetComponent<RectTransform>();
        if (Screen.width > Screen.height)
        {
            rows = landscapeRows;
            mapTr.anchoredPosition = new Vector2(-100, 0);
            mapTr.localScale = Vector3.one;
            cellSize = 120;
        }
        else
        {
            rows = portraitRows;
            mapTr.anchoredPosition = Vector2.zero;
            mapTr.localScale = Vector3.one * 0.7f;
            cellSize = 84;
        }
    }

    

    void InitMap()
    {
        map = MapData.Map.DeepClone();
    }

    #region Modes / режимы

    public void ShowPreloader()
    {
        btnInfo.gameObject.SetActive(true);
        btnPlay.gameObject.SetActive(true);

        instrText.text = Language.Value(strings.INFO);
        PlayText.text = Language.Value(strings.PLAY);

        LoadScreen(new Vector2((map.Length / 48) - 1, 0), Vector2.zero);
        player.gameObject.SetActive(false);
        _starting = false;
        Sounds.Music();
        buttons.SetActive(false);
        CloserPanel.SetActive(false);

        _gameMode = GameMode.MENU;
    }

    public void Quit()
    {
        Application.Quit();
    }

    void ShowCloser()
    {
        CloserPanel.SetActive(true);
    }

    public void ShowInformation()
    {
        infoPanel.SetActive(true);
        parentObject.parent.gameObject.SetActive(false);
        _gameMode = GameMode.INFO;
    }

    bool _starting = false;

    /// <summary>
    /// Начало игры. Биндится на кнопку
    /// </summary>
    public void DoStartGame()
    {
        if (!_starting)
        {
            _starting = true;
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        Sounds.Play(SoundType.START);
        yield return new WaitForSeconds(2f);

        btnInfo.gameObject.SetActive(false);
        btnPlay.gameObject.SetActive(false);

        player.gameObject.SetActive(true);
        InitMap();
        LoadScreen(Vector2.zero, new Vector2(6, 4));
        player.HaveSword = true; //false;
        player.Lives = 5;
        Score = 0;
        buttons.SetActive(true);
        _gameMode = GameMode.GAME;
    }

    #endregion modes

    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 50;
        QualitySettings.maxQueuedFrames = 3;

        Language.rus = Application.systemLanguage == SystemLanguage.Russian ||
                       Application.systemLanguage == SystemLanguage.Ukrainian ||
                       Application.systemLanguage == SystemLanguage.Belarusian;
        instance = this;
        InitMap();

        btnPlay.onClick.AddListener(DoStartGame);
        btnInfo.onClick.AddListener(ShowInformation);
    }

    void Start()
    {
        ShowPreloader();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Menu))
        {
            if (_gameMode != GameMode.MENU)
                ShowPreloader();
            else
                ShowCloser();
        }

        if (_gameMode == GameMode.MENU)
        {
            foreach (KeyCode c in Player.Joystics)
                if (Input.GetKeyDown(c))
                    DoStartGame();
        }
    }

    #region map

    int offset(Vector2 pPoint)
    {
        return (int) (pPoint.x * 48 + pPoint.y * 48 * 8);
    }

    public CellData GETSpecCell(string pName)
    {
        foreach (CellData cd in specCells)
        {
            if (cd.mainSprite.name.Equals(pName))
                return cd;
        }

        return null;
    }

    public static void UpdateCell(int pNo, string pNewName, TheCell who)
    {
        instance.map[pNo] = (byte) instance.GETSpriteNo(pNewName);
        who.init(pNo);
    }

    public void LoadScreen(Vector2 pPoint, Vector2 reposPlayer)
    {
        _exSpawnPoint = _spawnPoint;
        _spawnPoint = reposPlayer;
        _exSpawnScreen = _spawnScreen;
        _spawnScreen = pPoint;
        _xy = pPoint;
        int start = offset(pPoint);
        foreach (MapRow row in rows)
        {
            foreach (TheCell i in row.columns)
            {
                if (data[map[start]] == null)
                    Debug.LogError("Empty " + map[start].ToString());
                else
                    i.init(start);
                start++;
            }
        }

        if (reposPlayer.sqrMagnitude > 0)
            Player.instance.Point = reposPlayer;

        Sword[] swords = parentObject.GetComponentsInChildren<Sword>(false);
        if (swords != null && swords.Length > 0)
            foreach (Sword sword in swords)
                Destroy(sword.gameObject);
    }

    public static bool CanStep(int pX, int pY)
    {
        if (pX < 0 || pX > 7 || pY < 0 || pY > 5)
        {
            Step(new Vector2(pX, pY));
            return false;
        }
        else
            return instance.rows[pY].columns[pX].canStep();
    }

    static Vector2 _exPoint = Vector2.zero;

    public static void Step(Vector2 pPoint)
    {
        if (_exPoint != pPoint)
        {
            _exPoint = pPoint;
            if (pPoint.y < 0)
            {
                _xy.y--;
                instance.LoadScreen(_xy, new Vector2(pPoint.x, 5));
            }
            else if (pPoint.y > 5)
            {
                _xy.y++;
                instance.LoadScreen(_xy, new Vector2(pPoint.x, 0));
            }
            else if (pPoint.x < 0)
            {
                _xy.x--;
                instance.LoadScreen(_xy, new Vector2(7, pPoint.y));
            }
            else if (pPoint.x > 7)
            {
                _xy.x++;
                instance.LoadScreen(_xy, new Vector2(0, pPoint.y));
            }
            else
                instance.rows[(int) pPoint.y].columns[(int) pPoint.x].step();
        }
    }

    #endregion map

    #region images

    int GETSpriteNo(string sName)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] != null && data[i].name == sName)
                return i;
        }

        return -1;
    }

    #endregion images

    #region GameEvents

    static Vector2 _spawnScreen, _exSpawnScreen = Vector2.zero;
    static Vector2 _spawnPoint, _exSpawnPoint = Vector2.zero;

    public static void PlayerDie()
    {
        instance.player.HaveSword = false;
        instance.player.Lives--;
        if (instance.player.Lives > 0)
            instance.LoadScreen(_exSpawnScreen, _exSpawnPoint);
        else
            instance.ShowPreloader();
    }

    int _score = 0;

    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            scoreText.text = "Score :\n" + value.ToString().PadLeft(3, '0');
        }
    }

    public void DropSword(Vector2 pPosition, Vector2 pDirection, bool isPlayer, TheCell owner)
    {
        GameObject go = Instantiate(swordPrefab.gameObject);
        go.GetComponent<RectTransform>().SetParent(parentObject);
        go.transform.localScale = Vector3.one;
        Sword sw = go.GetComponent<Sword>();
        sw.drop(pPosition, pDirection, isPlayer, owner);
    }

    #endregion
}