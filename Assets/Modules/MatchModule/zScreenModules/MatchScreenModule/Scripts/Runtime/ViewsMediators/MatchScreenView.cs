using System;
using System.Collections.Generic;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.ScreenModule.ViewsMediators.Screen;
using Modules.MatchModule.Shared.Data.ValueObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.MatchModule.MatchScreenModule.ViewsMediators
{
    /// <summary>
    /// The match overlay: the scoreboard and the button that leaves the match. Leaving asks first,
    /// on a small panel the view builds for itself.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class MatchScreenView : ScreenView
    {
        private const string CONFIRM_QUESTION = "Maçtan çıkmak istiyor musun?";
        private const string CONFIRM_YES = "Evet";
        private const string CONFIRM_NO = "Hayır";

        [SerializeField] private Transform _rowContent;
        [SerializeField] private ScoreboardRowView _rowPrefab;
        [SerializeField] private Button _exitButton;

        private readonly List<ScoreboardRowView> _rows = new();

        private GameObject _confirmPanel;
        private Button _confirmYes;
        private Button _confirmNo;

        /// <summary>The exit button was pressed; the Mediator asks for confirmation.</summary>
        public Action ExitClicked;

        /// <summary>The player answered yes on the confirmation panel.</summary>
        public Action ExitConfirmed;

        private void Awake() => BuildConfirmPanel();

        private void OnEnable()
        {
            _exitButton.onClick.AddListener(OnExitClicked);
            _confirmYes.onClick.AddListener(OnConfirmYes);
            _confirmNo.onClick.AddListener(HideExitConfirm);
        }

        private void OnDisable()
        {
            _exitButton.onClick.RemoveListener(OnExitClicked);
            _confirmYes.onClick.RemoveListener(OnConfirmYes);
            _confirmNo.onClick.RemoveListener(HideExitConfirm);
        }

        // A pooled screen comes back with whatever its last opening left; the question is never left up.
        public override void BeforeScreenActivation()
        {
            base.BeforeScreenActivation();
            HideExitConfirm();
        }

        public void ShowScores(ScoreEntryVO[] scores)
        {
            // Rows are kept and reused; the board only ever holds the players of one match.
            while (_rows.Count < scores.Length)
                _rows.Add(Instantiate(_rowPrefab, _rowContent));

            for (int i = 0; i < _rows.Count; i++)
            {
                bool used = i < scores.Length;
                _rows[i].gameObject.SetActive(used);

                if (used)
                    _rows[i].Show(scores[i].PlayerName, scores[i].Kills, scores[i].Deaths, scores[i].Kda);
            }
        }

        public void ShowExitConfirm() => _confirmPanel.SetActive(true);

        public void HideExitConfirm()
        {
            if (_confirmPanel != null)
                _confirmPanel.SetActive(false);
        }

        private void OnExitClicked() => ExitClicked?.Invoke();

        private void OnConfirmYes()
        {
            HideExitConfirm();
            ExitConfirmed?.Invoke();
        }

        private void BuildConfirmPanel()
        {
            // A full-screen shade catches every click, so nothing behind the question is pressed.
            _confirmPanel = CreateRect("ExitConfirm", transform, Vector2.zero, Vector2.one, Vector2.zero);
            _confirmPanel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);

            GameObject box = CreateRect("Box", _confirmPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(360f, 170f));
            box.AddComponent<Image>().color = new Color(0.12f, 0.12f, 0.14f, 0.95f);

            GameObject question = CreateRect("Question", box.transform, new Vector2(0f, 0.5f), new Vector2(1f, 1f), Vector2.zero);
            TMP_Text questionText = question.AddComponent<TextMeshProUGUI>();
            questionText.text = CONFIRM_QUESTION;
            questionText.fontSize = 24f;
            questionText.alignment = TextAlignmentOptions.Center;

            _confirmYes = CreateButton("Yes", box.transform, new Vector2(0.27f, 0.25f), CONFIRM_YES, new Color(0.7f, 0.2f, 0.2f));
            _confirmNo = CreateButton("No", box.transform, new Vector2(0.73f, 0.25f), CONFIRM_NO, new Color(0.3f, 0.3f, 0.34f));

            _confirmPanel.SetActive(false);
        }

        private static Button CreateButton(string name, Transform parent, Vector2 anchor, string label, Color color)
        {
            GameObject go = CreateRect(name, parent, anchor, anchor, new Vector2(130f, 48f));
            go.AddComponent<Image>().color = color;
            var button = go.AddComponent<Button>();

            GameObject text = CreateRect("Label", go.transform, Vector2.zero, Vector2.one, Vector2.zero);
            TMP_Text labelText = text.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 22f;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.raycastTarget = false;

            return button;
        }

        private static GameObject CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform) go.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
            return go;
        }

        protected override void PlayShowAnimation() => ShowCompleted?.Invoke(this);

        protected override void PlayHideAnimation() => HideCompleted?.Invoke(this);
    }
}
