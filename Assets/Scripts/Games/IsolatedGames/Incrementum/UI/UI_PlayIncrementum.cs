using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Incrementum
{
    public class UI_PlayIncrementum : MonoBehaviour
    {
        private int PlayerHighscore = 0;

        [Header("Prefabs")]
        public UI_IncrementumUpgrade UpgradePrefab;

        [Header("Elements")]
        public UI_ToggleButton PauseOnUpgradeToggle;
        public Button ResetButton;
        public Button CloseButton;
        public TextMeshProUGUI TickText;
        public TextMeshProUGUI ResourceText;
        public TextMeshProUGUI ResourceIncomeText;
        public TextMeshProUGUI AiHighscoreText;
        public TextMeshProUGUI PlayerHighscoreText;

        // Optional: parent containers (if null, created at runtime)
        public RectTransform NodesRoot;
        public RectTransform LinesRoot;

        [Header("Run Settings")]
        public float TicksPerSecond = 10f;
        public KeyCode PauseKey = KeyCode.Space;

        private IncrementumTask Game;
        private bool Paused = false;
        private float TickAccum;

        private readonly Dictionary<UpgradeDef, UI_IncrementumUpgrade> uiByDef = new();
        private UpgradeDef staged;

        private static int UPGRADE_SIZE_X = 240;
        private static int UPGRADE_SIZE_Y = 90;

        // --- lifecycle -------------------------------------------------------

        public void Init()
        {
            AiHighscoreText.text = "";
            PlayerHighscoreText.text = "";

            ResetButton.onClick.AddListener(ResetGame);
            CloseButton.onClick.AddListener(Close);

            if (NodesRoot == null)
            {
                var nodesGO = new GameObject("NodesRoot", typeof(RectTransform));
                nodesGO.transform.SetParent(transform, false);
                NodesRoot = nodesGO.GetComponent<RectTransform>();
                CopyContainerSize(NodesRoot);
            }
            if (LinesRoot == null)
            {
                var linesGO = new GameObject("LinesRoot", typeof(RectTransform));
                linesGO.transform.SetParent(transform, false);
                LinesRoot = linesGO.GetComponent<RectTransform>();
                CopyContainerSize(LinesRoot);
            }

            BuildTree();
            ResetGame();
        }

        private void Update()
        {
            // Block pause toggling once the game is finished
            if (Input.GetKeyDown(PauseKey) && !Game.IsDone)
                Paused = !Paused;

            if (!Paused)
            {
                TickAccum += Time.deltaTime;
                float interval = 1f / Mathf.Max(1e-3f, TicksPerSecond);
                while (TickAccum >= interval && !Paused)
                {
                    TickAccum -= interval;
                    // Stop immediately if this tick finished the game
                    if (StepOneTick())
                    {
                        // Game is done
                        Paused = true;

                        // Highscore
                        int finalScore = Game.GetFitnessValue();
                        if (finalScore > PlayerHighscore)
                        {
                            PlayerHighscore = finalScore;

                            string highscoreText = $"<b>Player Highscore: {Game.GetFitnessValue()}</b>\n";
                            bool isFirst = true;
                            foreach (var h in Game.History)
                            {
                                if (!isFirst) highscoreText += " --> ";
                                highscoreText += $"{h.Value.Label} ({h.Key})";
                                isFirst = false;
                            }
                            PlayerHighscoreText.text = highscoreText;
                        }

                        break;
                    }
                }
            }

            UpdateHud();
        }

        // --- public actions --------------------------------------------------

        private void ClearUISelection()
        {
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
        }


        public void Show()
        {
            gameObject.SetActive(true);
            Paused = true;
            TickAccum = 0f;
            ResetGame();
            ClearUISelection();
        }


        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void ResetGame()
        {
            Game = new IncrementumTask(null);
            Game.Start();
            staged = null;
            Paused = true;
            TickAccum = 0f;

            foreach (var kv in uiByDef) kv.Value.SetState(UpgradeUIState.Unacquired);
            UpdateHud();
            ClearUISelection();
        }

        // called by cards
        public void OnUpgradeClicked(UI_IncrementumUpgrade card)
        {
            if (Game.IsDone) return;   // block interactions after end

            var def = card.Def;
            if (Game.AcquiredUpgrades[def]) return;

            // Try instant buy first
            if (Game.CanAcquireUpgrade(def) && Game.TryAcquireUpgrade(def))
            {
                card.SetState(UpgradeUIState.Acquired);
                if (staged == def) staged = null;
                RefreshAllStates();
                ClearUISelection();
                if (PauseOnUpgradeToggle.IsToggled) Paused = true;
                return;
            }

            // Only allow staging if requirements are met
            if (!RequirementsMet(def)) return;

            // Toggle staging...
            if (staged == def)
            {
                staged = null;
                card.SetState(UpgradeUIState.Unacquired);
            }
            else
            {
                if (staged != null && uiByDef.TryGetValue(staged, out var prev))
                    prev.SetState(UpgradeUIState.Unacquired);
                staged = def;
                card.SetState(UpgradeUIState.Staged);
            }

            ClearUISelection();
        }

        private bool RequirementsMet(UpgradeDef def)
        {
            var reqs = GetRequirements(def);
            return reqs.All(r => Game.AcquiredUpgrades[r]);
        }

        // --- ticking & HUD ---------------------------------------------------

        private bool StepOneTick()
        {
            // Don’t step if already done (extra safety)
            if (Game.IsDone) return true;

            Game.PlayerAdvanceTickNoAI();

            // auto-try staged purchase
            if (staged != null && Game.CanAcquireUpgrade(staged))
            {
                if (Game.TryAcquireUpgrade(staged))
                {
                    uiByDef[staged].SetState(UpgradeUIState.Acquired);
                    staged = null;
                    if (PauseOnUpgradeToggle.IsToggled) Paused = true;
                }
            }

            Game.CheckEnd();
            RefreshAllStates();

            return Game.IsDone;  // tell caller to stop & lock pause
        }

        private void UpdateHud()
        {
            var res = DefDatabase<ResourceDef>.AllDefs;

            TickText.text = Game.IsDone ? $"Tick {Game.TickNumber} (Finished)" : $"Tick {Game.TickNumber}";

            ResourceText.text = string.Join("   ",
                res.Select(r => $"{r.Label}: {Game.Resources[r]}"));

            ResourceIncomeText.text = string.Join("   ",
                res.Select(r =>
                {
                    int inc = Game.GetResourceIncomePerTick(r);
                    string sign = inc >= 0 ? "+" : "";
                    return $"{r.Label}/t: {sign}{inc}";
                }));
        }

        private void RefreshAllStates()
        {
            foreach (var kv in uiByDef)
            {
                var def = kv.Key;
                var card = kv.Value;

                if (Game.AcquiredUpgrades[def])
                {
                    card.SetState(UpgradeUIState.Acquired);
                }
                else if (staged == def)
                {
                    card.SetState(UpgradeUIState.Staged);
                }
                else
                {
                    card.SetState(UpgradeUIState.Unacquired);
                }
            }
        }

        // --- layout & connections -------------------------------------------

        private void BuildTree()
        {
            // clear previous
            foreach (Transform t in NodesRoot) Destroy(t.gameObject);
            foreach (Transform t in LinesRoot) Destroy(t.gameObject);
            uiByDef.Clear();

            var defs = DefDatabase<UpgradeDef>.AllDefs;
            // depth = longest path length from any root (no requirements)
            var depthCache = new Dictionary<UpgradeDef, int>();
            int Depth(UpgradeDef d)
            {
                if (depthCache.TryGetValue(d, out var v)) return v;
                var reqs = GetRequirements(d);
                int dep = reqs.Count == 0 ? 0 : 1 + reqs.Max(Depth);
                depthCache[d] = dep;
                return dep;
            }

            // compute depths and columns
            var byDepth = new Dictionary<int, List<UpgradeDef>>();
            foreach (var d in defs)
            {
                int dep = Depth(d);
                if (!byDepth.ContainsKey(dep)) byDepth[dep] = new List<UpgradeDef>();
                byDepth[dep].Add(d);
            }
            int maxDepth = byDepth.Keys.Count == 0 ? 0 : byDepth.Keys.Max();

            // layout rects
            var rt = (RectTransform)transform;
            float W = rt.rect.width, H = rt.rect.height;
            float paddingX = 40f, paddingY = 40f, colGap = 40f, rowGap = 20f;

            float colWidth = (W - 2 * paddingX - colGap * maxDepth) / Mathf.Max(1, (maxDepth + 1));
            float cardW = Mathf.Min(UPGRADE_SIZE_X, colWidth);
            float cardH = UPGRADE_SIZE_Y;

            // Instantiate cards column-by-column
            var cardRectByDef = new Dictionary<UpgradeDef, RectTransform>();

            for (int col = 0; col <= maxDepth; col++)
            {
                if (!byDepth.ContainsKey(col)) continue;
                var colDefs = byDepth[col].OrderBy(d => d.DefName).ToList();

                float colX = paddingX + col * (colWidth + colGap) + colWidth * 0.5f;

                // vertical positions, evenly spaced
                int n = colDefs.Count;
                float usableH = H - 2 * paddingY;
                float totalHeight = n * cardH + Mathf.Max(0, n - 1) * rowGap;
                float startY = paddingY + (usableH - totalHeight) * 0.5f + cardH * 0.5f;

                for (int i = 0; i < n; i++)
                {
                    var def = colDefs[i];
                    var go = Instantiate(UpgradePrefab, NodesRoot);
                    var cardRT = (RectTransform)go.transform;
                    cardRT.sizeDelta = new Vector2(cardW, cardH);
                    cardRT.anchorMin = cardRT.anchorMax = new Vector2(0f, 0f);
                    cardRT.anchoredPosition = new Vector2(colX, startY + i * (cardH + rowGap));

                    go.Init(this, def);
                    uiByDef[def] = go;
                    cardRectByDef[def] = cardRT;
                }
            }

            // draw prerequisite lines
            foreach (var def in defs)
            {
                var fromRT = cardRectByDef[def];
                foreach (var req in GetRequirements(def))
                {
                    var toRT = cardRectByDef[req];
                    DrawConnection(toRT.anchoredPosition, fromRT.anchoredPosition, new Color(1f, 1f, 1f, 0.15f), 3f);
                }
            }
        }

        private List<UpgradeDef> GetRequirements(UpgradeDef def)
        {
            // Prefer resolved references if available
            if (def.Requirements != null && def.Requirements.Count > 0)
                return def.Requirements;

            // Fallback to names if present
            if (def.RequirementDefNames != null && def.RequirementDefNames.Count > 0)
            {
                var all = DefDatabase<UpgradeDef>.AllDefs.ToDictionary(d => d.DefName);
                var list = new List<UpgradeDef>();
                foreach (var name in def.RequirementDefNames)
                    if (all.TryGetValue(name, out var req)) list.Add(req);
                return list;
            }
            return new List<UpgradeDef>();
        }

        private void DrawConnection(Vector2 from, Vector2 to, Color color, float thickness)
        {
            // simple line via Image (like your graph helper)
            var go = new GameObject("ReqLine", typeof(Image));
            go.transform.SetParent(LinesRoot, false);
            var img = go.GetComponent<Image>();
            img.color = color;

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0, 0);

            Vector2 dir = (to - from).normalized;
            float dist = Vector2.Distance(from, to);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            rt.sizeDelta = new Vector2(dist, thickness);
            rt.anchoredPosition = (from + to) * 0.5f;
            rt.localEulerAngles = new Vector3(0, 0, angle);
            go.transform.SetSiblingIndex(0);
        }

        private void CopyContainerSize(RectTransform child)
        {
            var rt = (RectTransform)transform;
            child.anchorMin = child.anchorMax = new Vector2(0, 0);
            child.pivot = new Vector2(0.5f, 0.5f);
            child.sizeDelta = rt.rect.size;
            child.anchoredPosition = Vector2.zero;
        }
    }
}
