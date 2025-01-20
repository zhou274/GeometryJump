




#pragma warning disable 0618 

using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;
using System.Collections.Generic;



#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

#if APPADVISORY_ADS
using AppAdvisory.Ads;
#endif

/// <summary>
/// Class in charge to manage all the UI in the game
/// </summary>
namespace AppAdvisory.GeometryJump
{
	public class CanvasManager : MonoBehaviorHelper 
	{
		/// <summary>
		/// We show ads - interstitials - ever 10 game over by default. To change it, change this number. You have to get "Very Simple Ad" from the asset store to use it: http://u3d.as/oWD
		/// </summary>
		public int numberOfPlayToShowInterstitial = 5;

		public string VerySimpleAdsURL = "http://u3d.as/oWD";

		[SerializeField] private Text scoreText; 
		[SerializeField] private TextMeshProUGUI bestScoreText; 
		[SerializeField] TextMeshProUGUI lastScoreText; 

		[SerializeField] private CanvasGroup canvasGroupInstruction;
		[SerializeField] private CanvasGroup canvasGroupScore;
		[SerializeField] private CanvasGroup canvasGroupGameOver;
		[SerializeField] private CanvasGroup canvasGroupMaskShop;

		[SerializeField] private Text lifeText;
		[SerializeField] private Text diamondText;

		[SerializeField] private Button buttonContinueWithLife;
		[SerializeField] private Button buttonContinueWithDiamond;
		[SerializeField] private Button buttonGetFreeLife;
		[SerializeField] private Button buttonGetFreeDiamonds;
		[SerializeField] private Button buttonRestart;
		[SerializeField] private Button buttonCloseMask;

		public AnimationTransitionManager m_animationTransitionManager;
		public AnimationTransition m_animationTransition;

		public delegate void AnimationTransitionInStart();
		public static event AnimationTransitionInStart OnAnimationTransitionInStart;

		public delegate void AnimationTransitionInEnd();
		public static event AnimationTransitionInEnd OnAnimationTransitionInEnd;

		public delegate void AnimationTransitionOutStart();
		public static event AnimationTransitionOutStart OnAnimationTransitionOutStart;

		public delegate void AnimationTransitionOutEnd();
		public static event AnimationTransitionOutEnd OnAnimationTransitionOutEnd;

        public string clickid;
        private StarkAdManager starkAdManager;
        public void _AnimationTransitionInStart()
		{
			if(OnAnimationTransitionInStart != null)
				OnAnimationTransitionInStart();
		}
		public void _AnimationTransitionInEnd()
		{
			if(OnAnimationTransitionInStart != null)
				OnAnimationTransitionInEnd();
		}
		public void _AnimationTransitionOutStart()
		{
			if(OnAnimationTransitionOutStart != null)
				OnAnimationTransitionOutStart();
		}
		public void _AnimationTransitionOutEnd()
		{
			if(OnAnimationTransitionOutEnd != null)
				OnAnimationTransitionOutEnd();
		}


		float timeAlphaAnim = 1f;

		float alphaInstruction
		{
			get
			{
				return canvasGroupInstruction.alpha;
			}

			set
			{
				canvasGroupInstruction.alpha = value;
				//			canvasGroupScore.alpha = 1f - value;
			}
		}

		void OnEnable()
		{
			GameManager.OnSetPoint += SetPointText;

			GameManager.OnSetDiamond += SetDiamondText;

			GameManager.OnGameStart += OnGameStart;

			GameManager.OnGameOverEnded += OnGameOverEnded;
		}

		void OnDisable()
		{
			GameManager.OnSetPoint -= SetPointText;

			GameManager.OnSetDiamond -= SetDiamondText;

			GameManager.OnGameStart -= OnGameStart;

			GameManager.OnGameOverEnded -= OnGameOverEnded;
		}

		void Start()
		{
			SetBestScoreText(gameManager.GestBestScore());

			SetPointText(0);

			lastScoreText.text = "上一次分数: " + gameManager.GestLastScore();

			lifeText.text = "x " + gameManager.GetLife().ToString();

			alphaInstruction = 1f;

			canvasGroupGameOver.alpha = 0;

			canvasGroupMaskShop.alpha = 0;

			canvasGroupGameOver.gameObject.SetActive(false);

			SetDiamondText(gameManager.diamond);
		}

		void ButtonLogic()
		{
			bool adsInitialized = false;

			#if APPADVISORY_ADS
			adsInitialized = AdsManager.instance.IsReadyRewardedVideo ();
			#endif

			bool haveLife = gameManager.HaveLife();
			bool haveEnoughtDiamond = gameManager.diamond >= 100;

			ActivateButton(buttonContinueWithLife, haveLife);
			ActivateButton(buttonGetFreeLife, adsInitialized);
			ActivateButton(buttonGetFreeDiamonds, adsInitialized);
			ActivateButton(buttonContinueWithDiamond, haveEnoughtDiamond);
		}

		void ActivateButton(Button b, bool activate)
		{
			b.GetComponent<CanvasGroup>().alpha = activate ? 1 : 0.5f;
			b.interactable = activate;
		}

		void OnGameOverEnded()
		{
            ShowInterstitialAd("1lcaf5895d5l1293dc",
            () => {
                Debug.LogError("--插屏广告完成--");

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
            ButtonLogic();

			canvasGroupGameOver.alpha = 0;

			canvasGroupGameOver.gameObject.SetActive(true);

			canvasGroupGameOver.DOFade(1,timeAlphaAnim)
				.OnComplete(() => {
					AddButtonListener();
				});

			#if !(UNITY_ANDROID || UNITY_IOS) && UNITY_TVOS
			for(int i = 0; i < tc.childCount; i++)
			{
			var t = tc.GetChild(i);
			if(tc.gameObject.activeInHierarchy)
			{
			var es = FindObjectOfType<EventSystem>();
			es.firstSelectedGameObject = t.gameObject;
			es.SetSelectedGameObject(t.gameObject);

			print("set selected: " + t.name);
			break;
			}
			}
			#endif
		}

		void OnGameStart()
		{
			//		SetCanvasGroupInstructionAlpha(1,0);
			//
			//		canvasGroupInstruction.GetComponent<AnimButtonHierarchy>().DoAnimOut();
		}

		void SetPointText(int point)
		{
			scoreText.text = point.ToString();

			int best = gameManager.GestBestScore ();

			if (point > best)
				SetBestScoreText(point);
		}

		void SetDiamondText(int diamond)
		{
			diamondText.text = "x " + diamond.ToString();
		}

		void SetBestScoreText(int p)
		{
			bestScoreText.text = "最高分: " + p.ToString();
		}

		public void SetCanvasGroupInstructionAlpha(float fromA, float toA)
		{
			DOVirtual.Float(fromA, toA, timeAlphaAnim, (float f) => {
				alphaInstruction = f;
			})
				.OnComplete(() => {
					alphaInstruction = toA;

					if(toA == 0)
					{
						canvasGroupGameOver.gameObject.SetActive(false);
					}
				});
		}

		public void SetCanvasGroupGameOverAlpha(float fromA, float toA)
		{
			DOVirtual.Float(fromA, toA, timeAlphaAnim, (float f) => {
				canvasGroupGameOver.alpha = f;
			})
				.OnComplete(() => {
					canvasGroupGameOver.alpha = toA;

					if(toA == 0)
					{
						canvasGroupGameOver.gameObject.SetActive(false);
					}

					AddButtonListener();
				});
		}

		void AddButtonListener()
		{
			bool adsInitialized = false;

			#if APPADVISORY_ADS
			adsInitialized = AdsManager.instance.IsReadyRewardedVideo ();
			#endif

			bool haveLife = gameManager.HaveLife();
			bool haveEnoughtDiamond = gameManager.diamond >= 100;


			ActivateButton(buttonContinueWithLife, haveLife);
			ActivateButton(buttonGetFreeLife, adsInitialized);
			ActivateButton(buttonGetFreeDiamonds, adsInitialized);
			ActivateButton(buttonContinueWithDiamond, haveEnoughtDiamond);

			buttonRestart.interactable = true;
		}

		void RemoveListener()
		{
			buttonContinueWithLife.interactable = false;
			buttonGetFreeLife.interactable = false;
			buttonGetFreeDiamonds.interactable = false;
			buttonContinueWithDiamond.interactable = false;
			buttonRestart.interactable = false;
		}

		public void OnClickedButton(GameObject b)
		{
			if(!b.GetComponent<Button>().interactable)
				return;

			if (b.name.Contains("ContinueWithLife"))
				OnClickedContinueWithLife();
			else if (b.name.Contains("ContinueWithDiamond"))
				OnClickedContinueWithDiamond();
			else if (b.name.Contains("ButtonGet3Life"))
				OnClickedGetFreeLife();
			else if (b.name.Contains("ButtonGet300Diamonds"))
				OnClickedGetFreeDiamonds();
			else if (b.name.Contains("GameOver"))
				OnClickedRestart();
			else if (b.name.Contains("ButtonMask"))
				OnClickedButtonMask();
		}

		void OnClickedButtonMask()
		{
			canvasGroupMaskShop.alpha = 0;
			canvasGroupMaskShop.gameObject.SetActive(true);

			canvasGroupMaskShop.DOFade(1,1);

			canvasGroupMaskShop.interactable = true;
			canvasGroupMaskShop.blocksRaycasts = true;

			buttonCloseMask.onClick.AddListener(CloseButtonMask);
		}

		void CloseButtonMask()
		{
			canvasGroupMaskShop.DOFade(0,1)
				.OnComplete( () => {
					canvasGroupMaskShop.blocksRaycasts = false;
					canvasGroupMaskShop.alpha = 0;
					canvasGroupMaskShop.gameObject.SetActive(false);
				});

			canvasGroupMaskShop.interactable = false;
		}

		void OnClickedContinueWithLife()
		{
			RemoveListener();
			SetCanvasGroupGameOverAlpha(1,0);
			gameManager.AddLife(-1);
			lifeText.text = "x " + gameManager.GetLife().ToString();
			playerManager.Continue();
		}
		public void Continue()
		{
            ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {
                    SetCanvasGroupGameOverAlpha(1, 0);
                    playerManager.Continue();
                    //gameManager.diamond += 500;



                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
            
        }
		public void AddDiamonds()
		{
            ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {

                    gameManager.diamond += 100;


                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
            
        }
		void OnClickedContinueWithDiamond()
		{
			RemoveListener();
			SetCanvasGroupGameOverAlpha(1,0);
			gameManager.diamond -= 100;
			playerManager.Continue();
		}

		void OnClickedGetFreeLife()
		{
			RemoveListener();

			#if APPADVISORY_ADS
			AdsManager.instance.ShowRewardedVideo((bool success) => {
				if(success)
				{
					gameManager.AddLife(3);
					lifeText.text = "x " + gameManager.GetLife().ToString();
				}
				ButtonLogic();
				AddButtonListener();
				buttonGetFreeLife.gameObject.SetActive(false);
			});
			#endif
		}

		void OnClickedGetFreeDiamonds()
		{
			RemoveListener();

			#if APPADVISORY_ADS
			AdsManager.instance.ShowRewardedVideo((bool success) => {
				if(success)
				{
					gameManager.diamond += 300;
					diamondText.text = "x " + gameManager.diamond.ToString();
				}
				ButtonLogic();
				AddButtonListener();
				buttonGetFreeDiamonds.gameObject.SetActive(false);
			});
			#endif
		}

		/// <summary>
		/// If using Very Simple Ads by App Advisory, show an interstitial if number of play > numberOfPlayToShowInterstitial: http://u3d.as/oWD
		/// </summary>
		public void ShowAds()
		{
			int count = PlayerPrefs.GetInt("GAMEOVER_COUNT",0);
			count++;
			PlayerPrefs.SetInt("GAMEOVER_COUNT",count);
			PlayerPrefs.Save();

			#if APPADVISORY_ADS
			if(count > numberOfPlayToShowInterstitial)
			{
				print("count = " + count + " > numberOfPlayToShowINterstitial = " + numberOfPlayToShowInterstitial);

				if(AdsManager.instance.IsReadyInterstitial())
				{
					print("AdsManager.instance.IsReadyInterstitial() == true ----> SO ====> set count = 0 AND show interstial");
					AdsManager.instance.ShowInterstitial();
					PlayerPrefs.SetInt("GAMEOVER_COUNT",0);
				}
				else
				{
			#if UNITY_EDITOR
					print("AdsManager.instance.IsReadyInterstitial() == false");
			#endif
				}

			}
			else
			{
				PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
			}
			PlayerPrefs.Save();
			#else
			if(count >= numberOfPlayToShowInterstitial)
			{
			Debug.LogWarning("To show ads, please have a look to Very Simple Ad on the Asset Store, or go to this link: " + VerySimpleAdsURL);
			Debug.LogWarning("Very Simple Ad is already implemented in this asset");
			Debug.LogWarning("Just import the package and you are ready to use it and monetize your game!");
			Debug.LogWarning("Very Simple Ad : " + VerySimpleAdsURL);
			PlayerPrefs.SetInt("GAMEOVER_COUNT",0);
			}
			else
			{
			PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
			}
			PlayerPrefs.Save();
			#endif
		}

		public void OnClickedRestart()
		{
			var an = FindObjectsOfType<AnimButtonHierarchy>();

			foreach(var a in an)
			{
				if(a.gameObject.activeInHierarchy)
					a.DoAnimOut();
			}

			DOTween.KillAll();

			ShowAds();

			m_animationTransition.DoAnimationIn( () => {
				RemoveListener();
				StopAllCoroutines();
				PlayerPrefsX.SetColor("BACKGROUND_COLOR", cam.backgroundColor);
				PlayerPrefs.Save();

				#if UNITY_5_3_OR_NEWER
				DOTween.KillAll();

				GC.Collect();

				Resources.UnloadUnusedAssets();

				SceneManager.LoadSceneAsync(0,LoadSceneMode.Single);

				Resources.UnloadUnusedAssets();

				GC.Collect();
				#else
				Application.LoadLevel (Application.loadedLevel);
				#endif
			});

		}
        public void getClickid()
        {
            var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
            if (launchOpt.Query != null)
            {
                foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                    if (kv.Value != null)
                    {
                        Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                        if (kv.Key.ToString() == "clickid")
                        {
                            clickid = kv.Value.ToString();
                        }
                    }
                    else
                    {
                        Debug.Log(kv.Key + "<-参数-> " + "null ");
                    }
            }
        }

        public void apiSend(string eventname, string clickid)
        {
            TTRequest.InnerOptions options = new TTRequest.InnerOptions();
            options.Header["content-type"] = "application/json";
            options.Method = "POST";

            JsonData data1 = new JsonData();

            data1["event_type"] = eventname;
            data1["context"] = new JsonData();
            data1["context"]["ad"] = new JsonData();
            data1["context"]["ad"]["callback"] = clickid;

            Debug.Log("<-data1-> " + data1.ToJson());

            options.Data = data1.ToJson();

            TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
               response => { Debug.Log(response); },
               response => { Debug.Log(response); });
        }


        /// <summary>
        /// </summary>
        /// <param name="adId"></param>
        /// <param name="closeCallBack"></param>
        /// <param name="errorCallBack"></param>
        public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
        {
            starkAdManager = StarkSDK.API.GetStarkAdManager();
            if (starkAdManager != null)
            {
                starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
            }
        }
        /// <summary>
        /// 播放插屏广告
        /// </summary>
        /// <param name="adId"></param>
        /// <param name="errorCallBack"></param>
        /// <param name="closeCallBack"></param>
        public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
        {
            starkAdManager = StarkSDK.API.GetStarkAdManager();
            if (starkAdManager != null)
            {
                var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
                mInterstitialAd.Load();
                mInterstitialAd.Show();
            }
        }
    }
}