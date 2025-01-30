using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class XMarkControl : MonoBehaviour {
    
    [Serializable]
    public class allMarks {
        //A dictionary that allows us to store the column and row from the transform
        public Dictionary<Transform, Vector2Int> marks = new();
        
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

    public allMarks CallMarks = new();
    
    void Update() {
        ClickFunc();
    }

    //The function that places the X mark on top of the tiles. 
    public void ClickFunc() {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null) {
            int column = Mathf.RoundToInt(hit.transform.position.x);
            int row = Mathf.RoundToInt(hit.transform.position.y);
            
            if (Board.I.XMarkRowColumn(column, row) == null) {
                Transform mark = ObjectPooling.I.CobjectPool.GetPooledObject();
                mark.transform.position = hit.transform.position;
                Board.I.AllXmarksPos[column, row] = mark.gameObject;
                CallMarks.Add(mark.transform, column, row);
                SetMatches();
            }
        }
        
    }

    //The function that checks for matched X marks and adds them to the list.
    public void SetMatches() {
        //Stores the previously visited "X Mark" objects.
        HashSet<Transform> checkedMarks = new();
        //Conversion of the Keys list into a list.
        List<Transform> allMarks = CallMarks.marks.Keys.ToList();

        for (int i = 0; i < allMarks.Count; i++) {
            Transform xMark = allMarks[i];
            if (checkedMarks.Contains(xMark)) continue;
            //Stores the "X Mark" objects that are connected to the starting mark.
            List<Transform> matchedMark = new();
            ScanNeighbors(xMark, matchedMark, checkedMarks);
            if (matchedMark.Count >= 3) {
                for (int j = 0; j < matchedMark.Count ; j++) {
                    Transform obj = matchedMark[j];
                    Vector2Int? pos = CallMarks.GetPosition(obj);
                    if (pos == null) continue;
                    
                    //Set the GameObject in the current column and row to null.
                    Board.I.AllXmarksPos[pos.Value.x, pos.Value.y] = null;
                    //Removing X mark from dictionary
                    CallMarks.Remove(obj);
                    //Deactivate the active X mark.
                    ObjectPooling.I.CobjectPool.ReturnToPool(obj.gameObject);
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

            Vector2Int? pos = CallMarks.GetPosition(current);
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
}
