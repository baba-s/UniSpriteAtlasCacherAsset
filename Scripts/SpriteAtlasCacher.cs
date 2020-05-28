using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace Kogane
{
	/// <summary>
	/// SpriteAtlas.GetSprite したスプライトをキャッシュするクラス
	/// </summary>
	[CreateAssetMenu( fileName = "SpriteAtlasCacher", order = 350 )]
	public sealed class SpriteAtlasCacher :
		ScriptableObject,
		IDisposable
	{
		//================================================================================
		// 変数(SerializeField)
		//================================================================================
		[SerializeField] private SpriteAtlas m_spriteAtlas = default;

		//================================================================================
		// 変数(readonly)
		//================================================================================
		private readonly Dictionary<string, Sprite> m_table = new Dictionary<string, Sprite>();

		//================================================================================
		// プロパティ
		//================================================================================
		public ICollection<KeyValuePair<string, Sprite>> Table         => m_table;
		public IReadOnlyCollection<string>               CachedNames   => m_table.Keys;
		public IReadOnlyCollection<Sprite>               CachedSprites => m_table.Values;

		//================================================================================
		// 関数
		//================================================================================
		/// <summary>
		/// 有効になった時に呼び出されます
		/// </summary>
		private void OnEnable()
		{
			if ( !Application.isPlaying ) return;
			Dispose();
		}

		/// <summary>
		/// 無効になった時に呼び出されます
		/// </summary>
		private void OnDisable()
		{
			if ( !Application.isPlaying ) return;
			Dispose();
		}

		/// <summary>
		/// <para>指定されたスプライトを返します</para>
		/// <para>キャッシュに存在しない場合は SpriteAtlas からスプライトを取得してキャッシュします</para>
		/// <para>キャッシュに存在する場合はキャッシュから返します</para>
		/// </summary>
		public Sprite GetSprite( string spriteName )
		{
			if ( m_spriteAtlas == null ) return null;

			if ( m_table.TryGetValue( spriteName, out var sprite ) ) return sprite;

			sprite = m_spriteAtlas.GetSprite( spriteName );
			m_table.Add( spriteName, sprite );
			return sprite;
		}

		/// <summary>
		/// SpriteAtlas に含まれているすべてのスプライトをキャッシュします
		/// </summary>
		public void CacheAll()
		{
			var sprites = new Sprite[m_spriteAtlas.spriteCount];
			m_spriteAtlas.GetSprites( sprites );

			m_table.Clear();

			for ( var i = 0; i < sprites.Length; i++ )
			{
				var sprite = sprites[ i ];
				var name   = sprite.name;

				name = name.Remove( name.Length - 7, 7 );

				m_table.Add( name, sprite );
			}
		}

		/// <summary>
		/// すべてのキャッシュを破棄します
		/// </summary>
		public void Dispose()
		{
			if ( m_spriteAtlas == null ) return;
			if ( m_table.Count <= 0 ) return;

			var sprites = m_table.Values.Where( sprite => sprite != null );

			foreach ( var sprite in sprites )
			{
				if ( Application.isPlaying )
				{
					Object.Destroy( sprite );
				}
				else
				{
					Object.DestroyImmediate( sprite );
				}
			}

			m_table.Clear();
		}

		//================================================================================
		// 関数(static)
		//================================================================================
#if UNITY_EDITOR

		/// <summary>
		/// <para>Unity 再生時にすべての SpriteAtlasCacher がキャッシュしているスプライトを解放します</para>
		/// <para>Enter Play Mode が有効な時は Unity 再生時に ScriptableObject.OnEnable が呼び出されず、</para>
		/// <para>2回目以降の Unity の再生でスプライトが正しく表示されなくなるため</para>
		/// <para>RuntimeInitializeOnLoadMethod でキャッシュされているスプライトを解放するようにしています</para>
		/// </summary>
		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		private static void Init()
		{
			var list = UnityEditor.AssetDatabase
					.FindAssets( $"t:{nameof( SpriteAtlasCacher )}" )
					.Select( x => UnityEditor.AssetDatabase.GUIDToAssetPath( x ) )
					.Select( x => UnityEditor.AssetDatabase.LoadAssetAtPath<SpriteAtlasCacher>( x ) )
					.ToArray()
				;

			for ( var i = 0; i < list.Length; i++ )
			{
				var n = list[ i ];
				n.Dispose();
			}
		}
#endif
	}
}