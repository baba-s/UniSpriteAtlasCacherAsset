# UniSpriteAtlasCacherAsset

SpriteAtlas.GetSprite したスプライトをキャッシュするクラス（ScriptableObject 版）

## 使用例

![2020-06-17_212316](https://user-images.githubusercontent.com/6134875/84897715-0cf90d00-b0e1-11ea-9b7a-fe8c91907624.png)

Project ビューの「+ > Sprite Atlas Cacher」を選択して  

![2020-06-17_212402](https://user-images.githubusercontent.com/6134875/84897718-0d91a380-b0e1-11ea-9677-17cb6c2f8a32.png)

生成された SpriteAtlasCacher の「SpriteAtlas」の欄に、  
GetSprite したスプライトをキャッシュしておきたい SpriteAtlas を設定する  

後は下記のようなスクリプトを作成することで使用できます  

```cs
using Kogane;
using UnityEngine;

public class Example : MonoBehaviour
{
    public SpriteAtlasCacher m_cacher;

    private void Start()
    {
        // 指定されたスプライトを SpriteAtlas.GetSprite して内部でキャッシュする
        var sprite = m_cacher.GetSprite( "【スプライト名】" );

        // SpriteAtlas に含まれているすべてのスプライトをキャッシュする
        m_cacher.CacheAll();

        // キャッシュされているすべてのスプライトの情報を取得する
        foreach ( var n in m_cacher.Table )
        {
            Debug.Log( n.Key + ":" + n.Value.name );
        }
        
        // キャッシュされているすべてのスプライトの名前を取得する
        foreach ( var n in m_cacher.CachedNames )
        {
            Debug.Log( n );
        }
        
        // キャッシュされているすべてのスプライトを取得する
        foreach ( var n in m_cacher.CachedSprites )
        {
            Debug.Log( n.name );
        }
    }

    private void OnDestroy()
    {
        // SpriteAtlas.GetSprite したすべてのスプライトを破棄する
        m_cacher.Dispose();
    }
}
```
