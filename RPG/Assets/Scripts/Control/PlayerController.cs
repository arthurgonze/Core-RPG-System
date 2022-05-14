using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
    
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private float _maxNavMeshSampleDistance = 1f;
        [SerializeField] private float _maxNavPathDistance = 20f;
        [SerializeField] CursorMapping[] _cursorMappings = null;


        // Cached Reference
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;
        

        // Start is called before the first frame update
        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (InteractWithUI()) return;
            if (_health.IsDead()) 
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                    if(raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
            
            float[] distances = new float[hits.Length];
            for(int i=0; i<hits.Length;i++)
                distances[i] = hits[i].distance;
            
            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithUI()
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // RaycastHit hit;
            // bool hasHit = Physics.Raycast(ray, out hit);
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
                if (Input.GetMouseButton(0))
                    _mover.StartMovementAction(target, 1f);
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit);
            if(!hasHit) return false;
            
            NavMeshHit navMeshHit;
            hasHit = NavMesh.SamplePosition(hit.point, out navMeshHit, _maxNavMeshSampleDistance, NavMesh.AllAreas);
            if(!hasHit) return false;

            target = navMeshHit.position;
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            
            if((!hasPath) || (path.status != NavMeshPathStatus.PathComplete) || (GetPathLength(path) > _maxNavPathDistance)) return false;

            return hasHit;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float totalDistance = 0;
            if(path.corners.Length < 2) return totalDistance;
            for (int i=1; i < path.corners.Length; i++)
                totalDistance += Vector3.Distance(path.corners[i-1], path.corners[i]);
            
            return totalDistance;
        }

        public void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            CursorMapping cursor_none = _cursorMappings[0];
            foreach (CursorMapping cursor in _cursorMappings)
            {
                if(cursor.type == type)
                    return cursor;
                if(cursor.type == CursorType.None)
                    cursor_none = cursor;
            }

            // if dont find anything return the cursor none type or the first cursor defined
            return cursor_none;
        }

    }
}
