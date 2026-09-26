using System.IO;
using DexHigh.Core;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DexHigh.EditorTools
{
    [InitializeOnLoad]
    public static class ColosseumLightingBuilder
    {
        private const string MatDir = "Assets/_Project/Materials";
        private const string SODir = "Assets/_Project/ScriptableObjects";
        private const string VFXDir = "Assets/_Project/VFX";
        private const string ScenePath = "Assets/_Project/Scenes/DragonArenaBattle.unity";

        static ColosseumLightingBuilder()
        {
            EditorApplication.delayCall += () =>
            {
                if (!SessionState.GetBool("DexHigh_Colosseum_Built", false))
                {
                    SessionState.SetBool("DexHigh_Colosseum_Built", true);
                    BuildAtmosphere();
                }
            };
        }

        [MenuItem("DexHigh/Build Colosseum Lighting & Atmosphere")]
        public static void BuildAtmosphere()
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.path != ScenePath)
            {
                EditorSceneManager.OpenScene(ScenePath);
            }

            Debug.Log("<color=#F39C12><b>[Colosseum]</b> Starting Colosseum Environment & Golden Lighting generation...</color>");

            // 1. Create PBR Materials for Colosseum
            var mats = CreateColosseumMaterials();

            // 2. Setup Procedural Skybox & RenderSettings Fog
            SetupSkyboxAndFog();

            // 3. Build Post-Processing Volume Profile
            SetupVolumeProfile();

            // 4. Build Directional Sun & Fill Lighting
            SetupDirectionalLights();

            // 5. Build Colosseum Architecture (Floor, Tiers, Arches, Pillars, Rubble)
            BuildColosseumGeometry(mats);

            // 6. Build Volumetric Sun Shafts (God Rays) & Drifting Dust Motes
            BuildVolumetricSunShaftsAndDust(mats);

            // 7. Update NavMesh on the arena floor
            RebuildArenaNavMesh();

            // 8. Set Camera to Top-Down Combat Framing
            var cam = Camera.main;
            if (cam != null)
            {
                cam.transform.position = new Vector3(0f, 16.5f, -16.5f);
                cam.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            }

            // 9. Save Scene
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=#2ECC71><b>[Colosseum]</b> Successfully built Colosseum Architecture, Golden Hour Lighting, God Rays, and Atmospheric Fog!</color>");

            CaptureScreenshots();
        }

        [MenuItem("DexHigh/Capture Colosseum Screenshots")]
        public static void CaptureScreenshots()
        {
            string dir = "Assets/Screenshots";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var cam = Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("[Colosseum] Main Camera not found to capture screenshots.");
                return;
            }

            Vector3 origPos = cam.transform.position;
            Quaternion origRot = cam.transform.rotation;
            float origFov = cam.fieldOfView;

            // 1. Gameplay Combat Angle (Top-Down 42 degrees)
            cam.transform.position = new Vector3(0f, 16f, -18f);
            cam.transform.rotation = Quaternion.Euler(42f, 0f, 0f);
            cam.fieldOfView = 60f;
            RenderAndSave(cam, $"{dir}/Colosseum_Gameplay_View.png");

            // 2. Cinematic Wide Angle matching user reference photo (Looking across arena towards arches & sunbeams)
            cam.transform.position = new Vector3(0f, 2.8f, -18f);
            cam.transform.rotation = Quaternion.Euler(15f, 0f, 0f);
            cam.fieldOfView = 70f;
            RenderAndSave(cam, $"{dir}/Colosseum_Cinematic_Reference_View.png");

            // Restore to default combat angle
            cam.transform.position = new Vector3(0f, 16.5f, -16.5f);
            cam.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            cam.fieldOfView = 60f;

            Debug.Log("<color=cyan><b>[Colosseum]</b> Screenshots captured successfully in Assets/Screenshots/</color>");
            AssetDatabase.Refresh();
        }

        private static void RenderAndSave(Camera cam, string filePath)
        {
            RenderTexture rt = new RenderTexture(1920, 1080, 24, RenderTextureFormat.DefaultHDR);
            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
            tex.Apply();

            cam.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);

            File.WriteAllBytes(filePath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }

        private struct ColosseumMats
        {
            public Material sandFloor;
            public Material stoneWall;
            public Material stoneTrim;
            public Material seatingStone;
            public Material rubbleRocks;
            public Material sunShaft;
            public Material dustMotes;
        }

        private static ColosseumMats CreateColosseumMaterials()
        {
            ColosseumMats mats = new ColosseumMats();
            Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLit == null) urpLit = Shader.Find("Standard");

            Shader urpParticles = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (urpParticles == null) urpParticles = Shader.Find("Mobile/Particles/Additive");

            // 1. Sand Arena Floor
            mats.sandFloor = GetOrCreateMaterial($"{MatDir}/M_Colosseum_SandFloor.mat", urpLit);
            mats.sandFloor.SetColor("_BaseColor", new Color(0.76f, 0.58f, 0.38f)); // Sandy ochre
            mats.sandFloor.SetFloat("_Smoothness", 0.12f);
            mats.sandFloor.SetFloat("_Metallic", 0.0f);

            // Also update legacy floor material if still referenced anywhere
            var legacyFloor = AssetDatabase.LoadAssetAtPath<Material>($"{MatDir}/M_ArenaFloor.mat");
            if (legacyFloor != null)
            {
                legacyFloor.SetColor("_BaseColor", new Color(0.76f, 0.58f, 0.38f));
                legacyFloor.SetFloat("_Smoothness", 0.12f);
            }

            // 2. Stone Colosseum Wall
            mats.stoneWall = GetOrCreateMaterial($"{MatDir}/M_Colosseum_StoneWall.mat", urpLit);
            mats.stoneWall.SetColor("_BaseColor", new Color(0.62f, 0.48f, 0.34f)); // Weathered Roman travertine
            mats.stoneWall.SetFloat("_Smoothness", 0.22f);
            mats.stoneWall.SetFloat("_Metallic", 0.0f);

            var legacyPillar = AssetDatabase.LoadAssetAtPath<Material>($"{MatDir}/M_ArenaPillar.mat");
            if (legacyPillar != null)
            {
                legacyPillar.SetColor("_BaseColor", new Color(0.62f, 0.48f, 0.34f));
                legacyPillar.SetFloat("_Smoothness", 0.22f);
            }

            // 3. Stone Trim & Cornice
            mats.stoneTrim = GetOrCreateMaterial($"{MatDir}/M_Colosseum_StoneTrim.mat", urpLit);
            mats.stoneTrim.SetColor("_BaseColor", new Color(0.50f, 0.38f, 0.27f)); // Darker stone cornice
            mats.stoneTrim.SetFloat("_Smoothness", 0.28f);

            var legacyTrim = AssetDatabase.LoadAssetAtPath<Material>($"{MatDir}/M_ArenaTrim.mat");
            if (legacyTrim != null)
            {
                legacyTrim.SetColor("_BaseColor", new Color(0.50f, 0.38f, 0.27f));
                legacyTrim.SetFloat("_Smoothness", 0.28f);
            }

            // 4. Seating Bleachers
            mats.seatingStone = GetOrCreateMaterial($"{MatDir}/M_Colosseum_Seating.mat", urpLit);
            mats.seatingStone.SetColor("_BaseColor", new Color(0.58f, 0.45f, 0.32f));
            mats.seatingStone.SetFloat("_Smoothness", 0.18f);

            // 5. Rubble / Broken Rocks
            mats.rubbleRocks = GetOrCreateMaterial($"{MatDir}/M_Colosseum_Rubble.mat", urpLit);
            mats.rubbleRocks.SetColor("_BaseColor", new Color(0.42f, 0.32f, 0.22f));
            mats.rubbleRocks.SetFloat("_Smoothness", 0.15f);

            // 6. Volumetric Sun Shaft Material (Soft Additive)
            mats.sunShaft = GetOrCreateMaterial($"{MatDir}/M_Colosseum_SunShaft.mat", urpParticles);
            mats.sunShaft.SetFloat("_Surface", 1.0f); // Transparent
            mats.sunShaft.SetFloat("_Blend", 1.0f); // Additive
            mats.sunShaft.SetColor("_BaseColor", new Color(1.0f, 0.78f, 0.45f, 0.18f));
            if (mats.sunShaft.HasProperty("_Color")) mats.sunShaft.SetColor("_Color", new Color(1.0f, 0.78f, 0.45f, 0.18f));

            // Load soft smoke/shockwave texture for smooth light shaft falloff
            Texture2D softPuff = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Art/VFX/T_Particle_SmokePuff.png");
            if (softPuff != null)
            {
                mats.sunShaft.SetTexture("_BaseMap", softPuff);
                if (mats.sunShaft.HasProperty("_MainTex")) mats.sunShaft.SetTexture("_MainTex", softPuff);
            }

            // 7. Dust Motes Material
            mats.dustMotes = GetOrCreateMaterial($"{MatDir}/M_Colosseum_DustMotes.mat", urpParticles);
            mats.dustMotes.SetFloat("_Surface", 1.0f);
            mats.dustMotes.SetFloat("_Blend", 1.0f);
            mats.dustMotes.SetColor("_BaseColor", new Color(1.0f, 0.85f, 0.55f, 0.65f));
            Texture2D sparkTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Art/VFX/T_Particle_EmberSpark.png");
            if (sparkTex != null)
            {
                mats.dustMotes.SetTexture("_BaseMap", sparkTex);
                if (mats.dustMotes.HasProperty("_MainTex")) mats.dustMotes.SetTexture("_MainTex", sparkTex);
            }

            return mats;
        }

        private static Material GetOrCreateMaterial(string path, Shader shader)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }
            return mat;
        }

        private static void SetupSkyboxAndFog()
        {
            // Create Procedural Skybox Material with warm golden atmospheric scattering
            string skyMatPath = $"{MatDir}/M_Colosseum_Skybox.mat";
            Shader skyShader = Shader.Find("Skybox/Procedural");
            if (skyShader != null)
            {
                Material skyMat = GetOrCreateMaterial(skyMatPath, skyShader);
                skyMat.SetFloat("_SunSize", 0.045f);
                skyMat.SetFloat("_SunSizeConvergence", 8.0f);
                skyMat.SetFloat("_AtmosphereThickness", 1.35f); // Rich warm atmosphere
                skyMat.SetColor("_SkyTint", new Color(0.85f, 0.62f, 0.38f)); // Golden dusty sky
                skyMat.SetColor("_GroundColor", new Color(0.36f, 0.22f, 0.12f)); // Warm ochre earth ground
                skyMat.SetFloat("_Exposure", 1.35f);

                RenderSettings.skybox = skyMat;
            }

            // Ambient Trilight matching golden hour
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.84f, 0.65f, 0.44f); // Golden amber sky
            RenderSettings.ambientEquatorColor = new Color(0.66f, 0.45f, 0.28f); // Warm dusty ochre
            RenderSettings.ambientGroundColor = new Color(0.32f, 0.19f, 0.10f); // Deep warm umber bounce

            // Atmospheric Fog: Soft golden distance haze that blends distant arches with the horizon
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = 22f;
            RenderSettings.fogEndDistance = 78f;
            RenderSettings.fogColor = new Color(0.80f, 0.58f, 0.36f); // Warm dusty atmospheric haze
        }

        private static void SetupVolumeProfile()
        {
            string profilePath = $"{SODir}/Arena_VolumeProfile.asset";
            VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, profilePath);
            }

            // Clear old sub-assets if any
            var existingSubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(profilePath);
            foreach (var sub in existingSubAssets)
            {
                if (sub != null) Object.DestroyImmediate(sub, true);
            }
            profile.components.Clear();

            // 1. ACES Tonemapping for filmic dynamic range
            var tonemapping = profile.Add<Tonemapping>(true);
            tonemapping.mode.Override(TonemappingMode.ACES);
            AssetDatabase.AddObjectToAsset(tonemapping, profile);

            // 2. White Balance (Warm Golden Hour Temperature Shift)
            var whiteBalance = profile.Add<WhiteBalance>(true);
            whiteBalance.temperature.Override(24.0f);
            whiteBalance.tint.Override(4.0f);
            AssetDatabase.AddObjectToAsset(whiteBalance, profile);

            // 3. Color Adjustments (Cinematic Warm Contrast & Exposure)
            var colorAdj = profile.Add<ColorAdjustments>(true);
            colorAdj.postExposure.Override(0.25f);
            colorAdj.contrast.Override(18.0f);
            colorAdj.colorFilter.Override(new Color(1.0f, 0.92f, 0.82f));
            colorAdj.saturation.Override(14.0f);
            AssetDatabase.AddObjectToAsset(colorAdj, profile);

            // 4. Shadows Midtones Highlights (Warm Golden Split Toning)
            var smh = profile.Add<ShadowsMidtonesHighlights>(true);
            smh.shadows.Override(new Vector4(0.24f, 0.16f, 0.10f, 0.0f)); // Warm sepia / deep umber shadows
            smh.midtones.Override(new Vector4(0.68f, 0.48f, 0.30f, 0.0f)); // Earthy bronze / sandstone midtones
            smh.highlights.Override(new Vector4(1.0f, 0.86f, 0.65f, 0.0f)); // Radiant sunlight gold highlights
            AssetDatabase.AddObjectToAsset(smh, profile);

            // 5. Bloom (Atmospheric Sun Glare & Radiant Glow)
            var bloom = profile.Add<Bloom>(true);
            bloom.intensity.Override(0.95f);
            bloom.threshold.Override(0.85f);
            bloom.scatter.Override(0.72f);
            bloom.tint.Override(new Color(1.0f, 0.88f, 0.72f));
            AssetDatabase.AddObjectToAsset(bloom, profile);

            // 6. Vignette (Deep Warm Edge Falloff)
            var vignette = profile.Add<Vignette>(true);
            vignette.intensity.Override(0.32f);
            vignette.smoothness.Override(0.50f);
            vignette.color.Override(new Color(0.12f, 0.07f, 0.04f));
            AssetDatabase.AddObjectToAsset(vignette, profile);

            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();

            // Connect to Global Volume in scene
            var volGO = GameObject.Find("Global Volume");
            if (volGO == null)
            {
                var lightingRoot = GameObject.Find("Lighting & PostProcessing");
                volGO = new GameObject("Global Volume");
                if (lightingRoot != null) volGO.transform.parent = lightingRoot.transform;
            }
            var vol = volGO.GetComponent<Volume>();
            if (vol == null) vol = volGO.AddComponent<Volume>();
            vol.isGlobal = true;
            vol.sharedProfile = profile;
        }

        private static void SetupDirectionalLights()
        {
            var lightingRoot = GameObject.Find("Lighting & PostProcessing");
            if (lightingRoot == null)
            {
                lightingRoot = new GameObject("Lighting & PostProcessing");
            }

            // 1. Sun Directional Light (Golden-Hour Key Light)
            var sunGO = GameObject.Find("Directional Light (Sun)");
            if (sunGO == null)
            {
                sunGO = new GameObject("Directional Light (Sun)");
                sunGO.transform.parent = lightingRoot.transform;
            }

            // Angle matching reference: Sun beams entering from upper-left arches diagonally across arena
            sunGO.transform.rotation = Quaternion.Euler(46f, 38f, 0f);
            var sunLight = sunGO.GetComponent<Light>();
            if (sunLight == null) sunLight = sunGO.AddComponent<Light>();
            sunLight.type = LightType.Directional;
            sunLight.intensity = 2.85f;
            sunLight.color = new Color(1.0f, 0.76f, 0.44f); // Warm golden amber
            sunLight.shadows = LightShadows.Soft;
            sunLight.shadowResolution = LightShadowResolution.VeryHigh;
            sunLight.shadowNormalBias = 0.4f;
            sunLight.shadowBias = 0.05f;

            // 2. Directional Fill Light (Warm Sand Ground Bounce)
            var fillGO = GameObject.Find("Directional Light (Fill)");
            if (fillGO == null)
            {
                fillGO = new GameObject("Directional Light (Fill)");
                fillGO.transform.parent = lightingRoot.transform;
            }

            fillGO.transform.rotation = Quaternion.Euler(60f, -145f, 0f);
            var fillLight = fillGO.GetComponent<Light>();
            if (fillLight == null) fillLight = fillGO.AddComponent<Light>();
            fillLight.type = LightType.Directional;
            fillLight.intensity = 0.55f;
            fillLight.color = new Color(0.72f, 0.46f, 0.28f); // Warm earthen terracotta bounce
            fillLight.shadows = LightShadows.None;
        }

        private static void BuildColosseumGeometry(ColosseumMats mats)
        {
            var envRoot = GameObject.Find("Environment");
            if (envRoot == null)
            {
                envRoot = new GameObject("Environment");
            }

            // Remove old Pillars container if present to replace with authentic Colosseum architecture
            var oldPillars = envRoot.transform.Find("Pillars");
            if (oldPillars != null)
            {
                Object.DestroyImmediate(oldPillars.gameObject);
            }

            // 1. Arena Sand Floor (Radius 21m, Diameter 42m)
            float arenaRadius = 21f;
            var floor = envRoot.transform.Find("Arena_Floor");
            if (floor != null)
            {
                floor.localPosition = new Vector3(0f, -0.25f, 0f);
                floor.localScale = new Vector3(arenaRadius * 2f, 0.3f, arenaRadius * 2f);
                var ren = floor.GetComponent<Renderer>();
                if (ren != null) ren.sharedMaterial = mats.sandFloor;
            }

            // 2. Arena Stone Trim
            var trim = envRoot.transform.Find("Arena_Trim");
            if (trim != null)
            {
                trim.localPosition = new Vector3(0f, -0.28f, 0f);
                trim.localScale = new Vector3(arenaRadius * 2f + 2.5f, 0.25f, arenaRadius * 2f + 2.5f);
                var ren = trim.GetComponent<Renderer>();
                if (ren != null) ren.sharedMaterial = mats.stoneTrim;
            }

            // 3. Colosseum Architecture Root
            var colosseumGO = envRoot.transform.Find("Colosseum_Architecture");
            if (colosseumGO != null)
            {
                Object.DestroyImmediate(colosseumGO.gameObject);
            }

            GameObject archRoot = new GameObject("Colosseum_Architecture");
            archRoot.transform.parent = envRoot.transform;
            archRoot.transform.localPosition = Vector3.zero;

            // A. Inner Stone Ring (Radius ~8.5m) — scattered low stones / flagstones
            GameObject innerRing = new GameObject("Inner_Stone_Ring");
            innerRing.transform.parent = archRoot.transform;
            int innerStoneCount = 28;
            float innerRadius = 8.5f;
            for (int i = 0; i < innerStoneCount; i++)
            {
                float angle = (i * Mathf.PI * 2f) / innerStoneCount;
                float r = innerRadius + Mathf.Sin(i * 3.7f) * 0.4f;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * r, 0.08f, Mathf.Sin(angle) * r);

                GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stone.name = $"InnerStone_{i}";
                stone.transform.parent = innerRing.transform;
                stone.transform.localPosition = pos;
                stone.transform.localRotation = Quaternion.Euler(Random.Range(-5f, 5f), angle * Mathf.Rad2Deg + 90f, Random.Range(-5f, 5f));
                stone.transform.localScale = new Vector3(1.1f, 0.22f, 0.65f);
                stone.GetComponent<Renderer>().sharedMaterial = mats.rubbleRocks;
                // Remove collider so dragons walk smoothly
                var col = stone.GetComponent<Collider>();
                if (col != null) Object.DestroyImmediate(col);
            }

            // B. Perimeter Rubble Rocks (Radius ~20.5m) — weathered boulders lining the pit boundary
            GameObject rubbleRing = new GameObject("Boundary_Rubble_Rocks");
            rubbleRing.transform.parent = archRoot.transform;
            int rubbleCount = 40;
            float rubbleRadius = 20.5f;
            for (int i = 0; i < rubbleCount; i++)
            {
                float angle = (i * Mathf.PI * 2f) / rubbleCount;
                float r = rubbleRadius + Mathf.Sin(i * 2.3f) * 0.6f;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * r, 0.25f, Mathf.Sin(angle) * r);

                GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rock.name = $"Rubble_{i}";
                rock.transform.parent = rubbleRing.transform;
                rock.transform.localPosition = pos;
                rock.transform.localRotation = Quaternion.Euler(Random.Range(-12f, 15f), angle * Mathf.Rad2Deg + Random.Range(-20f, 20f), Random.Range(-10f, 12f));
                float s = Random.Range(1.2f, 2.0f);
                rock.transform.localScale = new Vector3(s * 1.3f, s * 0.65f, s * 0.9f);
                rock.GetComponent<Renderer>().sharedMaterial = mats.rubbleRocks;
                var col = rock.GetComponent<Collider>();
                if (col != null) Object.DestroyImmediate(col);
            }

            // C. Stepped Spectator Bleachers (Amphitheater Tiers)
            // 5 concentric tiers rising from radius 22m to 28m, height 0.5m to 4.5m
            GameObject seatingRoot = new GameObject("Spectator_Tiers");
            seatingRoot.transform.parent = archRoot.transform;
            int tierCount = 5;
            float tierStartRadius = 22.0f;
            float tierStepWidth = 1.25f;
            float tierStepHeight = 0.85f;

            for (int t = 0; t < tierCount; t++)
            {
                float currentRadius = tierStartRadius + t * tierStepWidth;
                float currentHeight = 0.4f + t * tierStepHeight;
                int segments = 48;

                GameObject tierGO = new GameObject($"Tier_{t + 1}");
                tierGO.transform.parent = seatingRoot.transform;

                for (int s = 0; s < segments; s++)
                {
                    float angle = (s * Mathf.PI * 2f) / segments;
                    Vector3 pos = new Vector3(Mathf.Cos(angle) * currentRadius, currentHeight, Mathf.Sin(angle) * currentRadius);

                    GameObject step = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    step.name = $"Step_{s}";
                    step.transform.parent = tierGO.transform;
                    step.transform.localPosition = pos;
                    step.transform.localRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg + 90f, 0f);
                    float arcWidth = (2f * Mathf.PI * currentRadius / segments) * 1.05f;
                    step.transform.localScale = new Vector3(arcWidth, tierStepHeight, tierStepWidth * 1.1f);
                    step.GetComponent<Renderer>().sharedMaterial = mats.seatingStone;
                }
            }

            // D. Grand Colosseum Arcade Wall (Arches & Pillars)
            // Multi-tiered massive outer wall at radius ~29.5m rising to 18m height
            GameObject arcadeWall = new GameObject("Colosseum_Arcade_Wall");
            arcadeWall.transform.parent = archRoot.transform;

            int archCount = 28;
            float wallRadius = 29.5f;
            float wallBaseHeight = 4.8f;
            float archHeight = 8.5f;
            float upperWallHeight = 5.0f;

            for (int i = 0; i < archCount; i++)
            {
                float angle = (i * Mathf.PI * 2f) / archCount;
                Vector3 wallCenter = new Vector3(Mathf.Cos(angle) * wallRadius, 0f, Mathf.Sin(angle) * wallRadius);
                Quaternion rotFacingCenter = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg - 90f, 0f);

                GameObject bayGO = new GameObject($"Arcade_Bay_{i + 1}");
                bayGO.transform.parent = arcadeWall.transform;
                bayGO.transform.localPosition = wallCenter;
                bayGO.transform.localRotation = rotFacingCenter;

                // 1. Lower Podium Wall
                GameObject podium = GameObject.CreatePrimitive(PrimitiveType.Cube);
                podium.name = "Podium_Base";
                podium.transform.parent = bayGO.transform;
                podium.transform.localPosition = new Vector3(0f, wallBaseHeight * 0.5f, 0f);
                float bayWidth = (2f * Mathf.PI * wallRadius / archCount) * 1.02f;
                podium.transform.localScale = new Vector3(bayWidth, wallBaseHeight, 2.2f);
                podium.GetComponent<Renderer>().sharedMaterial = mats.stoneWall;

                // 2. Colosseum Arch Columns (Left & Right)
                float columnX = bayWidth * 0.36f;
                float columnRadius = 0.65f;

                for (int colSide = -1; colSide <= 1; colSide += 2)
                {
                    GameObject col = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    col.name = colSide < 0 ? "Column_Left" : "Column_Right";
                    col.transform.parent = bayGO.transform;
                    col.transform.localPosition = new Vector3(colSide * columnX, wallBaseHeight + archHeight * 0.5f, 0.4f);
                    col.transform.localScale = new Vector3(columnRadius * 2f, archHeight * 0.5f, columnRadius * 2f);
                    col.GetComponent<Renderer>().sharedMaterial = mats.stoneWall;
                }

                // 3. Arch Lintel / Keystones (Spans across the columns)
                GameObject archLintel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                archLintel.name = "Arch_Keystone_Lintel";
                archLintel.transform.parent = bayGO.transform;
                archLintel.transform.localPosition = new Vector3(0f, wallBaseHeight + archHeight + 0.5f, 0f);
                archLintel.transform.localScale = new Vector3(bayWidth, 1.4f, 2.4f);
                archLintel.GetComponent<Renderer>().sharedMaterial = mats.stoneTrim;

                // 4. Upper Colosseum Attic Wall
                GameObject upperAttic = GameObject.CreatePrimitive(PrimitiveType.Cube);
                upperAttic.name = "Upper_Attic_Wall";
                upperAttic.transform.parent = bayGO.transform;
                upperAttic.transform.localPosition = new Vector3(0f, wallBaseHeight + archHeight + 1.2f + upperWallHeight * 0.5f, 0f);
                upperAttic.transform.localScale = new Vector3(bayWidth, upperWallHeight, 2.0f);
                upperAttic.GetComponent<Renderer>().sharedMaterial = mats.stoneWall;

                // 5. Crown Battlement / Pinnacle on Rim (weathered jagged top)
                if (i % 2 == 0)
                {
                    GameObject crown = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    crown.name = "Crown_Pinnacle";
                    crown.transform.parent = bayGO.transform;
                    crown.transform.localPosition = new Vector3(0f, wallBaseHeight + archHeight + 1.2f + upperWallHeight + 0.9f, 0.1f);
                    crown.transform.localScale = new Vector3(bayWidth * 0.45f, 1.8f, 1.6f);
                    crown.GetComponent<Renderer>().sharedMaterial = mats.stoneTrim;
                }
            }
        }

        private static void BuildVolumetricSunShaftsAndDust(ColosseumMats mats)
        {
            var lightingRoot = GameObject.Find("Lighting & PostProcessing");
            if (lightingRoot == null) lightingRoot = new GameObject("Lighting & PostProcessing");

            // Remove existing atmosphere effects if present
            var oldShafts = lightingRoot.transform.Find("Atmospheric_SunShafts");
            if (oldShafts != null) Object.DestroyImmediate(oldShafts.gameObject);

            var oldDust = lightingRoot.transform.Find("Atmospheric_DustMotes");
            if (oldDust != null) Object.DestroyImmediate(oldDust.gameObject);

            // 1. Dedicated Volumetric Sun Shafts (God Rays)
            // 3 angled translucent sunbeams piercing from the upper arches into the arena center
            GameObject shaftsRoot = new GameObject("Atmospheric_SunShafts");
            shaftsRoot.transform.parent = lightingRoot.transform;

            Vector3[] shaftOrigins = new Vector3[]
            {
                new Vector3(-18f, 17f, 16f), // Main dramatic sun shaft
                new Vector3(-12f, 18f, 21f), // Secondary sun shaft
                new Vector3(-22f, 16f, 10f)  // Third flank sun shaft
            };

            Vector3[] shaftTargets = new Vector3[]
            {
                new Vector3(2f, 0f, -1f),
                new Vector3(8f, 0f, 4f),
                new Vector3(-5f, 0f, -6f)
            };

            float[] shaftWidths = new float[] { 14f, 10f, 9f };

            for (int k = 0; k < shaftOrigins.Length; k++)
            {
                Vector3 origin = shaftOrigins[k];
                Vector3 target = shaftTargets[k];
                Vector3 dir = (target - origin).normalized;
                float dist = Vector3.Distance(origin, target);

                GameObject shaftGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                shaftGO.name = $"SunShaft_Beam_{k + 1}";
                shaftGO.transform.parent = shaftsRoot.transform;
                shaftGO.transform.position = origin + dir * (dist * 0.5f);
                shaftGO.transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90f, 0f, 0f);
                shaftGO.transform.localScale = new Vector3(shaftWidths[k], dist * 0.5f, shaftWidths[k]);

                // Destroy collider
                var col = shaftGO.GetComponent<Collider>();
                if (col != null) Object.DestroyImmediate(col);

                var ren = shaftGO.GetComponent<Renderer>();
                if (ren != null)
                {
                    ren.sharedMaterial = mats.sunShaft;
                    ren.shadowCastingMode = ShadowCastingMode.Off;
                    ren.receiveShadows = false;
                }
            }

            // 2. Suspended Golden Dust Motes (Particle System)
            GameObject dustGO = new GameObject("Atmospheric_DustMotes");
            dustGO.transform.parent = lightingRoot.transform;
            dustGO.transform.position = new Vector3(0f, 2.5f, 0f);

            var ps = dustGO.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 10f;
            main.loop = true;
            main.startLifetime = new ParticleSystem.MinMaxCurve(6f, 10f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.15f, 0.45f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.30f);
            main.startColor = new Color(1.0f, 0.88f, 0.60f, 0.45f); // Glowing golden amber
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 300;

            var emission = ps.emission;
            emission.rateOverTime = 35f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(36f, 8f, 36f); // Fills the arena volume

            var velOverLifetime = ps.velocityOverLifetime;
            velOverLifetime.enabled = true;
            velOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.25f, 0.25f);
            velOverLifetime.y = new ParticleSystem.MinMaxCurve(0.05f, 0.20f); // Gentle upward thermal drift
            velOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.25f, 0.25f);

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.9f, 0.6f), 0f), new GradientColorKey(new Color(1f, 0.8f, 0.5f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.6f, 0.3f), new GradientAlphaKey(0.6f, 0.7f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            var psRen = dustGO.GetComponent<ParticleSystemRenderer>();
            psRen.sharedMaterial = mats.dustMotes;
        }

        private static void RebuildArenaNavMesh()
        {
            var floor = GameObject.Find("Arena_Floor");
            if (floor != null)
            {
                var navSurface = floor.GetComponent<NavMeshSurface>();
                if (navSurface != null)
                {
                    navSurface.BuildNavMesh();
                }
            }
        }
    }
}
