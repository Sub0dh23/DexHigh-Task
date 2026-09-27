using System.IO;
using DexHigh.AI;
using DexHigh.Characters;
using DexHigh.Combat;
using DexHigh.CombatCamera;
using DexHigh.Core;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DexHigh.Editor
{
    [InitializeOnLoad]
    public static class ArenaBuilder
    {
        private const string RootDir = "Assets/_Project";
        private const string MatDir = "Assets/_Project/Materials";
        private const string SODir = "Assets/_Project/ScriptableObjects";
        private const string PrefabDir = "Assets/_Project/Prefabs";
        private const string VFXDir = "Assets/_Project/VFX";
        private const string SceneDir = "Assets/_Project/Scenes";
        private const string BuiltKey = "DexHigh_ArenaBuilder_Built";

        static ArenaBuilder()
        {
            EditorApplication.delayCall += () =>
            {
                if (!SessionState.GetBool(BuiltKey, false))
                {
                    SessionState.SetBool(BuiltKey, true);
                    BuildAll();
                }
            };
        }

        [MenuItem("DexHigh/Build Battle Arena & Prefabs")]
        public static void BuildAll()
        {
            EnsureDirectories();
            var mats = CreateMaterials();
            var vfx = CreateVFXPrefabs(mats.vfxMat);
            var abilities = CreateAbilityData(vfx);
            var prefabs = CreateDragonPrefabs(mats, abilities);
            BuildBattleScene(prefabs, mats);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=#2ECC71><b>[DexHigh]</b> Battle Arena, Dragon Prefabs, and VFX successfully built!</color>");
        }

        private static void EnsureDirectories()
        {
            string[] dirs = { RootDir, MatDir, SODir, PrefabDir, VFXDir, SceneDir };
            foreach (var dir in dirs)
            {
                if (!AssetDatabase.IsValidFolder(dir))
                {
                    string parent = Path.GetDirectoryName(dir).Replace('\\', '/');
                    string leaf = Path.GetFileName(dir);
                    AssetDatabase.CreateFolder(parent, leaf);
                }
            }
        }

        private struct MaterialSet
        {
            public Material playerMat;
            public Material aiMat;
            public Material floorMat;
            public Material trimMat;
            public Material pillarMat;
            public Material vfxMat;
        }

        private static MaterialSet CreateMaterials()
        {
            MaterialSet set = new MaterialSet();
            Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
            Shader urpParticles = Shader.Find("Universal Render Pipeline/Particles/Unlit");

            if (urpLit == null) urpLit = Shader.Find("Standard");
            if (urpParticles == null) urpParticles = Shader.Find("Mobile/Particles/Additive");

            // Textures
            Texture2D redBase = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Models/RedDragon/textures/redDragon_Base_color.png");
            Texture2D redEm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Models/RedDragon/textures/redDragon_Emission_color.png");
            Texture2D blueBase = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Models/RedDragon/textures/blueDragon_Base_color.png");
            Texture2D blueEm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Models/RedDragon/textures/blueDragon_Emission_color.png");
            Texture2D normalMap = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Models/RedDragon/textures/redDragon_Normal_OpenGL.png");
            Texture2D roughnessMap = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Models/RedDragon/textures/redDragon_Specular_roughness.png");
            Texture2D aoMap = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Models/RedDragon/textures/redDragon_Mixed_AO.png");

            // 1. Player Red Dragon Material
            string playerMatPath = $"{MatDir}/M_RedDragon_Player.mat";
            Material playerMat = AssetDatabase.LoadAssetAtPath<Material>(playerMatPath);
            if (playerMat == null)
            {
                playerMat = new Material(urpLit);
                AssetDatabase.CreateAsset(playerMat, playerMatPath);
            }
            playerMat.name = "M_RedDragon_Player";
            if (redBase != null) playerMat.SetTexture("_BaseMap", redBase);
            if (normalMap != null)
            {
                playerMat.SetTexture("_BumpMap", normalMap);
                playerMat.EnableKeyword("_NORMALMAP");
            }
            if (aoMap != null)
            {
                playerMat.SetTexture("_OcclusionMap", aoMap);
                playerMat.SetFloat("_OcclusionStrength", 0.8f);
            }
            if (redEm != null)
            {
                playerMat.SetTexture("_EmissionMap", redEm);
                playerMat.SetColor("_EmissionColor", new Color(1f, 0.4f, 0.1f) * 1.5f);
                playerMat.EnableKeyword("_EMISSION");
            }
            playerMat.SetFloat("_Smoothness", 0.45f);
            set.playerMat = playerMat;

            // 2. AI Blue Frost Dragon Material
            string aiMatPath = $"{MatDir}/M_BlueDragon_AI.mat";
            Material aiMat = AssetDatabase.LoadAssetAtPath<Material>(aiMatPath);
            if (aiMat == null)
            {
                aiMat = new Material(urpLit);
                AssetDatabase.CreateAsset(aiMat, aiMatPath);
            }
            aiMat.name = "M_BlueDragon_AI";
            if (blueBase != null) aiMat.SetTexture("_BaseMap", blueBase);
            if (normalMap != null)
            {
                aiMat.SetTexture("_BumpMap", normalMap);
                aiMat.EnableKeyword("_NORMALMAP");
            }
            if (aoMap != null)
            {
                aiMat.SetTexture("_OcclusionMap", aoMap);
                aiMat.SetFloat("_OcclusionStrength", 0.8f);
            }
            if (blueEm != null)
            {
                aiMat.SetTexture("_EmissionMap", blueEm);
                aiMat.SetColor("_EmissionColor", new Color(0.1f, 0.7f, 1f) * 1.8f);
                aiMat.EnableKeyword("_EMISSION");
            }
            aiMat.SetFloat("_Smoothness", 0.45f);
            set.aiMat = aiMat;

            // 3. Arena Floor Material (Sandy Ochre Gladiatorial Pit)
            string floorMatPath = $"{MatDir}/M_ArenaFloor.mat";
            Material floorMat = AssetDatabase.LoadAssetAtPath<Material>(floorMatPath);
            if (floorMat == null)
            {
                floorMat = new Material(urpLit);
                AssetDatabase.CreateAsset(floorMat, floorMatPath);
            }
            floorMat.name = "M_ArenaFloor";
            floorMat.SetColor("_BaseColor", new Color(0.76f, 0.58f, 0.38f));
            floorMat.SetFloat("_Smoothness", 0.12f);
            set.floorMat = floorMat;

            // 4. Arena Trim Material (Roman Stone Trim)
            string trimMatPath = $"{MatDir}/M_ArenaTrim.mat";
            Material trimMat = AssetDatabase.LoadAssetAtPath<Material>(trimMatPath);
            if (trimMat == null)
            {
                trimMat = new Material(urpLit);
                AssetDatabase.CreateAsset(trimMat, trimMatPath);
            }
            trimMat.name = "M_ArenaTrim";
            trimMat.SetColor("_BaseColor", new Color(0.50f, 0.38f, 0.27f));
            trimMat.SetFloat("_Metallic", 0.0f);
            trimMat.SetFloat("_Smoothness", 0.28f);
            set.trimMat = trimMat;

            // 5. Pillar Material (Weathered Travertine Colosseum Stone)
            string pillarMatPath = $"{MatDir}/M_ArenaPillar.mat";
            Material pillarMat = AssetDatabase.LoadAssetAtPath<Material>(pillarMatPath);
            if (pillarMat == null)
            {
                pillarMat = new Material(urpLit);
                AssetDatabase.CreateAsset(pillarMat, pillarMatPath);
            }
            pillarMat.name = "M_ArenaPillar";
            pillarMat.SetColor("_BaseColor", new Color(0.62f, 0.48f, 0.34f));
            pillarMat.SetFloat("_Smoothness", 0.22f);
            set.pillarMat = pillarMat;

            // 6. VFX Material
            string vfxMatPath = $"{MatDir}/M_VFX_Additive.mat";
            Material vfxMat = AssetDatabase.LoadAssetAtPath<Material>(vfxMatPath);
            if (vfxMat == null)
            {
                vfxMat = new Material(urpParticles);
                AssetDatabase.CreateAsset(vfxMat, vfxMatPath);
            }
            vfxMat.name = "M_VFX_Additive";
            vfxMat.SetFloat("_Surface", 1f); // Transparent
            vfxMat.SetFloat("_Blend", 1f); // Additive
            vfxMat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
            vfxMat.SetFloat("_DstBlend", 10f); // OneMinusSrcAlpha
            vfxMat.SetFloat("_DstBlendAlpha", 10f);
            vfxMat.SetFloat("_ZWrite", 0f);
            vfxMat.renderQueue = 3000;
            vfxMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            vfxMat.SetOverrideTag("RenderType", "Transparent");
            Texture2D flameTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Art/VFX/T_Particle_FlameTongue.png");
            if (flameTex != null)
            {
                vfxMat.SetTexture("_BaseMap", flameTex);
                if (vfxMat.HasProperty("_MainTex")) vfxMat.SetTexture("_MainTex", flameTex);
            }
            if (vfxMat.HasProperty("_BaseColor")) vfxMat.SetColor("_BaseColor", new Color(1f, 0.6f, 0.2f, 1f));
            if (vfxMat.HasProperty("_Color")) vfxMat.SetColor("_Color", new Color(1f, 0.6f, 0.2f, 1f));
            EditorUtility.SetDirty(vfxMat);
            set.vfxMat = vfxMat;

            return set;
        }

        private struct VFXSet
        {
            public GameObject fireBreathPrefab;
            public GameObject tailWhipPrefab;
            public GameObject targetIndicatorPrefab;
            public GameObject diveImpactPrefab;
        }

        private static VFXSet CreateVFXPrefabs(Material vfxMat)
        {
            VFXSet set = new VFXSet();

            // Load existing handcrafted VFX prefabs first
            string firePath = $"{VFXDir}/VFX_FireBreath.prefab";
            set.fireBreathPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(firePath);

            string tailPath = $"{VFXDir}/VFX_TailWhip.prefab";
            set.tailWhipPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(tailPath);

            string indicatorPath = $"{VFXDir}/VFX_DiveTargetIndicator.prefab";
            set.targetIndicatorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(indicatorPath);

            string divePath = $"{VFXDir}/VFX_DiveImpactSlam.prefab";
            set.diveImpactPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(divePath);

            if (set.fireBreathPrefab != null && set.tailWhipPrefab != null &&
                set.targetIndicatorPrefab != null && set.diveImpactPrefab != null)
            {
                return set;
            }

            // Fallback generation only if prefabs do not exist
            if (set.fireBreathPrefab == null)
            {
                GameObject fireObj = new GameObject("VFX_FireBreath");
                var firePs = fireObj.AddComponent<ParticleSystem>();
                var fireMain = firePs.main;
                fireMain.startLifetime = 0.8f;
                fireMain.startSpeed = 12f;
                fireMain.startSize = new ParticleSystem.MinMaxCurve(0.4f, 1.2f);
                fireMain.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.8f, 0.2f), new Color(1f, 0.2f, 0.05f));
                fireMain.simulationSpace = ParticleSystemSimulationSpace.World;

                var fireEmission = firePs.emission;
                fireEmission.rateOverTime = 75f;

                var fireShape = firePs.shape;
                fireShape.shapeType = ParticleSystemShapeType.Cone;
                fireShape.angle = 18f;
                fireShape.radius = 0.2f;

                var fireColorBySpeed = firePs.colorOverLifetime;
                fireColorBySpeed.enabled = true;
                Gradient fireGrad = new Gradient();
                fireGrad.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.9f, 0.3f), 0f), new GradientColorKey(new Color(1f, 0.3f, 0f), 0.6f), new GradientColorKey(new Color(0.2f, 0.2f, 0.2f), 1f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.8f, 0.7f), new GradientAlphaKey(0f, 1f) }
                );
                fireColorBySpeed.color = fireGrad;

                var fireRen = fireObj.GetComponent<ParticleSystemRenderer>();
                if (vfxMat != null) fireRen.material = vfxMat;

                set.fireBreathPrefab = PrefabUtility.SaveAsPrefabAsset(fireObj, firePath);
                Object.DestroyImmediate(fireObj);
            }

            // 2. Tail Whip VFX
            if (set.tailWhipPrefab == null)
            {
                GameObject tailObj = new GameObject("VFX_TailWhip");
                var tailPs = tailObj.AddComponent<ParticleSystem>();
                var tailMain = tailPs.main;
                tailMain.duration = 0.35f;
                tailMain.loop = false;
                tailMain.startLifetime = 0.4f;
                tailMain.startSpeed = 8f;
                tailMain.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.8f);
                tailMain.startColor = new Color(1f, 0.9f, 0.5f, 0.9f);

                var tailEmission = tailPs.emission;
                tailEmission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 40) });

                var tailShape = tailPs.shape;
                tailShape.shapeType = ParticleSystemShapeType.Donut;
                tailShape.radius = 2.5f;
                tailShape.donutRadius = 0.5f;
                tailShape.arc = 180f;

                var tailRen = tailObj.GetComponent<ParticleSystemRenderer>();
                if (vfxMat != null) tailRen.material = vfxMat;

                set.tailWhipPrefab = PrefabUtility.SaveAsPrefabAsset(tailObj, tailPath);
                Object.DestroyImmediate(tailObj);
            }

            // 3. Target Indicator Decal
            if (set.targetIndicatorPrefab == null)
            {
                GameObject indObj = new GameObject("VFX_DiveTargetIndicator");
                var indPs = indObj.AddComponent<ParticleSystem>();
                var indMain = indPs.main;
                indMain.startLifetime = 0.6f;
                indMain.startSpeed = 0f;
                indMain.startSize = 5f;
                indMain.startColor = new Color(1f, 0.2f, 0.1f, 0.6f);

                var indEmission = indPs.emission;
                indEmission.rateOverTime = 15f;

                var indShape = indPs.shape;
                indShape.shapeType = ParticleSystemShapeType.Circle;
                indShape.radius = 2.5f;

                var indRen = indObj.GetComponent<ParticleSystemRenderer>();
                if (vfxMat != null) indRen.material = vfxMat;

                set.targetIndicatorPrefab = PrefabUtility.SaveAsPrefabAsset(indObj, indicatorPath);
                Object.DestroyImmediate(indObj);
            }

            // 4. Dive Impact Slam VFX
            if (set.diveImpactPrefab == null)
            {
                GameObject diveObj = new GameObject("VFX_DiveImpactSlam");
                var divePs = diveObj.AddComponent<ParticleSystem>();
                var diveMain = divePs.main;
                diveMain.duration = 0.5f;
                diveMain.loop = false;
                diveMain.startLifetime = 0.6f;
                diveMain.startSpeed = 16f;
                diveMain.startSize = new ParticleSystem.MinMaxCurve(0.5f, 1.8f);
                diveMain.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.6f, 0.1f), new Color(1f, 0.1f, 0.05f));

                var diveEmission = divePs.emission;
                diveEmission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 60) });

                var diveShape = divePs.shape;
                diveShape.shapeType = ParticleSystemShapeType.Hemisphere;
                diveShape.radius = 1.0f;

                var diveRen = diveObj.GetComponent<ParticleSystemRenderer>();
                if (vfxMat != null) diveRen.material = vfxMat;

                set.diveImpactPrefab = PrefabUtility.SaveAsPrefabAsset(diveObj, divePath);
                Object.DestroyImmediate(diveObj);
            }

            return set;
        }

        private struct AbilitySet
        {
            public AbilityData fireBreath;
            public AbilityData tailWhip;
            public AbilityData flyDive;
        }

        private static AbilitySet CreateAbilityData(VFXSet vfx)
        {
            AbilitySet set = new AbilitySet();

            // 1. Fire Breath
            string firePath = $"{SODir}/SO_Ability_FireBreath.asset";
            AbilityData fire = AssetDatabase.LoadAssetAtPath<AbilityData>(firePath);
            if (fire == null)
            {
                fire = ScriptableObject.CreateInstance<AbilityData>();
                AssetDatabase.CreateAsset(fire, firePath);
            }
            SetPrivateField(fire, "_abilityType", AbilityType.FireBreath);
            SetPrivateField(fire, "_abilityName", "Fire Breath");
            SetPrivateField(fire, "_description", "Channeled searing flame cone dealing continuous fire damage in front.");
            SetPrivateField(fire, "_defaultHotkey", KeyCode.Q);
            SetPrivateField(fire, "_baseDamage", 35f);
            SetPrivateField(fire, "_cooldown", 4.0f);
            SetPrivateField(fire, "_castDuration", 1.4f);
            SetPrivateField(fire, "_effectiveRange", 8.5f);
            SetPrivateField(fire, "_minRange", 0f);
            SetPrivateField(fire, "_knockbackForce", 4f);
            SetPrivateField(fire, "_animationTrigger", "FireBreath");
            SetPrivateField(fire, "_vfxPrefab", vfx.fireBreathPrefab);
            EditorUtility.SetDirty(fire);
            set.fireBreath = fire;

            // 2. Tail Whip
            string tailPath = $"{SODir}/SO_Ability_TailWhip.asset";
            AbilityData tail = AssetDatabase.LoadAssetAtPath<AbilityData>(tailPath);
            if (tail == null)
            {
                tail = ScriptableObject.CreateInstance<AbilityData>();
                AssetDatabase.CreateAsset(tail, tailPath);
            }
            SetPrivateField(tail, "_abilityType", AbilityType.TailWhip);
            SetPrivateField(tail, "_abilityName", "Tail Whip");
            SetPrivateField(tail, "_description", "Powerful close-range 180 sweep with high knockback.");
            SetPrivateField(tail, "_defaultHotkey", KeyCode.W);
            SetPrivateField(tail, "_baseDamage", 25f);
            SetPrivateField(tail, "_cooldown", 2.5f);
            SetPrivateField(tail, "_castDuration", 0.5f);
            SetPrivateField(tail, "_effectiveRange", 3.8f);
            SetPrivateField(tail, "_minRange", 0f);
            SetPrivateField(tail, "_knockbackForce", 9f);
            SetPrivateField(tail, "_animationTrigger", "TailWhip");
            SetPrivateField(tail, "_vfxPrefab", vfx.tailWhipPrefab);
            EditorUtility.SetDirty(tail);
            set.tailWhip = tail;

            // 3. Fly Dive
            string flyPath = $"{SODir}/SO_Ability_FlyDive.asset";
            AbilityData fly = AssetDatabase.LoadAssetAtPath<AbilityData>(flyPath);
            if (fly == null)
            {
                fly = ScriptableObject.CreateInstance<AbilityData>();
                AssetDatabase.CreateAsset(fly, flyPath);
            }
            SetPrivateField(fly, "_abilityType", AbilityType.FlyDive);
            SetPrivateField(fly, "_abilityName", "Sky Dive Slam");
            SetPrivateField(fly, "_description", "Takes flight into the skies, targets enemy location, and executes an explosive dive slam.");
            SetPrivateField(fly, "_defaultHotkey", KeyCode.E);
            SetPrivateField(fly, "_baseDamage", 55f);
            SetPrivateField(fly, "_cooldown", 8.0f);
            SetPrivateField(fly, "_castDuration", 2.0f);
            SetPrivateField(fly, "_effectiveRange", 16f);
            SetPrivateField(fly, "_minRange", 4f);
            SetPrivateField(fly, "_knockbackForce", 14f);
            SetPrivateField(fly, "_animationTrigger", "DiveBomb");
            SetPrivateField(fly, "_targetIndicatorPrefab", vfx.targetIndicatorPrefab);
            SetPrivateField(fly, "_impactVfxPrefab", vfx.diveImpactPrefab);
            EditorUtility.SetDirty(fly);
            set.flyDive = fly;

            return set;
        }

        private struct PrefabPair
        {
            public GameObject playerPrefab;
            public GameObject aiPrefab;
        }

        private static PrefabPair CreateDragonPrefabs(MaterialSet mats, AbilitySet abilities)
        {
            PrefabPair pair = new PrefabPair();
            GameObject modelFbx = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/RedDragon/source/RedDragon_Optimized.fbx");
            if (modelFbx == null)
            {
                modelFbx = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/RedDragon/source/RedDragon.fbx");
            }
            RuntimeAnimatorController animCtrl = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/_Project/Animations/Dragon_AnimatorController.controller");

            // 1. Player Dragon Prefab
            string playerPath = $"{PrefabDir}/Prefab_PlayerDragon.prefab";
            GameObject playerRoot = new GameObject("PlayerDragon");
            playerRoot.tag = "Player";

            var playerCc = playerRoot.AddComponent<CharacterController>();
            playerCc.radius = 1.3f;
            playerCc.height = 2.6f;
            playerCc.center = new Vector3(0f, 1.3f, 0f);

            var playerHealth = playerRoot.AddComponent<DragonHealth>();
            SetPrivateField(playerHealth, "_maxHealth", 200f);
            SetPrivateField(playerHealth, "_characterName", "Inferno Drake (Player)");
            SetPrivateField(playerHealth, "_isPlayer", true);

            var playerMotor = playerRoot.AddComponent<DragonMotor>();
            SetPrivateField(playerMotor, "_moveSpeed", 7.0f);
            SetPrivateField(playerMotor, "_rotationSpeed", 14.0f);

            var playerCombat = playerRoot.AddComponent<DragonCombat>();
            AbilityData[] pAbilities = new AbilityData[] { abilities.fireBreath, abilities.tailWhip, abilities.flyDive };
            SetPrivateField(playerCombat, "_abilities", pAbilities);

            playerRoot.AddComponent<PlayerDragonController>();
            var playerProcAnim = playerRoot.AddComponent<DragonProceduralAnimator>();

            // Instantiate visual child
            if (modelFbx != null)
            {
                GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(modelFbx, playerRoot.transform);
                visual.name = "Model";
                visual.transform.localPosition = Vector3.zero;
                visual.transform.localRotation = Quaternion.Euler(0f, 180f, 0f); // Face forward (+Z)
                visual.transform.localScale = Vector3.one * 0.085f;

                var anim = visual.GetComponent<Animator>();
                if (anim == null) anim = visual.AddComponent<Animator>();
                if (animCtrl != null) anim.runtimeAnimatorController = animCtrl;

                Transform jaw = null;
                Transform tail = null;
                foreach (var t in visual.GetComponentsInChildren<Transform>())
                {
                    if (t.name == "jaw" || (jaw == null && t.name == "head")) jaw = t;
                    if (t.name == "tail6" || (tail == null && t.name.StartsWith("tail"))) tail = t;
                }
                SetPrivateField(playerCombat, "_mouthTransform", jaw);
                SetPrivateField(playerCombat, "_tailTransform", tail);
                SetPrivateField(playerCombat, "_animator", anim);
                SetPrivateField(playerProcAnim, "_animator", anim);

                // Apply Player Material
                foreach (var ren in visual.GetComponentsInChildren<Renderer>())
                {
                    ren.sharedMaterial = mats.playerMat;
                }
            }

            pair.playerPrefab = PrefabUtility.SaveAsPrefabAsset(playerRoot, playerPath);
            Object.DestroyImmediate(playerRoot);

            // 2. AI Dragon Prefab
            string aiPath = $"{PrefabDir}/Prefab_AIDragon.prefab";
            GameObject aiRoot = new GameObject("AIDragon");
            aiRoot.tag = "Enemy";

            var aiCc = aiRoot.AddComponent<CharacterController>();
            aiCc.radius = 1.3f;
            aiCc.height = 2.6f;
            aiCc.center = new Vector3(0f, 1.3f, 0f);

            var aiAgent = aiRoot.AddComponent<UnityEngine.AI.NavMeshAgent>();
            aiAgent.radius = 1.3f;
            aiAgent.height = 2.6f;
            aiAgent.speed = 6.2f;
            aiAgent.stoppingDistance = 2.0f;
            aiAgent.updatePosition = false;
            aiAgent.updateRotation = false;

            var aiHealth = aiRoot.AddComponent<DragonHealth>();
            SetPrivateField(aiHealth, "_maxHealth", 200f);
            SetPrivateField(aiHealth, "_characterName", "Frost Drake (AI)");
            SetPrivateField(aiHealth, "_isPlayer", false);

            var aiMotor = aiRoot.AddComponent<DragonMotor>();
            SetPrivateField(aiMotor, "_moveSpeed", 6.2f);
            SetPrivateField(aiMotor, "_rotationSpeed", 12.0f);

            var aiCombat = aiRoot.AddComponent<DragonCombat>();
            AbilityData[] aiAbilities = new AbilityData[] { abilities.fireBreath, abilities.tailWhip, abilities.flyDive };
            SetPrivateField(aiCombat, "_abilities", aiAbilities);

            aiRoot.AddComponent<AIDragonController>();
            var aiProcAnim = aiRoot.AddComponent<DragonProceduralAnimator>();

            if (modelFbx != null)
            {
                GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(modelFbx, aiRoot.transform);
                visual.name = "Model";
                visual.transform.localPosition = Vector3.zero;
                visual.transform.localRotation = Quaternion.Euler(0f, 180f, 0f); // Face forward (+Z)
                visual.transform.localScale = Vector3.one * 0.085f;

                var anim = visual.GetComponent<Animator>();
                if (anim == null) anim = visual.AddComponent<Animator>();
                if (animCtrl != null) anim.runtimeAnimatorController = animCtrl;

                Transform jaw = null;
                Transform tail = null;
                foreach (var t in visual.GetComponentsInChildren<Transform>())
                {
                    if (t.name == "jaw" || (jaw == null && t.name == "head")) jaw = t;
                    if (t.name == "tail6" || (tail == null && t.name.StartsWith("tail"))) tail = t;
                }
                SetPrivateField(aiCombat, "_mouthTransform", jaw);
                SetPrivateField(aiCombat, "_tailTransform", tail);
                SetPrivateField(aiCombat, "_animator", anim);
                SetPrivateField(aiProcAnim, "_animator", anim);

                // Apply AI Frost Material
                foreach (var ren in visual.GetComponentsInChildren<Renderer>())
                {
                    ren.sharedMaterial = mats.aiMat;
                }
            }

            pair.aiPrefab = PrefabUtility.SaveAsPrefabAsset(aiRoot, aiPath);
            Object.DestroyImmediate(aiRoot);

            return pair;
        }

        private static void BuildBattleScene(PrefabPair prefabs, MaterialSet mats)
        {
            string scenePath = $"{SceneDir}/DragonArenaBattle.unity";
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, scenePath);

            // 1. Systems Root
            GameObject systemsRoot = new GameObject("Systems");
            var gameMgr = systemsRoot.AddComponent<BattleGameManager>();
            systemsRoot.AddComponent<DexHigh.Audio.AudioManager>();

            // 2. Environment, Golden Lighting, Volumetric God Rays & Atmosphere
            DexHigh.EditorTools.ColosseumLightingBuilder.BuildAtmosphere();

            // 3. Cameras Root
            GameObject camerasRoot = new GameObject("Cameras");
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            camObj.transform.parent = camerasRoot.transform;
            var cam = camObj.AddComponent<UnityEngine.Camera>();
            camObj.AddComponent<AudioListener>();
            camObj.AddComponent<UniversalAdditionalCameraData>();
            var combatCam = camObj.AddComponent<DynamicCombatCamera>();
            var fader = camObj.AddComponent<CameraObstructionFader>();
            SetPrivateField(combatCam, "_offset", new Vector3(0f, 20f, -16f));
            SetPrivateField(combatCam, "_minHeight", 16f);
            SetPrivateField(combatCam, "_maxHeight", 28f);
            SetPrivateField(combatCam, "_minDistance", 12f);
            SetPrivateField(combatCam, "_maxDistance", 30f);

            // 4. Combatants Root
            GameObject combatantsRoot = new GameObject("Combatants");

            GameObject playerInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabs.playerPrefab, combatantsRoot.transform);
            playerInstance.name = "PlayerDragon";
            playerInstance.transform.position = new Vector3(-9f, 0f, 0f);
            playerInstance.transform.rotation = Quaternion.LookRotation(Vector3.right);

            GameObject aiInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabs.aiPrefab, combatantsRoot.transform);
            aiInstance.name = "AIDragon";
            aiInstance.transform.position = new Vector3(9f, 0f, 0f);
            aiInstance.transform.rotation = Quaternion.LookRotation(Vector3.left);

            // Connect references
            combatCam.SetTargets(playerInstance.transform, aiInstance.transform);
            fader.SetTargets(playerInstance.transform, aiInstance.transform);
            SetPrivateField(gameMgr, "_playerDragon", playerInstance.GetComponent<DragonHealth>());
            SetPrivateField(gameMgr, "_aiDragon", aiInstance.GetComponent<DragonHealth>());
            SetPrivateField(gameMgr, "_combatCamera", combatCam);

            // 5. Tactical Caliper Mouse Aim Cursor (Player Attack Marker)
            string cursorPrefabPath = $"{PrefabDir}/VFX_TacticalCaliperCursor.prefab";
            GameObject cursorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(cursorPrefabPath);
            if (cursorPrefab != null)
            {
                GameObject cursorInstance = (GameObject)PrefabUtility.InstantiatePrefab(cursorPrefab);
                cursorInstance.name = "VFX_TacticalCaliperCursor";
                var impactCursor = cursorInstance.GetComponent<DexHigh.UI.CircularImpactCursor>();
                if (impactCursor != null)
                {
                    SetPrivateField(impactCursor, "_playerController", playerInstance.GetComponent<PlayerDragonController>());
                }
            }

            // 6. Combat HUD & Event System
            DexHigh.EditorTools.BuildCombatHUD.BuildHUD();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, scenePath);
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(target, value);
            }
        }
    }
}
