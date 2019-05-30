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
    [CustomGridBrush(true, false, false, "Grass Brush")]
    [CreateAssetMenu(fileName = "New Grass Brush", menuName = "Brushes/Grass Brush")]
#if UNITY_EDITOR
    public class GrassBrush : GridBrush {
#else
    public class GrassBrush : GridBrushBase {
#endif
        [SerializeField]
        public GrassTile Tile;

        GridInformation GetOrCreateInfo(Tilemap tilemap)
        {
            GridInformation info = tilemap.GetComponent<GridInformation>();
            if (info == null)
                info = tilemap.GetComponent<Transform>().gameObject.AddComponent<GridInformation>();
            return info;
        }

        void SetTileTag(Tilemap tilemap, Vector3Int position)
        {
            GetOrCreateInfo(tilemap).SetPositionProperty(position, GrassTileTag.Name, GrassTileTag.Grass);
        }

        int GetTileTag(Tilemap tilemap, Vector3Int position)
        {
            return GetOrCreateInfo(tilemap).GetPositionProperty(position, GrassTileTag.Name, GrassTileTag.None);
        }

        void EraseTileTag(Tilemap tilemap, Vector3Int position)
        {
            GetOrCreateInfo(tilemap).ErasePositionProperty(position, GrassTileTag.Name);
        }

        private void UpdateAround(Tilemap tilemap, Vector3Int position)
        {
            tilemap.RefreshTile(position);

            for (int yd = 0; yd <= 1; yd++)
            {
                for (int xd = -1; xd <= 0; xd++)
                {
                    Vector3Int posAround = new Vector3Int(position.x + xd, position.y + yd, position.z);
                    if (GetTileTag(tilemap, posAround) == GrassTileTag.Grass)
                        return;
                }
            }

            tilemap.SetTile(position, null);
        }
        
        public override void Pick(GridLayout grid, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
        {
            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;
            
            Tile = tilemap.GetTile(new Vector3Int(position.xMin, position.yMin, position.zMin)) as GrassTile;
        }
        
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            SetTileTag(tilemap, position);

            for (int yd = -1; yd <= 0; yd++)
            {
                for (int xd = 0; xd <= 1; xd++)
                {
                    Vector3Int posAround = new Vector3Int(position.x + xd, position.y + yd, position.z);
                    TileBase tile = tilemap.GetTile(posAround);
                    if (tile == null)
                        tilemap.SetTile(posAround, Tile);

                    tilemap.RefreshTile(posAround);
                }
            }
        }
        
        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            EraseTileTag(tilemap, position);

            for (int yd = -1; yd <= 0; yd++)
            {
                for (int xd = 0; xd <= 1; xd++)
                {
                    UpdateAround(tilemap, new Vector3Int(position.x + xd, position.y + yd, position.z));
                }
            }
        }
        /*
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
        }*/
    }

    [CustomEditor(typeof(GrassBrush))]
    public class GrassBrushEditor : GridBrushEditor
    {
        private GrassBrush Brush { get { return target as GrassBrush; } }
        //private Tilemap tilemap;

        /*
        private void paintPreview(Tilemap tilemap, Vector3Int position)
        {
            tilemap.SetEditorPreviewTile(position, Brush.Tile);
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
        }*/
        /*
        public override void OnToolActivated(GridBrushBase.Tool tool)
        {
            base.OnToolActivated(tool);

            if (tilemap == null)
                return;

            tilemap.ClearAllEditorPreviewTiles();
        }
        
        public override void OnToolDeactivated(GridBrushBase.Tool tool)
        {
            base.OnToolDeactivated(tool);

            if (tilemap == null)
                return;
            
            tilemap.ClearAllEditorPreviewTiles();
        }
        
        */
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
            Brush.Tile = (GrassTile)EditorGUILayout.ObjectField("GrassTile", Brush.Tile, typeof(GrassTile), false, null);
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(Brush);
        }
    }
}
