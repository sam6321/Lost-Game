using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

public class CreateTilePrefabs
{
    [MenuItem("Lost Tools/Create Tile Prefabs")]
    private static void CreateTilePrefabsOption()
    {
        foreach(Texture2D texture in Selection.GetFiltered<Texture2D>(SelectionMode.Unfiltered))
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(texture));

            GameObject gameObject = new GameObject(texture.name);
            SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;

            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size.Set(1.0f, 1.0f);

            PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/Prefabs/" + sprite.name + ".prefab");

            Object.DestroyImmediate(gameObject);
        }
    }
}

#endif