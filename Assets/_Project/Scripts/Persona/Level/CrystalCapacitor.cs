using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Common.CommonData;

namespace Persona.Level
{
    public class CrystalCapacitor : MonoBehaviour
    {
        #region VARIABLES

        [Header("Setup")]
        public Animator animator;

        [Header("Sound")]
        public AudioSource audioSource;
        public AudioClip getInSound;
        public AudioClip getoutSound;

        //Private
        private bool destructable = false;

        //Events
        public static event CustomEvent DestroyCrystal_Event;

        #endregion

        #region UNITY EVENTS

        void Start()
        {
            StartCoroutine(Setup());
        }

        #endregion

        #region SETUP

        private IEnumerator Setup()
        {
            PlayGetIn();
            animator.SetBool(PlayerAnimTag.GetIn.ToString(), true);
            destructable = true;
            yield return new WaitForSeconds(GameplayValues.CRYSTAL_DESTROY_TIME);
        }

        public void DestroyCapacitor()
        {
            if (destructable)
            {
                destructable = false;
                PlayGetOut();
                animator.SetBool(PlayerAnimTag.GetOut.ToString(), true);
                StartCoroutine(DestroyCapacitorEnd());

                DestroyCrystal_Event?.Invoke();
            }
        }

        private IEnumerator DestroyCapacitorEnd()
        {
            yield return new WaitForSeconds(GameplayValues.CRYSTAL_DESTROY_TIME);
            Destroy(gameObject);
        }

        #endregion

        #region SOUND CONTROL

        private void PlayAudio(AudioClip _clip)
        {
            if (audioSource != null && _clip != null)
            {
                audioSource.clip = _clip;
                audioSource.loop = false;
                audioSource.Play();
            }
        }

        private void PlayGetIn() => PlayAudio(getInSound);

        private void PlayGetOut() => PlayAudio(getoutSound);

        #endregion
    }
}