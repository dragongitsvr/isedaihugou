using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Photon.Services
{
    public class LoadingService : MonoBehaviour
    {

        private const float DURATION = 1f;

        [SerializeField] private GameObject loading;
        [SerializeField] private GameObject loadingBackGround;

        /// <summary>
        /// ローディング画面を表示
        /// </summary>
        public void ShowLoading()
        {
            loading.SetActive(true);
            loadingBackGround.SetActive(true);
            Component[] circles = loading.GetComponentsInChildren<Image>();
            for (var i = 0; i < circles.Length; i++)
            {
                var angle = -2 * Mathf.PI * i / circles.Length;
                circles[i].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 50f;
                circles[i].GetComponent<Image>().DOFade(0f, DURATION).SetLoops(-1, LoopType.Yoyo).SetDelay(DURATION * i / circles.Length);
            }
        }

        /// <summary>
        /// ローディング画面を閉じる
        /// </summary>
        public void CloseLoading()
        {
            if(loading != null)
            {
                loading.SetActive(false);
            }
            if(loadingBackGround != null)
            {
                loadingBackGround.SetActive(false);
            }
        }
    }
}