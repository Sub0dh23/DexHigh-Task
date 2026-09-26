using System.Collections.Generic;
using DexHigh.Characters;
using DexHigh.Combat;
using DexHigh.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace DexHigh.EditorTools
{
    public static class BuildCombatHUD
    {
        private const string SPRITES_PATH = "Assets/_Project/Art/UI/StatusFrame/";
        private const string VICTORY_PATH = "Assets/_Project/Art/UI/VictoryScreen/";
        private const string FONT_PATH = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";

        [MenuItem("DexHigh/Build Dota2 Combat HUD")]
        public static void BuildHUD()
        {
            var activeScene = EditorSceneManager.GetActiveScene();

            // 1. Ensure EventSystem exists
            EnsureEventSystem();

            // 2. Remove any old CombatHUD_Canvas if present
            var existingCanvas = GameObject.Find("CombatHUD_Canvas");
            if (existingCanvas != null)
            {
                Object.DestroyImmediate(existingCanvas);
            }

            // 3. Load font
            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_PATH);

            // 4. Create Canvas Root
            var canvasGO = new GameObject("CombatHUD_Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();
            var combatHUD = canvasGO.AddComponent<CombatHUD>();

            // 5. Load Sprites
            var spPlayerFrame = LoadSprite("Frame_DragonBastion_Player.png");
            var spAIFrame = LoadSprite("Frame_DragonBastion_AI.png");
            var spPlayerChassis = LoadSprite("Chassis_Portrait_Player.png");
            var spAIChassis = LoadSprite("Chassis_Portrait_AI.png");
            var spPlayerDragon = LoadSprite("Icon_Dragon_Player.png");
            var spAIDragon = LoadSprite("Icon_Dragon_AI.png");
            var spBadgeShield = LoadSprite("Badge_LevelShield.png");
            var spChannelCradle = LoadSprite("Health_Channel_Cradle.png");
            var spEmeraldBlock = LoadSprite("Health_Block_Emerald.png");
            var spCrimsonBlock = LoadSprite("Health_Block_Crimson.png");
            var spGhostAmberBlock = LoadSprite("Health_Block_GhostAmber.png");
            var spDepletedBlock = LoadSprite("Health_Block_Depleted.png");
            var spSocketReadyGreen = LoadSprite("Talisman_Socket_Ready_Green.png");
            var spSocketCooldownAmber = LoadSprite("Talisman_Socket_Cooldown_Amber.png");
            var spJewelDiamond = LoadSprite("Jewel_Hotkey_Diamond.png");
            var spIconFireBreath = LoadSprite("Icon_Ability_FireBreath.png");
            var spIconTailWhip = LoadSprite("Icon_Ability_TailWhip.png");
            var spIconFlyDive = LoadSprite("Icon_Ability_FlyDive.png");

            // Victory Screen Sprites
            var spVicModal = LoadVictorySprite("Victory_Modal_Chassis.png");
            var spVicCrest = LoadVictorySprite("Victory_Crest_Horns.png");
            var spVicChassis = LoadVictorySprite("Victory_Portrait_Chassis.png");
            var spVicStatStrip = LoadVictorySprite("Victory_Stat_Strip_Chassis.png");
            var spVicBtnRematch = LoadVictorySprite("Victory_Btn_Rematch.png");
            var spVicBtnLeave = LoadVictorySprite("Victory_Btn_Leave.png");

            // 6. Build Player HUD (Top-Left)
            var playerFrameGO = CreateUIObject("Player_DragonBastion_Frame", canvasGO.transform);
            var playerFrameRT = playerFrameGO.GetComponent<RectTransform>();
            SetAnchor(playerFrameRT, 0, 1, 0, 1, 0, 1);
            playerFrameRT.anchoredPosition = new Vector2(30, -30);
            playerFrameRT.sizeDelta = new Vector2(500, 120);
            var playerFrameImg = playerFrameGO.AddComponent<Image>();
            playerFrameImg.sprite = spPlayerFrame;

            // Portrait Chassis
            var pPortraitGO = CreateUIObject("Chassis_Portrait", playerFrameGO.transform);
            var pPortraitRT = pPortraitGO.GetComponent<RectTransform>();
            SetAnchor(pPortraitRT, 0, 0.5f, 0, 0.5f, 0.5f, 0.5f);
            pPortraitRT.anchoredPosition = new Vector2(62, 0);
            pPortraitRT.sizeDelta = new Vector2(100, 100);
            pPortraitGO.AddComponent<Image>().sprite = spPlayerChassis;

            // Dragon Icon inside Portrait
            var pIconGO = CreateUIObject("Icon_Dragon", pPortraitGO.transform);
            var pIconRT = pIconGO.GetComponent<RectTransform>();
            SetAnchor(pIconRT, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
            pIconRT.anchoredPosition = Vector2.zero;
            pIconRT.sizeDelta = new Vector2(84, 84);
            pIconGO.AddComponent<Image>().sprite = spPlayerDragon;

            // Badge Level Shield
            var pBadgeGO = CreateUIObject("Badge_LevelShield", pPortraitGO.transform);
            var pBadgeRT = pBadgeGO.GetComponent<RectTransform>();
            SetAnchor(pBadgeRT, 1, 0, 1, 0, 0.5f, 0.5f);
            pBadgeRT.anchoredPosition = new Vector2(-12, 12);
            pBadgeRT.sizeDelta = new Vector2(28, 28);
            pBadgeGO.AddComponent<Image>().sprite = spBadgeShield;

            var pLvlText = CreateTMPText("Text_Level", pBadgeGO.transform, "1", fontAsset, 12, FontStyles.Bold, new Color(1f, 0.9f, 0.4f), TextAlignmentOptions.Center);
            pLvlText.rectTransform.anchoredPosition = new Vector2(0, 0);
            pLvlText.rectTransform.sizeDelta = new Vector2(28, 28);

            // Player Name Text
            var pNameText = CreateTMPText("Text_DragonName", playerFrameGO.transform, "INFERNO WYRM", fontAsset, 15, FontStyles.Bold, new Color(1f, 0.84f, 0.45f), TextAlignmentOptions.Left);
            SetAnchor(pNameText.rectTransform, 0, 1, 0, 1, 0, 1);
            pNameText.rectTransform.anchoredPosition = new Vector2(125, -16);
            pNameText.rectTransform.sizeDelta = new Vector2(180, 24);

            // Player Health Value Text
            var pHealthValText = CreateTMPText("Text_HealthVal", playerFrameGO.transform, "1000 / 1000", fontAsset, 13, FontStyles.Bold, new Color(0.9f, 0.9f, 0.9f), TextAlignmentOptions.Right);
            SetAnchor(pHealthValText.rectTransform, 0, 1, 0, 1, 1, 1);
            pHealthValText.rectTransform.anchoredPosition = new Vector2(480, -16);
            pHealthValText.rectTransform.sizeDelta = new Vector2(160, 24);

            // Health Channel Cradle
            var pCradleGO = CreateUIObject("Health_Channel_Cradle", playerFrameGO.transform);
            var pCradleRT = pCradleGO.GetComponent<RectTransform>();
            SetAnchor(pCradleRT, 0, 1, 0, 1, 0, 1);
            pCradleRT.anchoredPosition = new Vector2(125, -44);
            pCradleRT.sizeDelta = new Vector2(360, 22);
            pCradleGO.AddComponent<Image>().sprite = spChannelCradle;

            // 8 Player Emerald Blocks
            var playerBlocks = new Image[8];
            float blockWidth = 41.5f;
            float blockHeight = 16f;
            float blockSpacing = 2.8f;
            float startX = 5f;
            for (int i = 0; i < 8; i++)
            {
                var blockGO = CreateUIObject($"Block_{i}", pCradleGO.transform);
                var blockRT = blockGO.GetComponent<RectTransform>();
                SetAnchor(blockRT, 0, 0.5f, 0, 0.5f, 0, 0.5f);
                blockRT.anchoredPosition = new Vector2(startX + i * (blockWidth + blockSpacing), 0);
                blockRT.sizeDelta = new Vector2(blockWidth, blockHeight);
                var img = blockGO.AddComponent<Image>();
                img.sprite = spEmeraldBlock;
                playerBlocks[i] = img;
            }

            // Ability Sockets Row (Bottom row inside Bastion Frame)
            var pSocketsGO = CreateUIObject("Ability_Sockets_Row", playerFrameGO.transform);
            var pSocketsRT = pSocketsGO.GetComponent<RectTransform>();
            SetAnchor(pSocketsRT, 0, 1, 0, 1, 0, 1);
            pSocketsRT.anchoredPosition = new Vector2(125, -70);
            pSocketsRT.sizeDelta = new Vector2(350, 48);

            string[] hotkeys = new string[] { "Q", "V", "E" };
            string[] abilityNames = new string[] { "Fire Breath", "Tail Whip", "Sky Dive" };
            Sprite[] abilityIcons = new Sprite[] { spIconFireBreath, spIconTailWhip, spIconFlyDive };
            var abilitySlots = new AbilitySlotUI[3];

            for (int i = 0; i < 3; i++)
            {
                var socketGO = CreateUIObject($"AbilitySlot_{i}", pSocketsGO.transform);
                var socketRT = socketGO.GetComponent<RectTransform>();
                SetAnchor(socketRT, 0, 0.5f, 0, 0.5f, 0, 0.5f);
                socketRT.anchoredPosition = new Vector2(i * 115f, 0);
                socketRT.sizeDelta = new Vector2(46, 46);

                var slotUI = socketGO.AddComponent<AbilitySlotUI>();
                abilitySlots[i] = slotUI;

                // Socket Border Image
                var borderImg = socketGO.AddComponent<Image>();
                borderImg.sprite = spSocketReadyGreen;

                // Ability Icon
                var iconGO = CreateUIObject("Icon", socketGO.transform);
                var iconRT = iconGO.GetComponent<RectTransform>();
                SetAnchor(iconRT, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
                iconRT.anchoredPosition = Vector2.zero;
                iconRT.sizeDelta = new Vector2(34, 34);
                var iconImg = iconGO.AddComponent<Image>();
                iconImg.sprite = abilityIcons[i];

                // Cooldown Radial Overlay
                var cdGO = CreateUIObject("CooldownOverlay", socketGO.transform);
                var cdRT = cdGO.GetComponent<RectTransform>();
                SetAnchor(cdRT, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
                cdRT.anchoredPosition = Vector2.zero;
                cdRT.sizeDelta = new Vector2(34, 34);
                var cdImg = cdGO.AddComponent<Image>();
                cdImg.color = new Color(0.08f, 0.08f, 0.08f, 0.82f);
                cdImg.type = Image.Type.Filled;
                cdImg.fillMethod = Image.FillMethod.Radial360;
                cdImg.fillClockwise = false;
                cdImg.fillAmount = 0f;
                cdGO.SetActive(false);

                // Cooldown Number Text
                var cdText = CreateTMPText("Text_Cooldown", socketGO.transform, "", fontAsset, 16, FontStyles.Bold, new Color(1f, 0.9f, 0.2f), TextAlignmentOptions.Center);
                cdText.rectTransform.anchoredPosition = Vector2.zero;
                cdText.rectTransform.sizeDelta = new Vector2(46, 46);
                cdText.gameObject.SetActive(false);

                // Hotkey Jewel Diamond
                var jewelGO = CreateUIObject("Jewel_Hotkey", socketGO.transform);
                var jewelRT = jewelGO.GetComponent<RectTransform>();
                SetAnchor(jewelRT, 1, 0, 1, 0, 0.5f, 0.5f);
                jewelRT.anchoredPosition = new Vector2(-4, 4);
                jewelRT.sizeDelta = new Vector2(18, 18);
                jewelGO.AddComponent<Image>().sprite = spJewelDiamond;

                var hotkeyText = CreateTMPText("Text_Hotkey", jewelGO.transform, hotkeys[i], fontAsset, 10, FontStyles.Bold, new Color(1f, 0.88f, 0.35f), TextAlignmentOptions.Center);
                hotkeyText.rectTransform.anchoredPosition = Vector2.zero;
                hotkeyText.rectTransform.sizeDelta = new Vector2(18, 18);

                // Ability Name Label
                var nameText = CreateTMPText("Text_AbilityName", socketGO.transform, abilityNames[i], fontAsset, 10, FontStyles.Normal, new Color(0.85f, 0.8f, 0.7f), TextAlignmentOptions.Left);
                SetAnchor(nameText.rectTransform, 0, 0.5f, 0, 0.5f, 0, 0.5f);
                nameText.rectTransform.anchoredPosition = new Vector2(50, 0);
                nameText.rectTransform.sizeDelta = new Vector2(65, 30);

                // Serialize AbilitySlotUI fields
                var soSlot = new SerializedObject(slotUI);
                soSlot.FindProperty("_iconImage").objectReferenceValue = iconImg;
                soSlot.FindProperty("_socketBorderImage").objectReferenceValue = borderImg;
                soSlot.FindProperty("_cooldownRadialOverlay").objectReferenceValue = cdImg;
                soSlot.FindProperty("_cooldownText").objectReferenceValue = cdText;
                soSlot.FindProperty("_hotkeyText").objectReferenceValue = hotkeyText;
                soSlot.FindProperty("_abilityNameText").objectReferenceValue = nameText;
                soSlot.FindProperty("_readySocketSprite").objectReferenceValue = spSocketReadyGreen;
                soSlot.FindProperty("_cooldownSocketSprite").objectReferenceValue = spSocketCooldownAmber;
                soSlot.ApplyModifiedProperties();
            }

            // 7. Build AI HUD (Top-Right Mirrored)
            var aiFrameGO = CreateUIObject("AI_DragonBastion_Frame", canvasGO.transform);
            var aiFrameRT = aiFrameGO.GetComponent<RectTransform>();
            SetAnchor(aiFrameRT, 1, 1, 1, 1, 1, 1);
            aiFrameRT.anchoredPosition = new Vector2(-30, -30);
            aiFrameRT.sizeDelta = new Vector2(500, 120);
            aiFrameGO.AddComponent<Image>().sprite = spAIFrame;

            // AI Portrait Chassis
            var aiPortraitGO = CreateUIObject("Chassis_Portrait", aiFrameGO.transform);
            var aiPortraitRT = aiPortraitGO.GetComponent<RectTransform>();
            SetAnchor(aiPortraitRT, 1, 0.5f, 1, 0.5f, 0.5f, 0.5f);
            aiPortraitRT.anchoredPosition = new Vector2(-62, 0);
            aiPortraitRT.sizeDelta = new Vector2(100, 100);
            aiPortraitGO.AddComponent<Image>().sprite = spAIChassis;

            // AI Dragon Icon
            var aiIconGO = CreateUIObject("Icon_Dragon", aiPortraitGO.transform);
            var aiIconRT = aiIconGO.GetComponent<RectTransform>();
            SetAnchor(aiIconRT, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
            aiIconRT.anchoredPosition = Vector2.zero;
            aiIconRT.sizeDelta = new Vector2(84, 84);
            aiIconGO.AddComponent<Image>().sprite = spAIDragon;

            // AI Badge Shield
            var aiBadgeGO = CreateUIObject("Badge_LevelShield", aiPortraitGO.transform);
            var aiBadgeRT = aiBadgeGO.GetComponent<RectTransform>();
            SetAnchor(aiBadgeRT, 0, 0, 0, 0, 0.5f, 0.5f);
            aiBadgeRT.anchoredPosition = new Vector2(12, 12);
            aiBadgeRT.sizeDelta = new Vector2(28, 28);
            aiBadgeGO.AddComponent<Image>().sprite = spBadgeShield;

            var aiLvlText = CreateTMPText("Text_Level", aiBadgeGO.transform, "1", fontAsset, 12, FontStyles.Bold, new Color(0.6f, 0.9f, 1f), TextAlignmentOptions.Center);
            aiLvlText.rectTransform.anchoredPosition = Vector2.zero;
            aiLvlText.rectTransform.sizeDelta = new Vector2(28, 28);

            // AI Dragon Name Text
            var aiNameText = CreateTMPText("Text_DragonName", aiFrameGO.transform, "FROST WYRM", fontAsset, 15, FontStyles.Bold, new Color(0.55f, 0.88f, 1f), TextAlignmentOptions.Right);
            SetAnchor(aiNameText.rectTransform, 1, 1, 1, 1, 1, 1);
            aiNameText.rectTransform.anchoredPosition = new Vector2(-125, -16);
            aiNameText.rectTransform.sizeDelta = new Vector2(180, 24);

            // AI Health Value Text
            var aiHealthValText = CreateTMPText("Text_HealthVal", aiFrameGO.transform, "1000 / 1000", fontAsset, 13, FontStyles.Bold, new Color(0.9f, 0.9f, 0.9f), TextAlignmentOptions.Left);
            SetAnchor(aiHealthValText.rectTransform, 1, 1, 1, 1, 0, 1);
            aiHealthValText.rectTransform.anchoredPosition = new Vector2(-480, -16);
            aiHealthValText.rectTransform.sizeDelta = new Vector2(160, 24);

            // AI Health Cradle
            var aiCradleGO = CreateUIObject("Health_Channel_Cradle", aiFrameGO.transform);
            var aiCradleRT = aiCradleGO.GetComponent<RectTransform>();
            SetAnchor(aiCradleRT, 1, 1, 1, 1, 1, 1);
            aiCradleRT.anchoredPosition = new Vector2(-125, -44);
            aiCradleRT.sizeDelta = new Vector2(360, 22);
            aiCradleGO.AddComponent<Image>().sprite = spChannelCradle;

            // 8 AI Crimson Blocks
            var aiBlocks = new Image[8];
            for (int i = 0; i < 8; i++)
            {
                var blockGO = CreateUIObject($"Block_{i}", aiCradleGO.transform);
                var blockRT = blockGO.GetComponent<RectTransform>();
                SetAnchor(blockRT, 1, 0.5f, 1, 0.5f, 1, 0.5f);
                blockRT.anchoredPosition = new Vector2(-(startX + (7 - i) * (blockWidth + blockSpacing)), 0);
                blockRT.sizeDelta = new Vector2(blockWidth, blockHeight);
                var img = blockGO.AddComponent<Image>();
                img.sprite = spCrimsonBlock;
                aiBlocks[i] = img;
            }

            // AI Subtitle Label
            var aiSubtitle = CreateTMPText("Text_AISubtitle", aiFrameGO.transform, "ANCIENT FROST ARCHON [BOSS]", fontAsset, 10, FontStyles.Italic, new Color(0.6f, 0.82f, 1f, 0.7f), TextAlignmentOptions.Right);
            SetAnchor(aiSubtitle.rectTransform, 1, 1, 1, 1, 1, 1);
            aiSubtitle.rectTransform.anchoredPosition = new Vector2(-125, -78);
            aiSubtitle.rectTransform.sizeDelta = new Vector2(250, 20);

            // 8. Build Winner Screen Panel (Variant 1: Sovereign Modal)
            var winnerPanelGO = CreateUIObject("Winner_Screen_Panel", canvasGO.transform);
            var winnerPanelRT = winnerPanelGO.GetComponent<RectTransform>();
            SetAnchor(winnerPanelRT, 0, 0, 1, 1, 0.5f, 0.5f);
            winnerPanelRT.sizeDelta = Vector2.zero;
            winnerPanelRT.anchoredPosition = Vector2.zero;
            var winnerBg = winnerPanelGO.AddComponent<Image>();
            winnerBg.color = new Color(0.04f, 0.05f, 0.07f, 0.90f);

            // Centered Modal Card (640 x 680)
            var modalGO = CreateUIObject("Modal_Sovereign_Emblem", winnerPanelGO.transform);
            var modalRT = modalGO.GetComponent<RectTransform>();
            SetAnchor(modalRT, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
            modalRT.sizeDelta = new Vector2(640, 680);
            modalRT.anchoredPosition = Vector2.zero;
            var modalImg = modalGO.AddComponent<Image>();
            modalImg.sprite = spVicModal;

            // Top Crest Horns
            var crestGO = CreateUIObject("Crest_Horns", modalGO.transform);
            var crestRT = crestGO.GetComponent<RectTransform>();
            SetAnchor(crestRT, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f);
            crestRT.anchoredPosition = new Vector2(0, -32);
            crestRT.sizeDelta = new Vector2(60, 26);
            crestGO.AddComponent<Image>().sprite = spVicCrest;

            // Title "VICTORY"
            var winnerTitle = CreateTMPText("Text_WinnerTitle", modalGO.transform, "VICTORY", fontAsset, 48, FontStyles.Bold, new Color(1f, 0.86f, 0.25f), TextAlignmentOptions.Center);
            SetAnchor(winnerTitle.rectTransform, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f);
            winnerTitle.rectTransform.anchoredPosition = new Vector2(0, -68);
            winnerTitle.rectTransform.sizeDelta = new Vector2(500, 60);
            winnerTitle.characterSpacing = 4f;

            // Subtitle "INFERNO WYRM ASCENDANT"
            var winnerSubtitle = CreateTMPText("Text_WinnerSubtitle", modalGO.transform, "INFERNO WYRM ASCENDANT", fontAsset, 13, FontStyles.Bold, new Color(0.85f, 0.75f, 0.55f), TextAlignmentOptions.Center);
            SetAnchor(winnerSubtitle.rectTransform, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f);
            winnerSubtitle.rectTransform.anchoredPosition = new Vector2(0, -132);
            winnerSubtitle.rectTransform.sizeDelta = new Vector2(400, 24);
            winnerSubtitle.characterSpacing = 2f;

            // Portrait Chassis (110 x 110)
            var portraitGO = CreateUIObject("Chassis_Portrait", modalGO.transform);
            var portraitRT = portraitGO.GetComponent<RectTransform>();
            SetAnchor(portraitRT, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f);
            portraitRT.anchoredPosition = new Vector2(0, -165);
            portraitRT.sizeDelta = new Vector2(110, 110);
            portraitGO.AddComponent<Image>().sprite = spVicChassis;

            // Dragon Icon inside Portrait
            var dragonIconGO = CreateUIObject("Icon_Dragon", portraitGO.transform);
            var dragonIconRT = dragonIconGO.GetComponent<RectTransform>();
            SetAnchor(dragonIconRT, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
            dragonIconRT.anchoredPosition = Vector2.zero;
            dragonIconRT.sizeDelta = new Vector2(86, 86);
            var dragonIconImg = dragonIconGO.AddComponent<Image>();
            dragonIconImg.sprite = spPlayerDragon;

            // Opponent Vanquished Note
            var opponentText = CreateTMPText("Text_OpponentNote", modalGO.transform, "Frost Wyrm Vanquished in Arena Combat", fontAsset, 12, FontStyles.Normal, new Color(0.6f, 0.65f, 0.72f), TextAlignmentOptions.Center);
            SetAnchor(opponentText.rectTransform, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f);
            opponentText.rectTransform.anchoredPosition = new Vector2(0, -295);
            opponentText.rectTransform.sizeDelta = new Vector2(400, 22);

            // Stat Strip Container (520 x 95)
            var statStripGO = CreateUIObject("Stat_Strip", modalGO.transform);
            var statStripRT = statStripGO.GetComponent<RectTransform>();
            SetAnchor(statStripRT, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f);
            statStripRT.anchoredPosition = new Vector2(0, -330);
            statStripRT.sizeDelta = new Vector2(520, 95);
            statStripGO.AddComponent<Image>().sprite = spVicStatStrip;

            // 3 Stat Columns
            // Col 0: Battle Time
            var colTime = CreateUIObject("Col_Time", statStripGO.transform);
            var colTimeRT = colTime.GetComponent<RectTransform>();
            SetAnchor(colTimeRT, 0, 0, 0, 1, 0, 0.5f);
            colTimeRT.anchoredPosition = new Vector2(0, 0);
            colTimeRT.sizeDelta = new Vector2(173, 0);
            var lblTime = CreateTMPText("Label", colTime.transform, "BATTLE TIME", fontAsset, 10, FontStyles.Bold, new Color(0.55f, 0.6f, 0.68f), TextAlignmentOptions.Center);
            lblTime.rectTransform.anchoredPosition = new Vector2(0, 20);
            lblTime.rectTransform.sizeDelta = new Vector2(170, 20);
            lblTime.characterSpacing = 1.5f;
            var statTimeText = CreateTMPText("Text_StatTime", colTime.transform, "01:42", fontAsset, 24, FontStyles.Bold, new Color(1f, 0.82f, 0.35f), TextAlignmentOptions.Center);
            statTimeText.rectTransform.anchoredPosition = new Vector2(0, -10);
            statTimeText.rectTransform.sizeDelta = new Vector2(170, 32);

            // Col 1: Total Damage
            var colDmg = CreateUIObject("Col_Damage", statStripGO.transform);
            var colDmgRT = colDmg.GetComponent<RectTransform>();
            SetAnchor(colDmgRT, 0.5f, 0, 0.5f, 1, 0.5f, 0.5f);
            colDmgRT.anchoredPosition = Vector2.zero;
            colDmgRT.sizeDelta = new Vector2(173, 0);
            var lblDmg = CreateTMPText("Label", colDmg.transform, "TOTAL DAMAGE", fontAsset, 10, FontStyles.Bold, new Color(0.55f, 0.6f, 0.68f), TextAlignmentOptions.Center);
            lblDmg.rectTransform.anchoredPosition = new Vector2(0, 20);
            lblDmg.rectTransform.sizeDelta = new Vector2(170, 20);
            lblDmg.characterSpacing = 1.5f;
            var statDamageText = CreateTMPText("Text_StatDamage", colDmg.transform, "1,450", fontAsset, 24, FontStyles.Bold, new Color(1f, 0.45f, 0.2f), TextAlignmentOptions.Center);
            statDamageText.rectTransform.anchoredPosition = new Vector2(0, -10);
            statDamageText.rectTransform.sizeDelta = new Vector2(170, 32);

            // Col 2: Surviving Health
            var colHp = CreateUIObject("Col_Health", statStripGO.transform);
            var colHpRT = colHp.GetComponent<RectTransform>();
            SetAnchor(colHpRT, 1, 0, 1, 1, 1, 0.5f);
            colHpRT.anchoredPosition = Vector2.zero;
            colHpRT.sizeDelta = new Vector2(173, 0);
            var lblHp = CreateTMPText("Label", colHp.transform, "SURVIVING HP", fontAsset, 10, FontStyles.Bold, new Color(0.55f, 0.6f, 0.68f), TextAlignmentOptions.Center);
            lblHp.rectTransform.anchoredPosition = new Vector2(0, 20);
            lblHp.rectTransform.sizeDelta = new Vector2(170, 20);
            lblHp.characterSpacing = 1.5f;
            var statHealthText = CreateTMPText("Text_StatHealth", colHp.transform, "75%", fontAsset, 24, FontStyles.Bold, new Color(0.3f, 0.85f, 0.45f), TextAlignmentOptions.Center);
            statHealthText.rectTransform.anchoredPosition = new Vector2(0, -10);
            statHealthText.rectTransform.sizeDelta = new Vector2(170, 32);

            // Primary Button: REMATCH (280 x 56)
            var rematchBtnGO = CreateUIObject("Button_Rematch", modalGO.transform);
            var rematchBtnRT = rematchBtnGO.GetComponent<RectTransform>();
            SetAnchor(rematchBtnRT, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f);
            rematchBtnRT.anchoredPosition = new Vector2(0, -475);
            rematchBtnRT.sizeDelta = new Vector2(280, 56);
            var rematchBtnImg = rematchBtnGO.AddComponent<Image>();
            rematchBtnImg.sprite = spVicBtnRematch;
            var rematchBtn = rematchBtnGO.AddComponent<Button>();

            var rematchText = CreateTMPText("Text_Rematch", rematchBtnGO.transform, "REMATCH", fontAsset, 17, FontStyles.Bold, new Color(0.12f, 0.06f, 0.02f), TextAlignmentOptions.Center);
            rematchText.rectTransform.anchoredPosition = Vector2.zero;
            rematchText.rectTransform.sizeDelta = new Vector2(280, 56);
            rematchText.characterSpacing = 2.5f;

            // Secondary Button: LEAVE ARENA (200 x 38)
            var leaveBtnGO = CreateUIObject("Button_Leave", modalGO.transform);
            var leaveBtnRT = leaveBtnGO.GetComponent<RectTransform>();
            SetAnchor(leaveBtnRT, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f);
            leaveBtnRT.anchoredPosition = new Vector2(0, -550);
            leaveBtnRT.sizeDelta = new Vector2(200, 38);
            var leaveBtnImg = leaveBtnGO.AddComponent<Image>();
            leaveBtnImg.sprite = spVicBtnLeave;
            var leaveBtn = leaveBtnGO.AddComponent<Button>();

            var leaveText = CreateTMPText("Text_Leave", leaveBtnGO.transform, "LEAVE ARENA", fontAsset, 12, FontStyles.Bold, new Color(0.65f, 0.7f, 0.78f), TextAlignmentOptions.Center);
            leaveText.rectTransform.anchoredPosition = Vector2.zero;
            leaveText.rectTransform.sizeDelta = new Vector2(200, 38);
            leaveText.characterSpacing = 2f;

            winnerPanelGO.SetActive(false);

            // 9. Hook References to CombatHUD
            var soHUD = new SerializedObject(combatHUD);

            // Player References
            var playerGO = GameObject.Find("Combatants/PlayerDragon");
            if (playerGO != null)
            {
                soHUD.FindProperty("_playerHealth").objectReferenceValue = playerGO.GetComponent<DragonHealth>();
                soHUD.FindProperty("_playerCombat").objectReferenceValue = playerGO.GetComponent<DragonCombat>();
            }
            soHUD.FindProperty("_playerNameText").objectReferenceValue = pNameText;
            soHUD.FindProperty("_playerHealthValText").objectReferenceValue = pHealthValText;
            soHUD.FindProperty("_playerBlockActiveSprite").objectReferenceValue = spEmeraldBlock;
            soHUD.FindProperty("_playerBlockGhostSprite").objectReferenceValue = spGhostAmberBlock;
            soHUD.FindProperty("_playerBlockDepletedSprite").objectReferenceValue = spDepletedBlock;

            var pBlocksProp = soHUD.FindProperty("_playerHealthBlocks");
            pBlocksProp.arraySize = 8;
            for (int i = 0; i < 8; i++) pBlocksProp.GetArrayElementAtIndex(i).objectReferenceValue = playerBlocks[i];

            // AI References
            var aiGO = GameObject.Find("Combatants/AIDragon");
            if (aiGO != null)
            {
                soHUD.FindProperty("_aiHealth").objectReferenceValue = aiGO.GetComponent<DragonHealth>();
            }
            soHUD.FindProperty("_aiNameText").objectReferenceValue = aiNameText;
            soHUD.FindProperty("_aiHealthValText").objectReferenceValue = aiHealthValText;
            soHUD.FindProperty("_aiBlockActiveSprite").objectReferenceValue = spCrimsonBlock;
            soHUD.FindProperty("_aiBlockGhostSprite").objectReferenceValue = spGhostAmberBlock;
            soHUD.FindProperty("_aiBlockDepletedSprite").objectReferenceValue = spDepletedBlock;

            var aiBlocksProp = soHUD.FindProperty("_aiHealthBlocks");
            aiBlocksProp.arraySize = 8;
            for (int i = 0; i < 8; i++) aiBlocksProp.GetArrayElementAtIndex(i).objectReferenceValue = aiBlocks[i];

            // Ability Slots
            var slotsProp = soHUD.FindProperty("_abilitySlots");
            slotsProp.arraySize = 3;
            for (int i = 0; i < 3; i++) slotsProp.GetArrayElementAtIndex(i).objectReferenceValue = abilitySlots[i];

            // Feedback & Winner Screen
            var dmgPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/UI/DamageNumberPopup.prefab");
            soHUD.FindProperty("_damageNumberPrefab").objectReferenceValue = dmgPrefab;
            soHUD.FindProperty("_winnerScreenPanel").objectReferenceValue = winnerPanelGO;
            soHUD.FindProperty("_winnerTitleText").objectReferenceValue = winnerTitle;
            soHUD.FindProperty("_winnerSubtitleText").objectReferenceValue = winnerSubtitle;
            soHUD.FindProperty("_winnerDragonIcon").objectReferenceValue = dragonIconImg;
            soHUD.FindProperty("_winnerOpponentNote").objectReferenceValue = opponentText;
            soHUD.FindProperty("_statTimeText").objectReferenceValue = statTimeText;
            soHUD.FindProperty("_statDamageText").objectReferenceValue = statDamageText;
            soHUD.FindProperty("_statHealthText").objectReferenceValue = statHealthText;
            soHUD.FindProperty("_restartButton").objectReferenceValue = rematchBtn;
            soHUD.FindProperty("_leaveButton").objectReferenceValue = leaveBtn;

            soHUD.ApplyModifiedProperties();

            // 10. Hook BattleGameManager._combatHUD
            var bgm = Object.FindFirstObjectByType<DexHigh.Core.BattleGameManager>();
            if (bgm != null)
            {
                var soBGM = new SerializedObject(bgm);
                var propHUD = soBGM.FindProperty("_combatHUD");
                if (propHUD != null)
                {
                    propHUD.objectReferenceValue = combatHUD;
                    soBGM.ApplyModifiedProperties();
                }
            }

            // 11. Save as Prefab for future reuse
            string prefabPath = "Assets/_Project/Prefabs/UI/CombatHUD_Canvas.prefab";
            PrefabUtility.SaveAsPrefabAssetAndConnect(canvasGO, prefabPath, InteractionMode.AutomatedAction);

            // 12. Mark scene dirty and save
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveScene(activeScene);
            AssetDatabase.SaveAssets();

            Debug.Log("<color=green><b>[DexHigh UI]</b> Dota 2 Combat HUD successfully built, assigned, and saved to scene!</color>");
        }

        private static GameObject CreateUIObject(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            return go;
        }

        private static void SetAnchor(RectTransform rt, float minX, float minY, float maxX, float maxY, float pivotX, float pivotY)
        {
            rt.anchorMin = new Vector2(minX, minY);
            rt.anchorMax = new Vector2(maxX, maxY);
            rt.pivot = new Vector2(pivotX, pivotY);
        }

        private static Sprite LoadSprite(string fileName)
        {
            string path = SPRITES_PATH + fileName;
            var sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sp == null) Debug.LogWarning($"[DexHigh UI] Failed to load sprite at {path}");
            return sp;
        }

        private static Sprite LoadVictorySprite(string fileName)
        {
            string path = VICTORY_PATH + fileName;
            var sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sp == null) Debug.LogWarning($"[DexHigh UI] Failed to load victory sprite at {path}");
            return sp;
        }

        private static TextMeshProUGUI CreateTMPText(string name, Transform parent, string text, TMP_FontAsset font, float size, FontStyles style, Color color, TextAlignmentOptions align)
        {
            var go = CreateUIObject(name, parent);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            if (font != null) tmp.font = font;
            tmp.text = text;
            tmp.fontSize = size;
            tmp.fontStyle = style;
            tmp.color = color;
            tmp.alignment = align;
            return tmp;
        }

        private static void EnsureEventSystem()
        {
            var es = Object.FindFirstObjectByType<EventSystem>();
            if (es == null)
            {
                var esGO = new GameObject("EventSystem");
                es = esGO.AddComponent<EventSystem>();
                esGO.AddComponent<InputSystemUIInputModule>();
            }
        }
    }
}
