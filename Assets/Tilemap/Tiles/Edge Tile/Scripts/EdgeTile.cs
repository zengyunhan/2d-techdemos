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
	[CreateAssetMenu(fileName = "New Edge Tile", menuName = "Tiles/Edge Tile")]
	public class EdgeTile : TileBase
	{
        public enum eSpriteType
        {
            None = -1,
            OneSide,
            OneSideOneCornerRight,
            OneSideOneCornerLeft,
            OneSideTwoCorners,
            TwoSides,
            TwoAdjacentSidesRight,
            TwoAdjacentSidesLeft,
            TwoSidesOneCornerRight,
            TwoSidesOneCornerLeft,
            ThreeSides,
            Filled,
            OneCorner,
            TwoAdjacentCorners,
            TwoOppositeCorners,
            ThreeCorners,
            FourCorners,
        }

        Dictionary<byte, eSpriteType> spriteMap = new Dictionary<byte, eSpriteType>();
        protected EdgeTile()
        {
            spriteMap[1] = eSpriteType.OneSide;
            spriteMap[9] = eSpriteType.OneSideOneCornerRight;
            spriteMap[33] = eSpriteType.OneSideOneCornerLeft;
            spriteMap[41] = eSpriteType.OneSideTwoCorners;
            spriteMap[17] = eSpriteType.TwoSides;
            spriteMap[5] = eSpriteType.TwoAdjacentSidesRight;
            spriteMap[65] = eSpriteType.TwoAdjacentSidesLeft;
            spriteMap[37] = eSpriteType.TwoSidesOneCornerRight;
            spriteMap[73] = eSpriteType.TwoSidesOneCornerLeft;
            spriteMap[21] = eSpriteType.ThreeSides;
            spriteMap[85] = eSpriteType.Filled;
            spriteMap[2] = eSpriteType.OneCorner;
            spriteMap[10] = eSpriteType.TwoAdjacentCorners;
            spriteMap[34] = eSpriteType.TwoOppositeCorners;
            spriteMap[42] = eSpriteType.ThreeCorners;
            spriteMap[170] = eSpriteType.FourCorners;
        }

        [SerializeField]
		public Sprite[] m_Sprites;

		public override void RefreshTile(Vector3Int location, ITilemap tileMap)
        {
            for (int yd = -1; yd <= 1; yd++)
				for (int xd = -1; xd <= 1; xd++)
					tileMap.RefreshTile(new Vector3Int(location.x + xd, location.y + yd, location.z));
		}

		public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			UpdateTile(location, tileMap, ref tileData);
		}

		private void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			tileData.transform = Matrix4x4.identity;
			tileData.color = Color.white;

			int mask = TileValue(tileMap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(1, 1, 0)) ? 2 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(1, 0, 0)) ? 4 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(1, -1, 0)) ? 8 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(0, -1, 0)) ? 16 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(-1, -1, 0)) ? 32 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(-1, 0, 0)) ? 64 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(-1, 1, 0)) ? 128 : 0;

			byte original = (byte)mask;
			if ((original | 254) == 255) { mask = mask & 125; }
			if ((original | 251) == 255) { mask = mask & 245; }
			if ((original | 239) == 255) { mask = mask & 215; }
			if ((original | 191) == 255) { mask = mask & 95; }

            eSpriteType index = GetIndex((byte)mask);
			if (index >= 0 && (int)index < m_Sprites.Length)
            {
				tileData.sprite = m_Sprites[(int)index];
				tileData.transform = GetTransform((byte)mask);
				tileData.color = Color.white;
				tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
				tileData.colliderType = Tile.ColliderType.Sprite;
			}
		}

		private bool TileValue(ITilemap tileMap, Vector3Int position)
		{
			TileBase tile = tileMap.GetTile(position);
			return (tile != null && tile != this);
		}

        private byte CycleShift(byte val, int iShiftBit, bool isLeft)
        {
            if (iShiftBit == 0)
                return val;

            byte temp = 0;
            byte result = 0;
            temp |= val;
            if (isLeft)
            {
                val <<= iShiftBit;
                temp >>= (8 - iShiftBit);
                result = (byte)(val | temp);
            }
            else
            {
                val >>= iShiftBit;
                temp <<= (8 - iShiftBit);
                result = (byte)(val | temp);
            }
            return result;
        }

        private eSpriteType GetIndex(byte mask)
        {
            for(int i = 0; i < 4; i++)
            {
                byte temp = CycleShift(mask, 2 * i, true);
                if (spriteMap.ContainsKey(temp))
                    return spriteMap[temp];
            }
            
            return eSpriteType.None;
        }

        private Matrix4x4 GetTransform(byte mask)
        {
            for (int i = 0; i < 4; i++)
            {
                byte temp = CycleShift(mask, 2 * i, true);
                if (spriteMap.ContainsKey(temp))
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f * i), Vector3.one);
            }
            return Matrix4x4.identity;
        }
    }

#if UNITY_EDITOR
	[CustomEditor(typeof(EdgeTile))]
	public class EdgeTileEditor : Editor
	{
		private EdgeTile tile { get { return (target as EdgeTile); } }

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
            tile.m_Sprites[0] = (Sprite)EditorGUILayout.ObjectField("OneSide", tile.m_Sprites[0], typeof(Sprite), false, null);
            tile.m_Sprites[1] = (Sprite)EditorGUILayout.ObjectField("OneSideOneCornerRight", tile.m_Sprites[1], typeof(Sprite), false, null);
            tile.m_Sprites[2] = (Sprite)EditorGUILayout.ObjectField("OneSideOneCornerLeft", tile.m_Sprites[2], typeof(Sprite), false, null);
            tile.m_Sprites[3] = (Sprite)EditorGUILayout.ObjectField("OneSideTwoCorners", tile.m_Sprites[3], typeof(Sprite), false, null);
            tile.m_Sprites[4] = (Sprite)EditorGUILayout.ObjectField("TwoSides", tile.m_Sprites[4], typeof(Sprite), false, null);
            tile.m_Sprites[5] = (Sprite)EditorGUILayout.ObjectField("TwoAdjacentSidesRight", tile.m_Sprites[5], typeof(Sprite), false, null);
            tile.m_Sprites[6] = (Sprite)EditorGUILayout.ObjectField("TwoAdjacentSidesLeft", tile.m_Sprites[6], typeof(Sprite), false, null);
            tile.m_Sprites[7] = (Sprite)EditorGUILayout.ObjectField("TwoSidesOneCornerRight", tile.m_Sprites[7], typeof(Sprite), false, null);
            tile.m_Sprites[8] = (Sprite)EditorGUILayout.ObjectField("TwoSidesOneCornerLeft", tile.m_Sprites[8], typeof(Sprite), false, null);
            tile.m_Sprites[9] = (Sprite)EditorGUILayout.ObjectField("ThreeSides", tile.m_Sprites[9], typeof(Sprite), false, null);
            tile.m_Sprites[10] = (Sprite)EditorGUILayout.ObjectField("Filled", tile.m_Sprites[10], typeof(Sprite), false, null);
            tile.m_Sprites[11] = (Sprite)EditorGUILayout.ObjectField("OneCorner", tile.m_Sprites[11], typeof(Sprite), false, null);
            tile.m_Sprites[12] = (Sprite)EditorGUILayout.ObjectField("TwoAdjacentCorners", tile.m_Sprites[12], typeof(Sprite), false, null);
            tile.m_Sprites[13] = (Sprite)EditorGUILayout.ObjectField("TwoOppositeCorners", tile.m_Sprites[13], typeof(Sprite), false, null);
            tile.m_Sprites[14] = (Sprite)EditorGUILayout.ObjectField("ThreeCorners", tile.m_Sprites[14], typeof(Sprite), false, null);
            tile.m_Sprites[15] = (Sprite)EditorGUILayout.ObjectField("FourCorners", tile.m_Sprites[15], typeof(Sprite), false, null);
            if (EditorGUI.EndChangeCheck())
				EditorUtility.SetDirty(tile);

			EditorGUIUtility.labelWidth = oldLabelWidth;
		}
	}
#endif
}
