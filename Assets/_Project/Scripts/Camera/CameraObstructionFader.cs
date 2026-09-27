using System.Collections.Generic;
using UnityEngine;

namespace DexHigh.CombatCamera
{
    /// <summary>
    /// Detects colosseum pillars, walls, and architectural structures that block the camera's
    /// line of sight to the battle combatants and smoothly reduces their opacity to 10-15%
    /// so the player can see through them during battle.
    /// </summary>
    public class CameraObstructionFader : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Transform _aiTransform;

        [Header("Transparency Settings")]
        [Tooltip("Target opacity when an obstacle blocks the camera view (10-15% as required).")]
        [Range(0.05f, 0.20f)]
        [SerializeField] private float _targetOpacity = 0.12f;

        [Tooltip("Speed at which obstacles fade into and out of transparency.")]
        [SerializeField] private float _fadeSpeed = 8.0f;

        [Header("Detection Settings")]
        [Tooltip("Radius of the sphere cast from camera to targets to prevent edge clipping.")]
        [SerializeField] private float _sphereCastRadius = 0.65f;

        [Tooltip("Layer mask for obstacles.")]
        [SerializeField] private LayerMask _obstacleLayers = ~0;

        [Tooltip("When true, fading a column or arch will fade its entire arcade bay uniformly.")]
        [SerializeField] private bool _fadeEntireModule = true;

        private class FadedObstacle
        {
            public GameObject RootObject;
            public List<Renderer> Renderers = new List<Renderer>();
            public List<Material[]> OriginalSharedMaterials = new List<Material[]>();
            public List<Material[]> InstancedMaterials = new List<Material[]>();
            public float CurrentAlpha = 1.0f;
            public bool IsOccluding = false;
            public bool IsTransparentMode = false;
        }

        private readonly Dictionary<GameObject, FadedObstacle> _trackedObstacles = new Dictionary<GameObject, FadedObstacle>();
        private readonly List<GameObject> _toRemove = new List<GameObject>();
        private readonly HashSet<GameObject> _currentFrameOccluders = new HashSet<GameObject>();

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly int SurfaceId = Shader.PropertyToID("_Surface");
        private static readonly int BlendId = Shader.PropertyToID("_Blend");
        private static readonly int SrcBlendId = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlendId = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWriteId = Shader.PropertyToID("_ZWrite");

        private void Start()
        {
            FindTargetsIfNull();
        }

        public void FindTargetsIfNull()
        {
            if (_playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) _playerTransform = player.transform;
            }

            if (_aiTransform == null)
            {
                var ai = GameObject.FindWithTag("Enemy");
                if (ai != null) _aiTransform = ai.transform;
            }
        }

        public void SetTargets(Transform player, Transform ai)
        {
            _playerTransform = player;
            _aiTransform = ai;
        }

        private void LateUpdate()
        {
            if (_playerTransform == null && _aiTransform == null)
            {
                FindTargetsIfNull();
                if (_playerTransform == null && _aiTransform == null) return;
            }

            DetectOccludingObstacles();
            UpdateObstacleFading();
        }

        private void DetectOccludingObstacles()
        {
            _currentFrameOccluders.Clear();

            Vector3 camPos = transform.position;
            List<Vector3> samplePoints = GetTargetSamplePoints();

            foreach (var targetPoint in samplePoints)
            {
                Vector3 toTarget = targetPoint - camPos;
                float distance = toTarget.magnitude;
                if (distance <= 0.1f) continue;

                Vector3 direction = toTarget / distance;

                // 1. Perform SphereCast to detect obstacles with thickness
                RaycastHit[] hits = Physics.SphereCastAll(camPos, _sphereCastRadius, direction, distance, _obstacleLayers, QueryTriggerInteraction.Ignore);
                ProcessHits(hits);

                // 2. Supplement with precise Raycast
                RaycastHit[] rayHits = Physics.RaycastAll(camPos, direction, distance, _obstacleLayers, QueryTriggerInteraction.Ignore);
                ProcessHits(rayHits);
            }

            // Mark tracking state for all registered obstacles
            foreach (var kvp in _trackedObstacles)
            {
                kvp.Value.IsOccluding = _currentFrameOccluders.Contains(kvp.Key);
            }

            // Register newly occluding obstacles
            foreach (var occluder in _currentFrameOccluders)
            {
                if (!_trackedObstacles.ContainsKey(occluder))
                {
                    RegisterNewObstacle(occluder);
                }
            }
        }

        private void ProcessHits(RaycastHit[] hits)
        {
            if (hits == null || hits.Length == 0) return;

            foreach (var hit in hits)
            {
                var collider = hit.collider;
                if (collider == null) continue;

                var go = collider.gameObject;

                // Filter out player, AI, combatants, floor, and VFX
                if (go.CompareTag("Player") || go.CompareTag("Enemy") || go.CompareTag("MainCamera")) continue;
                if (go.name.Contains("Floor") || go.name.Contains("Ground") || go.name.Contains("VFX") || go.name.Contains("Beam")) continue;

                // Identify target module or obstacle root
                GameObject obstacleRoot = GetObstacleRoot(go);
                if (obstacleRoot != null)
                {
                    _currentFrameOccluders.Add(obstacleRoot);
                }
            }
        }

        private GameObject GetObstacleRoot(GameObject hitObject)
        {
            if (!_fadeEntireModule) return hitObject;

            // Check if object is part of an Arcade Bay (Column, Arch, Podium, Upper Wall, etc.)
            Transform current = hitObject.transform;
            while (current != null)
            {
                if (current.name.StartsWith("Arcade_Bay_") || current.name.StartsWith("Pillar_") || current.name.StartsWith("Spectator_Tiers"))
                {
                    return current.gameObject;
                }

                if (current.name == "Colosseum_Architecture" || current.name == "Environment")
                {
                    // Don't go higher than the bay/module level
                    break;
                }

                current = current.parent;
            }

            return hitObject;
        }

        private List<Vector3> GetTargetSamplePoints()
        {
            var points = new List<Vector3>();

            if (_playerTransform != null)
            {
                Vector3 pPos = _playerTransform.position;
                points.Add(pPos + Vector3.up * 1.5f); // Center
                points.Add(pPos + Vector3.up * 2.5f); // Head / upper
                points.Add(pPos + Vector3.up * 0.5f); // Lower
                Vector3 rightOffset = _playerTransform.right * 1.2f;
                points.Add(pPos + Vector3.up * 1.5f + rightOffset);
                points.Add(pPos + Vector3.up * 1.5f - rightOffset);
            }

            if (_aiTransform != null)
            {
                Vector3 aiPos = _aiTransform.position;
                points.Add(aiPos + Vector3.up * 1.5f); // Center
                points.Add(aiPos + Vector3.up * 2.5f); // Head / upper
                points.Add(aiPos + Vector3.up * 0.5f); // Lower
                Vector3 rightOffset = _aiTransform.right * 1.2f;
                points.Add(aiPos + Vector3.up * 1.5f + rightOffset);
                points.Add(aiPos + Vector3.up * 1.5f - rightOffset);
            }

            if (_playerTransform != null && _aiTransform != null)
            {
                Vector3 mid = (_playerTransform.position + _aiTransform.position) * 0.5f;
                points.Add(mid + Vector3.up * 1.5f);
                points.Add(mid + Vector3.up * 2.5f);
            }

            return points;
        }

        private void RegisterNewObstacle(GameObject obstacleRoot)
        {
            var renderers = obstacleRoot.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;

            var obstacle = new FadedObstacle
            {
                RootObject = obstacleRoot,
                CurrentAlpha = 1.0f,
                IsOccluding = true,
                IsTransparentMode = false
            };

            foreach (var ren in renderers)
            {
                if (ren == null) continue;
                obstacle.Renderers.Add(ren);
                obstacle.OriginalSharedMaterials.Add(ren.sharedMaterials);

                // Clone materials for transparent instance manipulation
                Material[] instanced = new Material[ren.sharedMaterials.Length];
                for (int m = 0; m < ren.sharedMaterials.Length; m++)
                {
                    var orig = ren.sharedMaterials[m];
                    if (orig != null)
                    {
                        var clone = new Material(orig);
                        clone.name = $"{orig.name}_TransInstance";
                        instanced[m] = clone;
                    }
                }
                obstacle.InstancedMaterials.Add(instanced);
            }

            _trackedObstacles[obstacleRoot] = obstacle;
        }

        private void UpdateObstacleFading()
        {
            _toRemove.Clear();
            float dt = Time.deltaTime;

            foreach (var kvp in _trackedObstacles)
            {
                var obstacle = kvp.Value;
                if (obstacle.RootObject == null)
                {
                    _toRemove.Add(kvp.Key);
                    continue;
                }

                float targetAlpha = obstacle.IsOccluding ? _targetOpacity : 1.0f;
                obstacle.CurrentAlpha = Mathf.MoveTowards(obstacle.CurrentAlpha, targetAlpha, _fadeSpeed * dt);

                if (obstacle.CurrentAlpha < 0.999f)
                {
                    // Transition to transparent mode
                    if (!obstacle.IsTransparentMode)
                    {
                        for (int i = 0; i < obstacle.Renderers.Count; i++)
                        {
                            var ren = obstacle.Renderers[i];
                            if (ren != null)
                            {
                                var mats = obstacle.InstancedMaterials[i];
                                for (int m = 0; m < mats.Length; m++)
                                {
                                    SetMaterialTransparentMode(mats[m]);
                                }
                                ren.materials = mats;
                            }
                        }
                        obstacle.IsTransparentMode = true;
                    }

                    // Update alpha on instanced materials
                    for (int i = 0; i < obstacle.InstancedMaterials.Count; i++)
                    {
                        var mats = obstacle.InstancedMaterials[i];
                        for (int m = 0; m < mats.Length; m++)
                        {
                            SetMaterialAlpha(mats[m], obstacle.CurrentAlpha);
                        }
                    }
                }
                else
                {
                    // Fully restored to opaque
                    if (obstacle.IsTransparentMode)
                    {
                        for (int i = 0; i < obstacle.Renderers.Count; i++)
                        {
                            var ren = obstacle.Renderers[i];
                            if (ren != null)
                            {
                                ren.sharedMaterials = obstacle.OriginalSharedMaterials[i];
                            }
                        }
                        obstacle.IsTransparentMode = false;
                    }

                    // If not occluding and restored to 1.0, clean up instances and remove
                    if (!obstacle.IsOccluding)
                    {
                        CleanupInstancedMaterials(obstacle);
                        _toRemove.Add(kvp.Key);
                    }
                }
            }

            foreach (var key in _toRemove)
            {
                _trackedObstacles.Remove(key);
            }
        }

        private static void SetMaterialTransparentMode(Material mat)
        {
            if (mat == null) return;

            mat.SetFloat(SurfaceId, 1.0f); // Transparent surface
            mat.SetFloat(BlendId, 0.0f); // Alpha blend
            mat.SetInt(SrcBlendId, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt(DstBlendId, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt(ZWriteId, 0); // No z-write for see-through
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        private static void SetMaterialAlpha(Material mat, float alpha)
        {
            if (mat == null) return;

            if (mat.HasProperty(BaseColorId))
            {
                Color c = mat.GetColor(BaseColorId);
                c.a = alpha;
                mat.SetColor(BaseColorId, c);
            }
            else if (mat.HasProperty(ColorId))
            {
                Color c = mat.GetColor(ColorId);
                c.a = alpha;
                mat.SetColor(ColorId, c);
            }
        }

        private static void CleanupInstancedMaterials(FadedObstacle obstacle)
        {
            for (int i = 0; i < obstacle.InstancedMaterials.Count; i++)
            {
                var mats = obstacle.InstancedMaterials[i];
                for (int m = 0; m < mats.Length; m++)
                {
                    if (mats[m] != null)
                    {
                        Destroy(mats[m]);
                    }
                }
            }
            obstacle.InstancedMaterials.Clear();
        }

        private void OnDisable()
        {
            RestoreAllObstacles();
        }

        private void OnDestroy()
        {
            RestoreAllObstacles();
        }

        private void RestoreAllObstacles()
        {
            foreach (var kvp in _trackedObstacles)
            {
                var obstacle = kvp.Value;
                for (int i = 0; i < obstacle.Renderers.Count; i++)
                {
                    var ren = obstacle.Renderers[i];
                    if (ren != null && i < obstacle.OriginalSharedMaterials.Count)
                    {
                        ren.sharedMaterials = obstacle.OriginalSharedMaterials[i];
                    }
                }
                CleanupInstancedMaterials(obstacle);
            }
            _trackedObstacles.Clear();
        }
    }
}
