using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class XMarkControl : MonoBehaviour {
    public GameObject XMark;
    //A list that holds the matched objects.
    public List<Transform> destroyObjects = new();
    //A struct that holds all X mark, the column of the X mark, and the row of the X mark.
    [Serializable]
    public struct allMarks {
        public Dictionary<Transform, Vector2Int> marks;

        public void Initialize() {
            marks = new();
        }

        public void Add(Transform xMarkTransform, int markColumn, int markRow) {
            marks[xMarkTransform] = new Vector2Int(markColumn, markRow);
        }

        public Vector2Int? GetPosition(Transform xMarkTransform) {
            if (marks.TryGetValue(xMarkTransform, out Vector2Int pos)) {
                return pos;
            }
            return null;
        }

        public void Remove(Transform xMarkTransform) {
            marks.Remove(xMarkTransform);
        }
    }

    public allMarks SallMarks;
    void Start() {
        SallMarks.Initialize();
    }
    void Update() {
        ClickFunc();
        SetMatches();
        DestroyMatches();
    }

    //The function that places the X mark on top of the tiles. 
    public void ClickFunc() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null && hit.transform.childCount < 1) {
                GameObject mark = Instantiate(XMark, Vector2.zero, Quaternion.identity, hit.transform);
                mark.transform.localPosition = Vector2.zero;
                int column = Mathf.RoundToInt(hit.transform.position.x);
                int row = Mathf.RoundToInt(hit.transform.position.y);
                if (Board.I.XMarkRowColumn(column, row) == null) {
                    Board.I.AllXmarksPos[column, row] = mark;
                    SallMarks.Add(mark.transform, column, row);
                }
                
            }
        }
    }

    //The function that checks for matched X marks and adds them to the list.
    public void SetMatches() {
        //Stores the previously visited "X Mark" objects.
        HashSet<Transform> checkedMarks = new();
        //Conversion of the Keys list into a list.
        List<Transform> allMarks = SallMarks.marks.Keys.ToList();

        for (int i = 0; i < allMarks.Count; i++) {
            Transform xMark = allMarks[i];
            if (checkedMarks.Contains(xMark)) continue;
            //Stores the "X Mark" objects that are connected to the starting mark.
            List<Transform> matchedMark = new();
            ScanNeighbors(xMark, matchedMark, checkedMarks);

            if (matchedMark.Count >= 3) {
                for (int j = 0; j < matchedMark.Count; j++) {
                    if (!destroyObjects.Contains(matchedMark[j])) {
                        destroyObjects.Add(matchedMark[j]);
                    }
                }
            }
        }
    }

    //The function that implements a Breadth-First Search (BFS) to find and collect all the connected "X Marks"
    void ScanNeighbors(Transform markTransform, List<Transform> matchedMark, HashSet<Transform> checkedMarks) {
        Queue<Transform> queue = new();
        queue.Enqueue(markTransform);

        while (queue.Count > 0) {
            Transform current = queue.Dequeue();
            if (checkedMarks.Contains(current)) continue;

            checkedMarks.Add(current);
            matchedMark.Add(current);

            Vector2Int? pos = SallMarks.GetPosition(current);
            if (pos == null) continue;
            int x = pos.Value.x, y = pos.Value.y;
            //Control all directions such as up,down,left,right
            Vector2Int[] allDirections = { 
                Vector2Int.right, 
                Vector2Int.left,
                Vector2Int.up, 
                Vector2Int.down,
            };

            for (int i = 0; i < allDirections.Length; i++) {
                int newX = x + allDirections[i].x;
                int newY = y + allDirections[i].y;

                if (newX < 0 || newX >= Board.I.sideLength || newY < 0 || newY >= Board.I.sideLength) continue;

                Transform neighbor = Board.I.XMarkRowColumn(newX, newY);
                if (neighbor != null && !checkedMarks.Contains(neighbor)) {
                    queue.Enqueue(neighbor);
                }
            }
        }
    }
    
    //The function that performs the destroy operation and removes the item from the dictionary in the struct.
    public void DestroyMatches() {
        for (int i = destroyObjects.Count - 1; i >= 0; i--) {
            Transform obj = destroyObjects[i];
            Vector2Int? pos = SallMarks.GetPosition(obj);
            if (pos == null) continue;

            Board.I.AllXmarksPos[pos.Value.x, pos.Value.y] = null;
            SallMarks.Remove(obj);
            Destroy(obj.gameObject);
            destroyObjects.RemoveAt(i);
        }
    }
}
