using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMarkControl : MonoBehaviour {
    public GameObject XMark;
    //A list that holds the matched objects.
    public List<Transform> destroyObjects = new();
    //A struct that holds all X mark, the column of the X mark, and the row of the X mark.
    [Serializable]
    public struct allMarks {
        public List<Transform> mark;
        public List<int> column;
        public List<int> row;

        public void Initialize() {
            mark = new();
            column = new();
            row = new();
        }
        //The function that adds items to the lists.
        public void Add(Transform xMarkTransform, int markColumn, int markRow) {
            mark.Add(xMarkTransform);
            column.Add(markColumn);
            row.Add(markRow);
        }
        
        //The function that returns the column of the specified X mark.
        public int XMarkColumn(Transform xMarkTransform) {
            if (xMarkTransform == null) return -1;
            int index = mark.IndexOf(xMarkTransform);
            if (index == -1) return -1;
            return column[index];
        }

        //The function that returns the row of the specified X mark.
        public int XMarkRow(Transform xMarkTransform) {
            if (xMarkTransform == null) return -1;
            int index = mark.IndexOf(xMarkTransform);
            if (index == -1) return -1;
            return row[index];
        }
        
        //The function that removes the specified X mark from all lists.
        public void RemoveAtIndex(Transform xMarkTransform) {
            if(xMarkTransform == null) return;
            int index = mark.IndexOf(xMarkTransform);
            if(index == -1) return;
            mark.RemoveAt(index);
            column.RemoveAt(index);
            row.RemoveAt(index);
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
                int column = (int)hit.transform.position.x;
                int row = (int)hit.transform.position.y;
                if (Board.I.XMarkRowColumn(column, row) == null) {
                    Board.I.AllXmarksPos[column, row] = mark;
                    SallMarks.Add(mark.transform, column, row);
                }
                
            }
        }
    }

    //The function that checks for matched X marks and adds them to the list.
    public void SetMatches() {
        for (int i = 0; i < SallMarks.mark.Count; i++) {
            List<Transform> matchedMark = new();
            Transform xMark = SallMarks.mark[i];
            int markColumn = SallMarks.column[i];
            int markRow = SallMarks.row[i];
            
            //The specified X mark being added to a local list after being checked.
            if(!matchedMark.Contains(xMark)) matchedMark.Add(xMark);
            
            //Checking the tile to the right.
            if (markColumn + 1 < Board.I.sideLength) {
                Transform rightXMark = Board.I.XMarkRowColumn(markColumn + 1, markRow);
                //If there is an X mark here and it is not in the local list.
                if (rightXMark != null && !matchedMark.Contains(rightXMark)) matchedMark.Add(rightXMark);
            }
            //Checking the tile to the left.
            if (markColumn -1 >= 0) {
                Transform leftXMark = Board.I.XMarkRowColumn(markColumn - 1, markRow);
                //If there is an X mark here and it is not in the local list.
                if (leftXMark != null && !matchedMark.Contains(leftXMark)) matchedMark.Add(leftXMark);
            }
            //Checking the tile to the up.
            if (markRow +1 < Board.I.sideLength) {
                Transform upXMark = Board.I.XMarkRowColumn(markColumn, markRow+1);
                //If there is an X mark here and it is not in the local list.
                if (upXMark != null && !matchedMark.Contains(upXMark)) matchedMark.Add(upXMark);
            }
            //Checking the tile to the down
            if (markRow -1 >= 0) {
                Transform downXMark = Board.I.XMarkRowColumn(markColumn, markRow-1);
                //If there is an X mark here and it is not in the local list.
                if (downXMark != null && !matchedMark.Contains(downXMark)) matchedMark.Add(downXMark);
            }
            
            //If the number of elements in the local list is 3 or more, add them to the list for the matching process.
            if (matchedMark.Count >= 3) {
                for (int j = 0; j < matchedMark.Count ; j++) {
                    if(!destroyObjects.Contains(matchedMark[j])) destroyObjects.Add(matchedMark[j]);
                }
            }
        }
    }
    
    //The function that performs the destroy operation and removes the item from the list in the struct.
    public void DestroyMatches() {
        for (int i = destroyObjects.Count-1; i >= 0; i--) {
            int destroyColumn = SallMarks.XMarkColumn(destroyObjects[i]);
            int destroyRow = SallMarks.XMarkRow(destroyObjects[i]);
            Board.I.AllXmarksPos[destroyColumn, destroyRow] = null;
            SallMarks.RemoveAtIndex(destroyObjects[i]);
            Destroy(destroyObjects[i].gameObject);
            destroyObjects.RemoveAt(i);
        }
    }
}
