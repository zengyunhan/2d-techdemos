using System;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace UnityEngine.Tilemaps
{
    [Serializable]
    [CustomGridBrush(true, false, false, "Edge Brush")]
    [CreateAssetMenu(fileName = "New Edge Brush", menuName = "Brushes/Edge Brush")]
#if UNITY_EDITOR
    public class EdgeBrush : GridBrush {
#else
    public class EdgeBrush : GridBrushBase {
#endif
        [SerializeField]
        public TileBase Tile;
        [SerializeField]
        public TileBase TileEdge;

        private void DrawTile(Tilemap tilemap, Vector3Int position)
        {
            tilemap.SetTile(position, Tile);

            for (int yd = -1; yd <= 1; yd++)
            {
                for (int xd = -1; xd <= 1; xd++)
                {
                    Vector3Int posAround = new Vector3Int(position.x + xd, position.y + yd, position.z);
                    TileBase tile = tilemap.GetTile(posAround);
                    if (tile == null || tile == TileEdge)
                    {
                        tilemap.SetTile(posAround, TileEdge);
                        tilemap.RefreshTile(posAround);
                    }
                }
            }
        }

        private void UpdateEdge(Tilemap tilemap, Vector3Int position)
        {
            for (int yd = -1; yd <= 1; yd++)
            {
                for (int xd = -1; xd <= 1; xd++)
                {
                    TileBase tile = tilemap.GetTile(new Vector3Int(position.x + xd, position.y + yd, position.z));
                    if (tile != TileEdge)
                    {
                        tilemap.SetTile(position, TileEdge);
                        tilemap.RefreshTile(position);
                        return;
                    }
                }
            }
            tilemap.SetTile(position, null);
            tilemap.RefreshTile(position);
        }

        public override void Pick(GridLayout grid, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
        {
            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            TileBase tile = tilemap.GetTile(new Vector3Int(position.xMin, position.yMin, position.zMin));
            if (tile == TileEdge)
                tile = null;

            Tile = tile;
        }

        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            DrawTile(tilemap, position);
        }

        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            tilemap.SetTile(position, null);

            for (int yd = -1; yd <= 1; yd++)
            {
                for (int xd = -1; xd <= 1; xd++)
                {
                    Vector3Int posAround = new Vector3Int(position.x + xd, position.y + yd, position.z);
                    TileBase tile = tilemap.GetTile(posAround);
                    if (tile == null || tile == TileEdge)
                    {
                        UpdateEdge(tilemap, posAround);
                    }
                }
            }
        }

        public override void FloodFill(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            //var zPosition = new Vector3Int(position.x, position.y, z);
            //base.FloodFill(grid, brushTarget, zPosition);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt bounds)
        {
            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;
            for (int yd = bounds.yMin; yd < bounds.yMax; yd++)
            {
                for (int xd = bounds.xMin; xd < bounds.xMax; xd++)
                {
                    DrawTile(tilemap, new Vector3Int(xd, yd, bounds.z));
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EdgeBrush))]
    public class EdgeBrushEditor : GridBrushEditor
    {
        private EdgeBrush edgeBrush { get { return target as EdgeBrush; } }
        private Tilemap tilemap;

        private void paintPreview(Tilemap tilemap, Vector3Int position)
        {
            tilemap.SetEditorPreviewTile(position, edgeBrush.Tile);

            for (int yd = -1; yd <= 1; yd++)
            {
                for (int xd = -1; xd <= 1; xd++)
                {
                    Vector3Int posAround = new Vector3Int(position.x + xd, position.y + yd, position.z);
                    TileBase tilePreview = tilemap.GetEditorPreviewTile(posAround);
                    if (tilePreview == null)
                    {
                        TileBase tile = tilemap.GetTile(posAround);
                        if (tile == null || tile == edgeBrush.TileEdge)
                        {
                            tilemap.SetEditorPreviewTile(posAround, edgeBrush.TileEdge);
                        }
                    }
                }
            }
        }

        public override void PaintPreview(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            tilemap.ClearAllEditorPreviewTiles();
            paintPreview(tilemap, position);
        }

        public override void BoxFillPreview(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            tilemap.ClearAllEditorPreviewTiles();
            //tilemap.EditorPreviewBoxFill(position.position, edgeBrush.Tile, position.x, position.y, position.x + position.size.x - 1, position.y + position.size.y - 1);
            for (int yd = position.yMin; yd < position.yMax; yd++)
            {
                for (int xd = position.xMin; xd < position.xMax; xd++)
                {
                    paintPreview(tilemap, new Vector3Int(xd, yd, position.z));
                    //PaintPreview(gridLayout, brushTarget, new Vector3Int(xd, yd, position.z));
                }
            }
            //tilemap.EditorPreviewBoxFill(position.position, edgeBrush.Tile, position.xMin, position.yMin, position.xMax - 1, position.yMax - 1);
        }

        public override void OnToolDeactivated(GridBrushBase.Tool tool)
        {
            base.OnToolDeactivated(tool);

            if (tilemap == null)
                return;

            tilemap.ClearAllEditorPreviewTiles();
        }

        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);

            if (position.z != 0)
            {
                var zPosition = new Vector3Int(position.min.x, position.min.y, position.z);
                BoundsInt newPosition = new BoundsInt(zPosition, position.size);
                Vector3[] cellLocals = new Vector3[]
                {
                    grid.CellToLocal(new Vector3Int(newPosition.min.x, newPosition.min.y, newPosition.min.z)),
                    grid.CellToLocal(new Vector3Int(newPosition.max.x, newPosition.min.y, newPosition.min.z)),
                    grid.CellToLocal(new Vector3Int(newPosition.max.x, newPosition.max.y, newPosition.min.z)),
                    grid.CellToLocal(new Vector3Int(newPosition.min.x, newPosition.max.y, newPosition.min.z))
                };

                Handles.color = Color.blue;
                int i = 0;
                for (int j = cellLocals.Length - 1; i < cellLocals.Length; j = i++)
                {
                    Handles.DrawLine(cellLocals[j], cellLocals[i]);
                }
            }


            var labelText = "Pos: " + new Vector2Int(position.x, position.y);
            if (position.size.x > 1 || position.size.y > 1) {
                labelText += " Size: " + new Vector2Int(position.size.x, position.size.y);
            }

            Handles.Label(grid.CellToWorld(new Vector3Int(position.x, position.y, position.z)), labelText);
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            edgeBrush.Tile = (TileBase)EditorGUILayout.ObjectField("Tile", edgeBrush.Tile, typeof(TileBase), false, null);
            edgeBrush.TileEdge = (TileBase)EditorGUILayout.ObjectField("TileEdge", edgeBrush.TileEdge, typeof(TileBase), false, null);
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(edgeBrush);
        }
    }
#endif
}
