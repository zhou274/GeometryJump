




using UnityEngine;
using System.Collections;

/// <summary>
/// Class in charge to fix the player size in the game
/// </summary>
namespace AppAdvisory.GeometryJump
{
	public class FixeSpritePlayerSize : MonoBehaviour 
	{
		public SpriteRenderer sprite;
		public SpriteRenderer shadow;

		#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			FixSize();
		}
		#endif

		void FixSize()
		{
			if(sprite == null || shadow == null)
				return;

			var sizeSprite = sprite.sprite.bounds.size;

			print("size = " + sizeSprite);

			sprite.transform.localScale = Vector2.one * 1f / sizeSprite.x;

			shadow.transform.localScale = Vector2.one * 1f / sizeSprite.x;
		}
	}
}