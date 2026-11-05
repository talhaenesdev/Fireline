using System;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.ConsoleModule;
using FlowIoC.ScreenModule.ViewsMediators.Screen;
using Modules.LobbyModule.Shared.Data.ValueObjects;
using Modules.LobbyModule.Shared.Enums;
using Modules.LocalizationModule.Entities;
using Modules.LocalizationModule.Enums;
using Modules.MatchModule.Shared.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.LobbyModule.LobbyScreenModule.ViewsMediators
{
    /// <summary>
    /// The lobby: name, code, players, and the host's choice of mode, map and difficulty. The prefab's
    /// buttons and fields are kept and moved into a layout built at runtime from the sprites under
    /// Resources/UI, so the prefab needs no new objects. The layout is drawn on a 1600x900 board that
    /// is scaled to fit whatever canvas the screen lands on.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class LobbyScreenView : ScreenView
    {
        private const string SPRITES = "UI/SPR_Lobby_";
        private const float BOARD_WIDTH = 1600f;
        private const float BOARD_HEIGHT = 900f;
        private const int SLOTS = 4;

        private static readonly Color Orange = new(1f, 0.54f, 0.12f);
        private static readonly Color Cyan = new(0.31f, 0.89f, 1f);
        private static readonly Color Muted = new(0.62f, 0.66f, 0.72f);
        private static readonly Color Dimmed = new(0.38f, 0.38f, 0.42f, 1f);

        [Header("Buttons")]
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _startGameButton;

        [Header("Input")]
        [SerializeField] private TMP_InputField _joinCodeInput;
        [SerializeField] private TMP_InputField _playerNameInput;

        [Header("Texts")]
        [SerializeField] private TMP_Text _sessionCodeText;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private TMP_Text _hostStatusText;
        [SerializeField] private TMP_Text _playerCountText;
        [SerializeField] private GameObject _readyMark;

        public Action<string> CreateClicked;
        public Action<string, string> JoinClicked;
        public Action LeaveClicked;
        public Action ReadyClicked;
        public Action StartClicked;
        public Action ModeClicked;
        public Action DifficultyClicked;
        public Action MapClicked;
        public Action LanguageClicked;

        /// <summary>Filled by the Mediator; every text on the board is looked up through it.</summary>
        public TextLocalizer Localizer { get; } = new();

        private RectTransform _board;

        private Button _deathmatchCard;
        private Button _zombieCard;
        private Image _deathmatchArt;
        private Image _zombieArt;
        private RectTransform _selectedFrame;

        private GameObject _mapSection;
        private Image _mapArt;
        private TMP_Text _mapTitle;
        private TMP_Text _mapDescription;
        private Button _mapLeft;
        private Button _mapRight;
        private Button _mapCard;

        private GameObject _difficultySection;
        private Image _difficultyBar;
        private TMP_Text _difficultyText;
        private Button _difficultyLeft;
        private Button _difficultyRight;
        private Button _difficultyButton;

        private GameObject _deathmatchNote;
        private GameObject _lockRow;
        private Button _copyButton;
        private Image _readyImage;
        private TMP_Text _readyLabel;
        private readonly Image[] _slots = new Image[SLOTS];
        private readonly TMP_Text[] _slotTexts = new TMP_Text[SLOTS];
        private readonly Image[] _slotTicks = new Image[SLOTS];

        private Sprite _primary;
        private Sprite _secondary;
        private Sprite _readySelected;
        private Sprite _difficultyDefault;
        private Sprite _difficultyNightmare;
        private Sprite _mapArena;
        private Sprite _mapBase;

        private Button _languageButton;
        private TMP_Text _languageText;

        private GameMode _mode;
        private string _joinCode = string.Empty;
        private LobbyStateVO _state;
        private bool _hasState;

        private void Awake()
        {
            _primary = Load("Button_Primary");
            _secondary = Load("Button_Secondary");
            _readySelected = Load("Ready_Selected");
            _difficultyDefault = Load("Difficulty_Default");
            _difficultyNightmare = Load("Difficulty_Nightmare");
            _mapArena = Load("Map_ZombieArena");
            _mapBase = Load("Map_BaseDefense");

            // The prefab's own panel goes; its controls move onto the board.
            Transform oldPanel = transform.GetChild(0);
            BuildBackground();
            BuildBoard();
            BuildLeftColumn();
            BuildModes();
            BuildMap();
            BuildDifficulty();
            BuildBottomBar();
            BuildLanguageButton();
            oldPanel.gameObject.SetActive(false);
            Localizer.Changed += Redraw;

            if (_readyMark != null)
                _readyMark.SetActive(false);
        }

        private void OnEnable()
        {
            _createButton.onClick.AddListener(OnCreateClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
            _leaveButton.onClick.AddListener(OnLeaveClicked);
            _readyButton.onClick.AddListener(OnReadyClicked);
            _startGameButton.onClick.AddListener(OnStartClicked);
            _deathmatchCard.onClick.AddListener(OnDeathmatchClicked);
            _zombieCard.onClick.AddListener(OnZombieClicked);
            _mapLeft.onClick.AddListener(OnMapClicked);
            _mapRight.onClick.AddListener(OnMapClicked);
            _mapCard.onClick.AddListener(OnMapClicked);
            _difficultyLeft.onClick.AddListener(OnDifficultyClicked);
            _difficultyRight.onClick.AddListener(OnDifficultyClicked);
            _difficultyButton.onClick.AddListener(OnDifficultyClicked);
            _copyButton.onClick.AddListener(OnCopyClicked);
            _languageButton.onClick.AddListener(OnLanguageClicked);
        }

        private void OnDisable()
        {
            _createButton.onClick.RemoveListener(OnCreateClicked);
            _joinButton.onClick.RemoveListener(OnJoinClicked);
            _leaveButton.onClick.RemoveListener(OnLeaveClicked);
            _readyButton.onClick.RemoveListener(OnReadyClicked);
            _startGameButton.onClick.RemoveListener(OnStartClicked);
            _deathmatchCard.onClick.RemoveListener(OnDeathmatchClicked);
            _zombieCard.onClick.RemoveListener(OnZombieClicked);
            _mapLeft.onClick.RemoveListener(OnMapClicked);
            _mapRight.onClick.RemoveListener(OnMapClicked);
            _mapCard.onClick.RemoveListener(OnMapClicked);
            _difficultyLeft.onClick.RemoveListener(OnDifficultyClicked);
            _difficultyRight.onClick.RemoveListener(OnDifficultyClicked);
            _difficultyButton.onClick.RemoveListener(OnDifficultyClicked);
            _copyButton.onClick.RemoveListener(OnCopyClicked);
            _languageButton.onClick.RemoveListener(OnLanguageClicked);
        }

        // The board keeps its proportions and fits inside the screen, whatever the canvas scaler says.
        private void LateUpdate()
        {
            if (_board == null || transform is not RectTransform area)
                return;

            Vector2 size = area.rect.size;
            if (size.x <= 0f || size.y <= 0f)
                return;

            float scale = Mathf.Min(size.x / BOARD_WIDTH, size.y / BOARD_HEIGHT);
            _board.localScale = Vector3.one * scale;
        }

        public void ShowPlayerName(string playerName) => _playerNameInput.text = playerName;

        /// <summary>The language in use, marked on the language button.</summary>
        public void ShowLanguage(Language language) =>
            _languageText.text = language == Language.Turkish
                ? "<color=#FF8A1F>TR</color>  <color=#7A808A>EN</color>"
                : "<color=#7A808A>TR</color>  <color=#FF8A1F>EN</color>";

        private void Redraw()
        {
            if (_hasState)
                ShowState(_state);
        }

        private string L(string text) => Localizer.Get(text);

        public void ShowState(LobbyStateVO state)
        {
            _state = state;
            _hasState = true;
            _mode = state.Mode;
            _joinCode = state.HasSession ? state.JoinCode : string.Empty;

            _readyButton.interactable = !state.IsBusy && state.HasSession;
            _createButton.interactable = !state.IsBusy && !state.HasSession;
            _joinButton.interactable = !state.IsBusy && !state.HasSession;

            _leaveButton.gameObject.SetActive(state.HasSession);
            _leaveButton.interactable = !state.IsBusy;

            _startGameButton.gameObject.SetActive(state.IsHost);
            _startGameButton.interactable = state.CanStart;

            _sessionCodeText.text = state.HasSession ? state.JoinCode : "-";
            _copyButton.gameObject.SetActive(state.HasSession);
            _hostStatusText.text = L(!state.HasSession ? "BAĞLI DEĞİL" : state.IsHost ? "HOST SENSİN" : "OYUNCUSUN");
            _playerCountText.text = state.MaxPlayers > 0
                ? $"{L("OYUNCULAR")}  <color=#FF8A1F>{state.PlayerCount}</color> / {state.MaxPlayers}"
                : L("OYUNCULAR");
            _statusText.text = L(state.Status);

            for (int i = 0; i < SLOTS; i++)
            {
                bool isFilled = state.HasSession && i < state.PlayerCount;
                // Before a lobby exists there is no limit yet, so every slot is shown empty.
                bool isActive = state.MaxPlayers <= 0 || i < state.MaxPlayers;
                _slots[i].gameObject.SetActive(isActive);
                _slots[i].color = isFilled ? Color.white : new Color(1f, 1f, 1f, 0.35f);
                _slotTexts[i].text = isFilled ? Localizer.Format("OYUNCU {0}", i + 1) : L("BOŞ");
                _slotTexts[i].color = isFilled ? Color.white : Muted;
                _slotTicks[i].gameObject.SetActive(isFilled && i < state.PlayerReady.Count && state.PlayerReady[i]);
            }

            // Chosen freely before a lobby exists; inside one, only the host changes it.
            bool canChoose = !state.IsBusy && (!state.HasSession || state.IsHost);
            _lockRow.SetActive(!canChoose && state.HasSession);

            bool isZombie = state.Mode == GameMode.Zombie;
            ShowModeCard(_deathmatchCard, _deathmatchArt, !isZombie, canChoose);
            ShowModeCard(_zombieCard, _zombieArt, isZombie, canChoose);
            _selectedFrame.SetParent(isZombie ? _zombieCard.transform : _deathmatchCard.transform, false);

            // Map and difficulty only mean something against zombies.
            _mapSection.SetActive(isZombie);
            _difficultySection.SetActive(isZombie);
            _deathmatchNote.SetActive(!isZombie);

            bool isBase = state.Map == ZombieMap.Base;
            _mapArt.sprite = isBase ? _mapBase : _mapArena;
            _mapTitle.text = L(isBase ? "ÜS SAVUNMASI" : "ZOMBİ ARENA");
            _mapDescription.text = L(isBase
                ? "Duvarları ör, kapıyı tut, karargâhı gece boyunca savun."
                : "Şehir sokakları, açılacak kapılar ve yetenek makinesi.");
            _mapLeft.interactable = _mapRight.interactable = _mapCard.interactable = canChoose;

            bool isNightmare = state.Difficulty == Difficulty.Nightmare;
            _difficultyBar.sprite = isNightmare ? _difficultyNightmare : _difficultyDefault;
            FitFrame(_difficultyBar, 74f);
            _difficultyText.text = state.Difficulty switch
            {
                Difficulty.Easy => L("KOLAY"),
                Difficulty.Nightmare => $"<color=#FF4A3D>{L("KÂBUS")}</color>",
                _ => L("NORMAL")
            };
            _difficultyLeft.interactable = _difficultyRight.interactable = _difficultyButton.interactable = canChoose;

            _readyImage.sprite = state.IsLocalReady ? _readySelected : _secondary;
            FitFrame(_readyImage, 72f);
            _readyLabel.text = state.IsLocalReady ? "      " + L("HAZIR") : L("HAZIR OL");

            // The host's code is filled in for them. A client's field is never touched: a state that
            // arrives while the code is being typed would otherwise wipe it.
            if (state.HasSession && state.IsHost)
                _joinCodeInput.text = state.JoinCode;
        }

        private static void ShowModeCard(Button card, Image art, bool isSelected, bool canChoose)
        {
            card.interactable = canChoose && !isSelected;
            art.color = isSelected ? Color.white : Dimmed;
            card.transform.localScale = Vector3.one * (isSelected ? 1f : 0.94f);
        }

        private void OnCreateClicked() => CreateClicked?.Invoke(_playerNameInput.text);
        private void OnJoinClicked() => JoinClicked?.Invoke(_playerNameInput.text, _joinCodeInput.text);
        private void OnLeaveClicked() => LeaveClicked?.Invoke();
        private void OnReadyClicked() => ReadyClicked?.Invoke();
        private void OnStartClicked() => StartClicked?.Invoke();
        private void OnDifficultyClicked() => DifficultyClicked?.Invoke();
        private void OnMapClicked() => MapClicked?.Invoke();
        private void OnLanguageClicked() => LanguageClicked?.Invoke();

        // There are two modes, so picking the one not shown is the same as cycling to it.
        private void OnDeathmatchClicked()
        {
            if (_mode != GameMode.Deathmatch)
                ModeClicked?.Invoke();
        }

        private void OnZombieClicked()
        {
            if (_mode != GameMode.Zombie)
                ModeClicked?.Invoke();
        }

        private void OnCopyClicked()
        {
            if (!string.IsNullOrEmpty(_joinCode))
                GUIUtility.systemCopyBuffer = _joinCode;
        }

        // ---------- Layout ----------

        private void BuildBackground()
        {
            Image background = Img("Background", transform, Load("Background"), Vector2.zero, Vector2.zero);
            Stretch(background.rectTransform);
            var fitter = background.gameObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = 1672f / 941f;

            Image shade = Img("Shade", transform, null, Vector2.zero, Vector2.zero);
            Stretch(shade.rectTransform);
            shade.color = new Color(0.02f, 0.03f, 0.04f, 0.45f);
        }

        private void BuildBoard()
        {
            var go = new GameObject("Board", typeof(RectTransform));
            _board = (RectTransform) go.transform;
            _board.SetParent(transform, false);
            _board.anchorMin = _board.anchorMax = new Vector2(0.5f, 0.5f);
            _board.sizeDelta = new Vector2(BOARD_WIDTH, BOARD_HEIGHT);
        }

        private void BuildLeftColumn()
        {
            const float x = -560f;
            const float width = 440f;

            Image logo = Img("Logo", _board, Load("Logo_Fireline"), new Vector2(x, 365f), new Vector2(width, 147f));
            logo.preserveAspect = true;

            Label("NameLabel", new Vector2(x, 278f), width, "OYUNCU ADI");
            PlaceField(_playerNameInput, new Vector2(x, 238f), new Vector2(width, 60f), "Adını yaz");

            Label("CodeLabel", new Vector2(x, 186f), width, "LOBİ KODU");
            Image codePanel = Frame("CodePanel", _board, Load("Panel"), new Vector2(x, 146f), new Vector2(width, 60f));
            Move(_sessionCodeText.rectTransform, codePanel.transform, new Vector2(-24f, 0f), new Vector2(width - 110f, 50f));
            Style(_sessionCodeText, 30f, Cyan, TextAlignmentOptions.Center);
            _sessionCodeText.characterSpacing = 12f;
            _copyButton = IconButton("CopyButton", codePanel.transform, Load("Icon_Copy"), new Vector2(width * 0.5f - 40f, 0f), 42f);

            PlaceField(_joinCodeInput, new Vector2(x, 76f), new Vector2(width, 60f), "Katılmak için kod gir");

            PlaceButton(_createButton, _primary, new Vector2(x - 112f, 6f), new Vector2(216f, 64f), "LOBİ KUR", Color.black);
            PlaceButton(_joinButton, _secondary, new Vector2(x + 112f, 6f), new Vector2(216f, 64f), "KATIL", Color.white);

            Move(_hostStatusText.rectTransform, _board, new Vector2(x, -52f), new Vector2(width, 30f));
            Style(_hostStatusText, 20f, Orange, TextAlignmentOptions.Center);

            Move(_playerCountText.rectTransform, _board, new Vector2(x, -88f), new Vector2(width, 32f));
            Style(_playerCountText, 22f, Color.white, TextAlignmentOptions.Left);

            Sprite slot = Load("PlayerSlot");
            Sprite tick = Load("Icon_Ready");
            for (int i = 0; i < SLOTS; i++)
            {
                Image image = Frame($"Slot{i}", _board, slot, new Vector2(x, -134f - i * 62f), new Vector2(width, 56f));
                _slots[i] = image;
                _slotTexts[i] = Txt("Name", image.transform, new Vector2(30f, 0f), new Vector2(width - 160f, 44f), string.Empty, 22f, Color.white, TextAlignmentOptions.Left);
                _slotTicks[i] = Img("Tick", image.transform, tick, new Vector2(-width * 0.5f + 34f, 0f), new Vector2(40f, 40f));
                _slotTicks[i].preserveAspect = true;
            }
        }

        private void BuildModes()
        {
            const float centre = 220f;

            Header("ModeHeader", new Vector2(centre, 392f), "OYUN MODU");
            _lockRow = new GameObject("Lock", typeof(RectTransform));
            var lockRect = (RectTransform) _lockRow.transform;
            lockRect.SetParent(_board, false);
            lockRect.anchoredPosition = new Vector2(centre + 380f, 392f);
            Img("Icon", lockRect, Load("Icon_Lock"), new Vector2(-120f, 0f), new Vector2(34f, 34f)).preserveAspect = true;
            Txt("Text", lockRect, new Vector2(10f, 0f), new Vector2(240f, 34f), "Host seçiyor", 20f, Muted, TextAlignmentOptions.Left);

            _deathmatchCard = ModeCard("Deathmatch", new Vector2(centre - 262f, 250f), Load("Mode_Deathmatch"), "DEATHMATCH", "Birbirinizle savaşın", out _deathmatchArt);
            _zombieCard = ModeCard("Zombie", new Vector2(centre + 262f, 250f), Load("Mode_Zombie"), "ZOMBİ", "Dalgalara karşı birlikte hayatta kalın", out _zombieArt);

            Image frame = Outline("Selected", _zombieCard.transform, new Vector2(500f, 227f));
            frame.raycastTarget = false;
            _selectedFrame = frame.rectTransform;

            _deathmatchNote = Txt("DeathmatchNote", _board, new Vector2(centre, -40f), new Vector2(900f, 80f),
                "Deathmatch tek haritada oynanır. Son kalan değil, en çok vuran kazanır.", 24f, Muted, TextAlignmentOptions.Center).gameObject;
            _deathmatchNote.GetComponent<TMP_Text>().textWrappingMode = TextWrappingModes.Normal;
        }

        private Button ModeCard(string name, Vector2 position, Sprite art, string title, string subtitle, out Image artImage)
        {
            var size = new Vector2(500f, 227f);
            artImage = Img(name + "Card", _board, art, position, size);
            var button = artImage.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;

            Image band = Img("Band", artImage.transform, null, new Vector2(0f, -size.y * 0.5f + 34f), new Vector2(size.x - 16f, 60f));
            band.color = new Color(0f, 0f, 0f, 0.6f);
            band.raycastTarget = false;
            Txt("Title", band.transform, new Vector2(0f, 9f), new Vector2(size.x - 40f, 36f), title, 30f, Color.white, TextAlignmentOptions.Center);
            Txt("Subtitle", band.transform, new Vector2(0f, -16f), new Vector2(size.x - 40f, 24f), subtitle, 16f, Muted, TextAlignmentOptions.Center);
            return button;
        }

        private void BuildMap()
        {
            const float centre = 220f;

            var section = new GameObject("Map", typeof(RectTransform));
            _mapSection = section;
            var rect = (RectTransform) section.transform;
            rect.SetParent(_board, false);

            Header("MapHeader", new Vector2(centre, 98f), "HARİTA", rect);

            var size = new Vector2(600f, 240f);
            _mapArt = Img("MapCard", rect, _mapArena, new Vector2(centre, -40f), size);
            _mapCard = _mapArt.gameObject.AddComponent<Button>();
            _mapCard.transition = Selectable.Transition.None;
            Image outline = Outline("Frame", _mapArt.transform, size);
            outline.raycastTarget = false;

            Image band = Img("Band", _mapArt.transform, null, new Vector2(0f, -size.y * 0.5f + 36f), new Vector2(size.x - 20f, 64f));
            band.color = new Color(0f, 0f, 0f, 0.62f);
            band.raycastTarget = false;
            _mapTitle = Txt("Title", band.transform, new Vector2(0f, 10f), new Vector2(size.x - 40f, 36f), string.Empty, 30f, Orange, TextAlignmentOptions.Center);
            _mapDescription = Txt("Description", band.transform, new Vector2(0f, -17f), new Vector2(size.x - 40f, 24f), string.Empty, 17f, Color.white, TextAlignmentOptions.Center);

            Sprite arrow = Load("Arrow_Left");
            _mapLeft = IconButton("Left", rect, arrow, new Vector2(centre - size.x * 0.5f - 60f, -40f), 76f);
            _mapRight = IconButton("Right", rect, arrow, new Vector2(centre + size.x * 0.5f + 60f, -40f), 76f);
            _mapRight.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        private void BuildDifficulty()
        {
            const float centre = 220f;

            var section = new GameObject("Difficulty", typeof(RectTransform));
            _difficultySection = section;
            var rect = (RectTransform) section.transform;
            rect.SetParent(_board, false);

            Txt("Label", rect, new Vector2(centre - 300f, -218f), new Vector2(160f, 40f), "ZORLUK", 22f, Muted, TextAlignmentOptions.Right);
            _difficultyBar = Frame("Bar", rect, _difficultyDefault, new Vector2(centre + 20f, -218f), new Vector2(340f, 74f));
            _difficultyButton = _difficultyBar.gameObject.AddComponent<Button>();
            _difficultyButton.transition = Selectable.Transition.None;
            _difficultyText = Txt("Text", _difficultyBar.transform, Vector2.zero, new Vector2(260f, 50f), "NORMAL", 30f, Color.white, TextAlignmentOptions.Center);

            Sprite arrow = Load("Arrow_Left");
            _difficultyLeft = IconButton("Left", rect, arrow, new Vector2(centre - 190f, -218f), 54f);
            _difficultyRight = IconButton("Right", rect, arrow, new Vector2(centre + 230f, -218f), 54f);
            _difficultyRight.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        private void BuildLanguageButton()
        {
            Image image = Frame("LanguageButton", _board, _secondary, new Vector2(700f, 412f), new Vector2(150f, 52f));
            _languageButton = image.gameObject.AddComponent<Button>();
            _languageText = Txt("Label", image.transform, Vector2.zero, new Vector2(120f, 40f), string.Empty, 22f, Color.white, TextAlignmentOptions.Center);
        }

        private void BuildBottomBar()
        {
            Move(_statusText.rectTransform, _board, new Vector2(-60f, -380f), new Vector2(560f, 60f));
            Style(_statusText, 20f, Muted, TextAlignmentOptions.Left);
            _statusText.textWrappingMode = TextWrappingModes.Normal;

            PlaceButton(_leaveButton, _secondary, new Vector2(320f, -380f), new Vector2(190f, 66f), "AYRIL", Color.white);
            PlaceButton(_readyButton, _secondary, new Vector2(530f, -380f), new Vector2(220f, 72f), "HAZIR OL", Color.white);
            _readyImage = (Image) _readyButton.targetGraphic;
            _readyLabel = _readyButton.GetComponentInChildren<TMP_Text>(true);
            PlaceButton(_startGameButton, _primary, new Vector2(530f, -290f), new Vector2(300f, 84f), "BAŞLAT", Color.black);
        }

        // ---------- Pieces ----------

        private Sprite Load(string name)
        {
            var sprite = Resources.Load<Sprite>(SPRITES + name);
            if (sprite == null)
                FlowLogger.LogWarning($"LobbyScreenView - the sprite {SPRITES}{name} is missing or not imported as a Sprite.");
            return sprite;
        }

        private void PlaceButton(Button button, Sprite sprite, Vector2 position, Vector2 size, string text, Color textColour)
        {
            var rect = (RectTransform) button.transform;
            Move(rect, _board, position, size);

            // The prefab's decoration goes; the sprite carries the look.
            for (int i = rect.childCount - 1; i >= 0; i--)
                rect.GetChild(i).gameObject.SetActive(false);

            Image image = Skin(rect);
            image.sprite = sprite;
            image.color = Color.white;
            FitFrame(image, size.y);
            button.targetGraphic = image;
            button.transition = Selectable.Transition.ColorTint;
            ColorBlock colours = button.colors;
            colours.normalColor = Color.white;
            colours.highlightedColor = new Color(1f, 0.92f, 0.8f);
            colours.pressedColor = new Color(0.75f, 0.75f, 0.75f);
            colours.disabledColor = new Color(0.45f, 0.45f, 0.45f, 0.75f);
            button.colors = colours;

            Txt("Label", rect, Vector2.zero, size - new Vector2(30f, 10f), text, size.y * 0.38f, textColour, TextAlignmentOptions.Center);
        }

        // The control's own graphic, whatever it is, makes way for an image behind its children: an
        // object holds one Graphic, so a control that already has a text or a raw image takes no Image.
        private static Image Skin(RectTransform rect)
        {
            if (rect.TryGetComponent(out Image own))
            {
                own.enabled = true;
                return own;
            }

            if (rect.TryGetComponent(out Graphic other))
                other.enabled = false;

            Image image = Img("Skin", rect, null, Vector2.zero, Vector2.zero);
            Stretch(image.rectTransform);
            image.transform.SetAsFirstSibling();
            return image;
        }

        private void PlaceField(TMP_InputField field, Vector2 position, Vector2 size, string placeholder)
        {
            var rect = (RectTransform) field.transform;
            Move(rect, _board, position, size);

            Image image = Skin(rect);
            image.sprite = Load("Panel");
            image.color = Color.white;
            FitFrame(image, size.y);

            if (field.textViewport != null)
            {
                field.textViewport.anchorMin = Vector2.zero;
                field.textViewport.anchorMax = Vector2.one;
                field.textViewport.offsetMin = new Vector2(28f, 6f);
                field.textViewport.offsetMax = new Vector2(-28f, -6f);
            }

            if (field.textComponent != null)
                Style(field.textComponent, 24f, Color.white, TextAlignmentOptions.MidlineLeft);

            if (field.placeholder is TMP_Text hint)
            {
                Style(hint, 22f, new Color(Muted.r, Muted.g, Muted.b, 0.6f), TextAlignmentOptions.MidlineLeft);
                Localizer.Bind(hint, placeholder);
            }
        }

        private Button IconButton(string name, Transform parent, Sprite sprite, Vector2 position, float size)
        {
            Image image = Img(name, parent, sprite, position, new Vector2(size, size));
            image.preserveAspect = true;
            var button = image.gameObject.AddComponent<Button>();
            ColorBlock colours = button.colors;
            colours.highlightedColor = new Color(1f, 0.85f, 0.6f);
            colours.disabledColor = new Color(0.4f, 0.4f, 0.4f, 0.5f);
            button.colors = colours;
            return button;
        }

        private void Header(string name, Vector2 position, string text, Transform parent = null)
        {
            Transform target = parent != null ? parent : _board;
            Txt(name, target, position, new Vector2(500f, 40f), text, 26f, Orange, TextAlignmentOptions.Center).characterSpacing = 8f;
        }

        private void Label(string name, Vector2 position, float width, string text) =>
            Txt(name, _board, position, new Vector2(width, 26f), text, 17f, Muted, TextAlignmentOptions.Left).characterSpacing = 4f;

        private static Image Img(string name, Transform parent, Sprite sprite, Vector2 position, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform) go.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            var image = go.AddComponent<Image>();
            image.sprite = sprite;
            return image;
        }

        // The selected frame's art sits inside a transparent margin (in its texture's pixels: 27 left
        // and right, 33 top, 60 bottom). The image is grown by that margin so the drawn edge lies on
        // the target's edge, a few units outside it.
        private const float OUTLINE_LEFT = 27f;
        private const float OUTLINE_RIGHT = 27f;
        private const float OUTLINE_TOP = 33f;
        private const float OUTLINE_BOTTOM = 60f;
        private const float OUTLINE_OUTSET = 6f;

        private Image Outline(string name, Transform parent, Vector2 target)
        {
            Sprite sprite = Load("Frame_Selected");
            Vector2 visible = target + Vector2.one * (OUTLINE_OUTSET * 2f);
            float textureHeight = sprite != null ? sprite.rect.height : 724f;
            float height = visible.y * textureHeight / (textureHeight - OUTLINE_TOP - OUTLINE_BOTTOM);
            float unitsPerPixel = height / textureHeight;
            float width = visible.x + (OUTLINE_LEFT + OUTLINE_RIGHT) * unitsPerPixel;
            float shift = (OUTLINE_TOP - OUTLINE_BOTTOM) * 0.5f * unitsPerPixel;

            Image image = Img(name, parent, sprite, new Vector2((OUTLINE_RIGHT - OUTLINE_LEFT) * 0.5f * unitsPerPixel, shift), new Vector2(width, height));
            FitFrame(image, height);
            image.raycastTarget = false;
            return image;
        }

        private static Image Frame(string name, Transform parent, Sprite sprite, Vector2 position, Vector2 size)
        {
            Image image = Img(name, parent, sprite, position, size);
            FitFrame(image, size.y);
            return image;
        }

        // A frame keeps its art's proportions top to bottom and stretches only in the middle.
        private static void FitFrame(Image image, float height)
        {
            if (image.sprite == null)
                return;

            image.type = image.sprite.border.sqrMagnitude > 0f ? Image.Type.Sliced : Image.Type.Simple;
            image.pixelsPerUnitMultiplier = image.sprite.rect.height / Mathf.Max(1f, height);
        }

        // A label built with a text keeps that text through every change of language.
        private TMP_Text Txt(string name, Transform parent, Vector2 position, Vector2 size, string text, float fontSize, Color colour, TextAlignmentOptions alignment)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform) go.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            var label = go.AddComponent<TextMeshProUGUI>();
            Style(label, fontSize, colour, alignment);
            if (string.IsNullOrEmpty(text))
                label.text = string.Empty;
            else
                Localizer.Bind(label, text);
            return label;
        }

        private static void Style(TMP_Text label, float fontSize, Color colour, TextAlignmentOptions alignment)
        {
            label.fontSize = fontSize;
            label.enableAutoSizing = false;
            label.fontStyle = FontStyles.Bold;
            label.color = colour;
            label.alignment = alignment;
            label.raycastTarget = false;
            label.richText = true;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.overflowMode = TextOverflowModes.Ellipsis;
        }

        private static void Move(RectTransform rect, Transform parent, Vector2 position, Vector2 size)
        {
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        protected override void PlayShowAnimation() => ShowCompleted?.Invoke(this);

        protected override void PlayHideAnimation() => HideCompleted?.Invoke(this);
    }
}
