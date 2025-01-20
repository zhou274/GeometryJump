




using UnityEngine;
using System.Collections;

/// <summary>
/// Class in charge to manage the diamonds
/// </summary>
namespace AppAdvisory.GeometryJump
{
	public class DiamondLogic : MonoBehaviorHelper 
	{
		Transform p;

		void Update()
		{
			if(p == null)
				p = playerManager.transform;

			var d = Vector2.Distance(p.position,transform.position);

			if(d < 0.1f)
			{
				if(gameManager != null)
					gameManager.diamond ++;

				gameObject.SetActive(false);
			}
		}
	}
}