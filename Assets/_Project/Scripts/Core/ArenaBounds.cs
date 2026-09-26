using UnityEngine;

namespace DexHigh.Core
{
    public class ArenaBounds : MonoBehaviour
    {
        [Header("Arena Dimensions")]
        [SerializeField] private Vector2 _arenaSize = new Vector2(28f, 28f);
        [SerializeField] private float _wallHeight = 6f;
        [SerializeField] private bool _generateColliders = true;

        private void Awake()
        {
            if (_generateColliders)
            {
                CreateBoundaryColliders();
            }
        }

        private void CreateBoundaryColliders()
        {
            float halfWidth = _arenaSize.x * 0.5f;
            float halfLength = _arenaSize.y * 0.5f;
            float halfHeight = _wallHeight * 0.5f;

            // North Wall
            CreateWall(new Vector3(0f, halfHeight, halfLength), new Vector3(_arenaSize.x, _wallHeight, 1f));
            // South Wall
            CreateWall(new Vector3(0f, halfHeight, -halfLength), new Vector3(_arenaSize.x, _wallHeight, 1f));
            // East Wall
            CreateWall(new Vector3(halfWidth, halfHeight, 0f), new Vector3(1f, _wallHeight, _arenaSize.y));
            // West Wall
            CreateWall(new Vector3(-halfWidth, halfHeight, 0f), new Vector3(1f, _wallHeight, _arenaSize.y));
        }

        private void CreateWall(Vector3 localPos, Vector3 size)
        {
            GameObject wall = new GameObject("ArenaWall");
            wall.transform.parent = transform;
            wall.transform.localPosition = localPos;
            var box = wall.AddComponent<BoxCollider>();
            box.size = size;
        }

        public Vector3 ClampToArena(Vector3 position, float padding = 1f)
        {
            float halfW = (_arenaSize.x * 0.5f) - padding;
            float halfL = (_arenaSize.y * 0.5f) - padding;

            position.x = Mathf.Clamp(position.x, transform.position.x - halfW, transform.position.x + halfW);
            position.z = Mathf.Clamp(position.z, transform.position.z - halfL, transform.position.z + halfL);
            return position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.35f);
            Vector3 center = transform.position + Vector3.up * (_wallHeight * 0.5f);
            Gizmos.DrawWireCube(center, new Vector3(_arenaSize.x, _wallHeight, _arenaSize.y));
        }
    }
}
