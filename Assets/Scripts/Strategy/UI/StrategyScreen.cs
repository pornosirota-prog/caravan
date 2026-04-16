using System;
using System.Collections.Generic;
using CaravanRoguelite.Strategy.Domain;
using CaravanRoguelite.Strategy.Services;
using CaravanRoguelite.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CaravanRoguelite.Strategy.UI
{
    public sealed class StrategyScreen
    {
        private readonly RectTransform _root;
        private readonly StrategyGameService _service;
        private readonly StrategyGameState _state;
        private readonly Dictionary<int, Button> _territoryButtons = new();

        private Text _resourceLabel;
        private Text _battleLabel;
        private Text _historyLabel;
        private Text _taskLabel;
        private InputField _answerInput;
        private Text _timerLabel;
        private Text _progressLabel;
        private float _answerTimer;

        public Action BackToCaravanRequested;

        public StrategyScreen(Transform parent, StrategyGameService service)
        {
            _service = service;
            _state = _service.CreateNewState();
            _root = new GameObject("StrategyScreen", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            _root.SetParent(parent, false);
            _root.anchorMin = new Vector2(0.02f, 0.02f);
            _root.anchorMax = new Vector2(0.98f, 0.98f);
            _root.offsetMin = Vector2.zero;
            _root.offsetMax = Vector2.zero;
            var image = _root.GetComponent<Image>();
            image.sprite = ProceduralSpriteFactory.CreateRoundedRect(new Color(0.04f, 0.06f, 0.1f, 0.9f), new Color(0.27f, 0.42f, 0.62f, 1f), 160, 24, 7);
            image.type = Image.Type.Sliced;

            BuildLayout();
            RefreshAll();
            SetVisible(false);
        }

        public void SetVisible(bool visible)
        {
            _root.gameObject.SetActive(visible);
        }

        public void Tick(float deltaTime)
        {
            if (!_root.gameObject.activeSelf)
            {
                return;
            }

            if (_state.ActiveBattle == null)
            {
                return;
            }

            _answerTimer += deltaTime;
            var outcome = _service.TickBattle(_state, deltaTime);
            if (outcome.Solved > 0 || outcome.Captured)
            {
                _battleLabel.text = outcome.Captured
                    ? $"Территория захвачена! Точность {Mathf.RoundToInt(outcome.Accuracy * 100f)}%"
                    : "Время вышло. Попробуйте штурм снова.";
                _answerInput.text = string.Empty;
            }

            RefreshBattle();
            RefreshMap();
            RefreshResources();
            RefreshHistory();
        }

        private void BuildLayout()
        {
            var header = UiFactory.MakeText(_root, "Стратегический режим", 28, TextAnchor.MiddleLeft);
            header.rectTransform.anchorMin = new Vector2(0.02f, 0.93f);
            header.rectTransform.anchorMax = new Vector2(0.65f, 0.995f);

            var backButton = UiFactory.MakeButton(_root, "К каравану");
            var backRect = backButton.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0.82f, 0.93f);
            backRect.anchorMax = new Vector2(0.98f, 0.99f);
            backRect.offsetMin = Vector2.zero;
            backRect.offsetMax = Vector2.zero;
            backButton.onClick.AddListener(() => BackToCaravanRequested?.Invoke());

            var mapPanel = new GameObject("Map", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            mapPanel.SetParent(_root, false);
            mapPanel.anchorMin = new Vector2(0.02f, 0.24f);
            mapPanel.anchorMax = new Vector2(0.6f, 0.9f);
            mapPanel.offsetMin = Vector2.zero;
            mapPanel.offsetMax = Vector2.zero;
            mapPanel.GetComponent<Image>().sprite = ProceduralSpriteFactory.CreateRoundedRect(new Color(0.07f, 0.1f, 0.14f, 0.92f), new Color(0.26f, 0.36f, 0.48f, 1f), 96, 16, 6);
            mapPanel.GetComponent<Image>().type = Image.Type.Sliced;

            BuildMapButtons(mapPanel);

            var sidePanel = new GameObject("Info", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            sidePanel.SetParent(_root, false);
            sidePanel.anchorMin = new Vector2(0.62f, 0.24f);
            sidePanel.anchorMax = new Vector2(0.98f, 0.9f);
            sidePanel.offsetMin = Vector2.zero;
            sidePanel.offsetMax = Vector2.zero;
            sidePanel.GetComponent<Image>().sprite = ProceduralSpriteFactory.CreateRoundedRect(new Color(0.06f, 0.08f, 0.13f, 0.94f), new Color(0.28f, 0.4f, 0.54f, 1f), 96, 16, 6);
            sidePanel.GetComponent<Image>().type = Image.Type.Sliced;

            _resourceLabel = UiFactory.MakeText(sidePanel, string.Empty, 16, TextAnchor.UpperLeft);
            _resourceLabel.rectTransform.anchorMin = new Vector2(0.05f, 0.72f);
            _resourceLabel.rectTransform.anchorMax = new Vector2(0.95f, 0.98f);

            _battleLabel = UiFactory.MakeText(sidePanel, "Выберите соседнюю территорию для штурма.", 15, TextAnchor.UpperLeft);
            _battleLabel.rectTransform.anchorMin = new Vector2(0.05f, 0.52f);
            _battleLabel.rectTransform.anchorMax = new Vector2(0.95f, 0.7f);

            _timerLabel = UiFactory.MakeText(sidePanel, string.Empty, 15, TextAnchor.MiddleLeft);
            _timerLabel.rectTransform.anchorMin = new Vector2(0.05f, 0.46f);
            _timerLabel.rectTransform.anchorMax = new Vector2(0.95f, 0.52f);

            _progressLabel = UiFactory.MakeText(sidePanel, string.Empty, 15, TextAnchor.MiddleLeft);
            _progressLabel.rectTransform.anchorMin = new Vector2(0.05f, 0.4f);
            _progressLabel.rectTransform.anchorMax = new Vector2(0.95f, 0.46f);

            _taskLabel = UiFactory.MakeText(sidePanel, "", 24, TextAnchor.MiddleCenter);
            _taskLabel.rectTransform.anchorMin = new Vector2(0.05f, 0.28f);
            _taskLabel.rectTransform.anchorMax = new Vector2(0.95f, 0.4f);

            BuildAnswerInput(sidePanel);

            _historyLabel = UiFactory.MakeText(_root, string.Empty, 15, TextAnchor.UpperLeft);
            _historyLabel.rectTransform.anchorMin = new Vector2(0.02f, 0.03f);
            _historyLabel.rectTransform.anchorMax = new Vector2(0.98f, 0.21f);
        }

        private void BuildMapButtons(RectTransform mapPanel)
        {
            foreach (var territory in _state.Map.Territories)
            {
                var button = UiFactory.MakeButton(mapPanel, $"#{territory.Id}");
                var rect = button.GetComponent<RectTransform>();
                float stepX = 1f / StrategyConfig.MapCols;
                float stepY = 1f / StrategyConfig.MapRows;
                rect.anchorMin = new Vector2(territory.Col * stepX + 0.03f, 1f - (territory.Row + 1) * stepY + 0.03f);
                rect.anchorMax = new Vector2((territory.Col + 1) * stepX - 0.03f, 1f - territory.Row * stepY - 0.03f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                int territoryId = territory.Id;
                button.onClick.AddListener(() => OnTerritoryClicked(territoryId));
                _territoryButtons[territory.Id] = button;
            }
        }

        private void BuildAnswerInput(RectTransform sidePanel)
        {
            var inputRoot = new GameObject("InputRoot", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            inputRoot.SetParent(sidePanel, false);
            inputRoot.anchorMin = new Vector2(0.05f, 0.18f);
            inputRoot.anchorMax = new Vector2(0.7f, 0.27f);
            inputRoot.offsetMin = Vector2.zero;
            inputRoot.offsetMax = Vector2.zero;
            var inputBg = inputRoot.GetComponent<Image>();
            inputBg.sprite = ProceduralSpriteFactory.CreateRoundedRect(new Color(0.12f, 0.16f, 0.22f, 1f), new Color(0.35f, 0.45f, 0.58f, 1f), 64, 10, 4);
            inputBg.type = Image.Type.Sliced;

            _answerInput = inputRoot.gameObject.AddComponent<InputField>();
            var text = UiFactory.MakeText(inputRoot, string.Empty, 20, TextAnchor.MiddleLeft);
            text.rectTransform.anchorMin = new Vector2(0.05f, 0.1f);
            text.rectTransform.anchorMax = new Vector2(0.95f, 0.9f);
            _answerInput.textComponent = text;
            _answerInput.characterValidation = InputField.CharacterValidation.Integer;
            _answerInput.lineType = InputField.LineType.SingleLine;

            var placeholder = UiFactory.MakeText(inputRoot, "Ответ", 18, TextAnchor.MiddleLeft);
            placeholder.rectTransform.anchorMin = new Vector2(0.05f, 0.1f);
            placeholder.rectTransform.anchorMax = new Vector2(0.95f, 0.9f);
            placeholder.color = new Color(0.7f, 0.76f, 0.84f, 0.7f);
            _answerInput.placeholder = placeholder;

            var submit = UiFactory.MakeButton(sidePanel, "Ответить");
            var submitRect = submit.GetComponent<RectTransform>();
            submitRect.anchorMin = new Vector2(0.73f, 0.18f);
            submitRect.anchorMax = new Vector2(0.95f, 0.27f);
            submitRect.offsetMin = Vector2.zero;
            submitRect.offsetMax = Vector2.zero;
            submit.onClick.AddListener(SubmitAnswer);
        }

        private void OnTerritoryClicked(int territoryId)
        {
            if (_state.ActiveBattle != null)
            {
                _battleLabel.text = "Бой уже идёт. Завершите текущий штурм.";
                return;
            }

            var session = _service.StartBattle(_state, territoryId);
            if (session == null)
            {
                _battleLabel.text = "Можно атаковать только соседние нейтральные территории.";
            }
            else
            {
                _battleLabel.text = $"Штурм территории #{territoryId}.";
                _answerTimer = 0f;
            }

            RefreshAll();
        }

        private void SubmitAnswer()
        {
            if (_state.ActiveBattle == null)
            {
                return;
            }

            if (!int.TryParse(_answerInput.text, out int answer))
            {
                _battleLabel.text = "Введите число в поле ответа.";
                return;
            }

            var result = _service.SubmitAnswer(_state, answer, _answerTimer);
            _answerInput.text = string.Empty;
            _answerTimer = 0f;
            _battleLabel.text = result.IsCorrect ? "Верно!" : "Неверно.";
            RefreshAll();
        }

        private void RefreshAll()
        {
            RefreshMap();
            RefreshResources();
            RefreshBattle();
            RefreshHistory();
        }

        private void RefreshMap()
        {
            foreach (var territory in _state.Map.Territories)
            {
                if (!_territoryButtons.TryGetValue(territory.Id, out var button))
                {
                    continue;
                }

                var image = button.GetComponent<Image>();
                var label = button.GetComponentInChildren<Text>();
                bool attackable = _service.CanAttack(_state, territory.Id);
                label.text = $"#{territory.Id}\n{territory.Type}";

                if (territory.Owner == TerritoryOwner.Player)
                {
                    image.color = new Color(0.52f, 0.85f, 0.58f, 1f);
                    button.interactable = false;
                }
                else
                {
                    image.color = attackable ? new Color(0.93f, 0.61f, 0.44f, 1f) : new Color(0.42f, 0.46f, 0.53f, 1f);
                    button.interactable = attackable && _state.ActiveBattle == null;
                }
            }
        }

        private void RefreshResources()
        {
            _resourceLabel.text = $"Золото: {_state.Gold}\nЕда: {_state.Food}\nОчки: {_state.Score}\nЗахвачено: {_state.CapturedCount}/{_state.Map.Territories.Count}";
        }

        private void RefreshBattle()
        {
            if (_state.ActiveBattle == null)
            {
                _taskLabel.text = "Нет активного боя";
                _timerLabel.text = "Таймер: —";
                _progressLabel.text = "Прогресс: —";
                return;
            }

            _taskLabel.text = _state.ActiveBattle.CurrentTask.Prompt;
            _timerLabel.text = $"Таймер: {Mathf.CeilToInt(_state.ActiveBattle.RemainingTime)} сек";
            _progressLabel.text = $"Прогресс: {Mathf.RoundToInt(_state.ActiveBattle.Progress)}% | Серия: {_state.ActiveBattle.Combo}";
        }

        private void RefreshHistory()
        {
            string history = "Последние действия:\n";
            if (_state.History.Count == 0)
            {
                history += "- Пока пусто";
            }
            else
            {
                foreach (var entry in _state.History)
                {
                    history += $"- {entry}\n";
                }
            }

            _historyLabel.text = history;
        }
    }
}
