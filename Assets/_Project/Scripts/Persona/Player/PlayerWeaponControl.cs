using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Persona.Player
{
    public class PlayerWeaponControl : MonoBehaviour
    {
        #region VARIABLES

        [Header("Setup")]
        public SpriteRenderer swordSprite;
        public Transform animCircle;
        public Transform aimCircle;
        public AudioSource audio;

        [Header("Data")]
        public Sprite defaultSword;
        public Sprite superSword;

        //Private
        private bool cooldown = false;

        #endregion

        #region FUNCTIONS

        public void Attack(Action<Transform> _attackComplete)
        {
            if(!cooldown)
            {
                audio.Play();
                cooldown = true;
                animCircle.DORotate(new Vector3(0, 0, 0), .2f)
                          .SetEase(Ease.Linear)
                          .OnComplete(() =>
                          {
                              animCircle.DORotate(new Vector3(0, 0, 0), 0);
                              StartCoroutine(CooldownEnd());
                          });

                _attackComplete(aimCircle);
            }
        }

        private IEnumerator CooldownEnd()
        {
            yield return new WaitForEndOfFrame();
            cooldown = false;
        }

        public void SuperSword(bool _enable)
        {
            if(!_enable)
                swordSprite.sprite = defaultSword;
            else
                swordSprite.sprite = superSword;
        }

        public void SuperDeflect(bool _enable)
        {
            if (!_enable)
                swordSprite.transform.localScale = new Vector3(1, 1, 1);
            else
                swordSprite.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }

        #endregion
    }
}