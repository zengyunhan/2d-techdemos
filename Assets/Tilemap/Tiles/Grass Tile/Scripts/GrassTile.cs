using System;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace UnityEngine.Tilemaps
{
    public class GrassTileTag
    {
        public const string Name = "GrassTileTag";
        public const int None = 0;
        public const int Grass = 1;
    }

    [Serializable]
	[CreateAssetMenu(fileName = "New Grass Tile", menuName = "Tiles/Grass Tile")]
	public class GrassTile : TileBase
	{
        [SerializeField]
		public Sprite[] m_Sprites;

        GridInformation GetOrCreateInfo(ITilemap tilemap)
        {
            GridInformation info = tilemap.GetComponent<GridInformation>();
            if (info == null)
                info = tilemap.GetComponent<Transform>().gameObject.AddComponent<GridInformation>();
            return info;
        }

        int GetTileTag(ITilemap tilemap, Vector3Int position)
        {
            return GetOrCreateInfo(tilemap).GetPositionProperty(position, GrassTileTag.Name, GrassTileTag.None);
        }

        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;

            int index = TileValue(tileMap, location + new Vector3Int(-1, 1, 0)) ? 1 : 0;
            index += TileValue(tileMap, location + new Vector3Int(0, 1, 0)) ? 2 : 0;
            index += TileValue(tileMap, location + new Vector3Int(0, 0, 0)) ? 4 : 0;
            index += TileValue(tileMap, location + new Vector3Int(-1, 0, 0)) ? 8 : 0;
            
            if (index >= 0 && index < m_Sprites.Length)
            {
                tileData.sprite = m_Sprites[index];
                tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
                tileData.colliderType = Tile.ColliderType.Sprite;
            }
        }

		private bool TileValue(ITilemap tileMap, Vector3Int position)
        {
            return GetTileTag(tileMap, position) == GrassTileTag.Grass;
        }
    }

#if UNITY_EDITOR
	[CustomEditor(typeof(GrassTile))]
	public class GrassTileEditor : Editor
	{
		private GrassTile tile { get { return (target as GrassTile); } }

		public void OnEnable()
		{
			if (tile.m_Sprites == null || tile.m_Sprites.Length != 16)
			{
				tile.m_Sprites = new Sprite[16];
				EditorUtility.SetDirty(tile);
			}
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.LabelField("Place sprites shown based on the contents of the sprite.");
			EditorGUILayout.Space();

			float oldLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 210;

			EditorGUI.BeginChangeCheck();
            tile.m_Sprites[0] = (Sprite)EditorGUILayout.ObjectField("0", tile.m_Sprites[0], typeof(Sprite), false, null);
            tile.m_Sprites[1] = (Sprite)EditorGUILayout.ObjectField("1", tile.m_Sprites[1], typeof(Sprite), false, null);
            tile.m_Sprites[2] = (Sprite)EditorGUILayout.ObjectField("2", tile.m_Sprites[2], typeof(Sprite), false, null);
            tile.m_Sprites[3] = (Sprite)EditorGUILayout.ObjectField("3", tile.m_Sprites[3], typeof(Sprite), false, null);
            tile.m_Sprites[4] = (Sprite)EditorGUILayout.ObjectField("4", tile.m_Sprites[4], typeof(Sprite), false, null);
            tile.m_Sprites[5] = (Sprite)EditorGUILayout.ObjectField("5", tile.m_Sprites[5], typeof(Sprite), false, null);
            tile.m_Sprites[6] = (Sprite)EditorGUILayout.ObjectField("6", tile.m_Sprites[6], typeof(Sprite), false, null);
            tile.m_Sprites[7] = (Sprite)EditorGUILayout.ObjectField("7", tile.m_Sprites[7], typeof(Sprite), false, null);
            tile.m_Sprites[8] = (Sprite)EditorGUILayout.ObjectField("8", tile.m_Sprites[8], typeof(Sprite), false, null);
            tile.m_Sprites[9] = (Sprite)EditorGUILayout.ObjectField("9", tile.m_Sprites[9], typeof(Sprite), false, null);
            tile.m_Sprites[10] = (Sprite)EditorGUILayout.ObjectField("10", tile.m_Sprites[10], typeof(Sprite), false, null);
            tile.m_Sprites[11] = (Sprite)EditorGUILayout.ObjectField("11", tile.m_Sprites[11], typeof(Sprite), false, null);
            tile.m_Sprites[12] = (Sprite)EditorGUILayout.ObjectField("12", tile.m_Sprites[12], typeof(Sprite), false, null);
            tile.m_Sprites[13] = (Sprite)EditorGUILayout.ObjectField("13", tile.m_Sprites[13], typeof(Sprite), false, null);
            tile.m_Sprites[14] = (Sprite)EditorGUILayout.ObjectField("14", tile.m_Sprites[14], typeof(Sprite), false, null);
            tile.m_Sprites[15] = (Sprite)EditorGUILayout.ObjectField("15", tile.m_Sprites[15], typeof(Sprite), false, null);
            if (EditorGUI.EndChangeCheck())
				EditorUtility.SetDirty(tile);

			EditorGUIUtility.labelWidth = oldLabelWidth;
		}
	}
#endif
}
