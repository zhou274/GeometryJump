



using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using TMPro;

/// <summary>
/// Class in charge to manage the mask icons in the character shop
/// </summary>
namespace AppAdvisory.GeometryJump
{
	public class MaskIcon : MonoBehaviorHelper
	{
		public Text[] texts;
		public Image[] images;

		public Button button;
		public TextMeshProUGUI buttonText;

		public GameObject lockImage;

		string id = null;

		public int price;

		public Image spriteMask;

		void Awake()
		{
			button = GetComponentInChildren<Button>();
			buttonText = button.transform.GetComponentInChildren<TextMeshProUGUI>();

			Logic();
		}

		void OnEnable()
		{
			Logic();
		}

		void OnDisable()
		{
			button.onClick.RemoveAllListeners();
		}

		void Logic()
		{
			foreach(var t in texts)
			{
				var c = t.color;

				c.a = alpha;

				t.color = c;
			}

			foreach(var t in images)
			{
				var c = t.color;

				c.a = alpha;

				t.color = c;
			}

			buttonText.text = text;

			lockImage.SetActive(!IsUnlock);

			button.onClick.AddListener(() => {
				if(IsUnlock)
				{
					ChangePlayerMask();
				}
				else
				{
					if(price <= gameManager.diamond)
					{
						gameManager.diamond -= price;
						IsUnlock = true;
						Logic();
						ChangePlayerMask();
					}
					else
					{
						transform.DOShakePosition(1,10,10,90);
					}
				}
			});
		}


		void ChangePlayerMask()
		{
			Transform t = transform.Find("Mask");
			if(t == null)
			{
				playerManager.SetMask(null);
				return;
			}
			Image i = t.GetComponent<Image>();
			Sprite s = i.sprite;
			playerManager.SetMask(s);
		}

		float alpha
		{
			get
			{
				float a = IsUnlock ? 1f : 0.5f;
				return a;
			}
		}

		string text
		{
			get
			{
				string s = IsUnlock ? "选择" : ("未解锁 " + price.ToString());

				return s;
			}
		}


		bool IsUnlock
		{
			get
			{
				if(id == null)
					id = gameObject.name;

				if(price == 0)
				{
					PlayerPrefs.SetInt(id,1);
					PlayerPrefs.Save();
				}

				bool isUnlock = PlayerPrefs.GetInt(id,0) == 1;

				return isUnlock;
			}

			set
			{
				if(value == true)
				{
					PlayerPrefs.SetInt(id,1);
					PlayerPrefs.Save();
				}
				else
				{
					PlayerPrefs.SetInt(id,0);
					PlayerPrefs.Save();
				}
			}
		}
	}
}