using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View.Components
{
    public class MenuButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region VARIABLES

        [Header("UI")]
        public Image cursor;
        public AudioSource audio;

        #endregion

        #region UNITY EVENTS

        void Start()
        {
            ShowHideCursor(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowHideCursor(true);

            if(audio != null)
                audio.Play();
        }

        public void OnPointerExit(PointerEventData eventData)
            => ShowHideCursor(false);

        #endregion

        #region FUNCTIONS

        public void ShowHideCursor(bool _show) 
            => cursor.gameObject.SetActive(_show);

        #endregion
    }
}