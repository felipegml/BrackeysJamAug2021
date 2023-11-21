using Persona.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.Components
{
    public class CameraShake : MonoBehaviour
    {
        #region VARIABLES

        [Header("Setup")]
        public Animator animator;

        //Private
        private bool shaking = false;

        #endregion

        #region UNITY EVENTS

        void Start()
        {
            //Add Events
            CrystalCapacitor.DestroyCrystal_Event += Shake_Event;
        }

        void OnDestroy()
        {
            //Remove Events
            CrystalCapacitor.DestroyCrystal_Event -= Shake_Event;
        }

        #endregion

        #region EVENTS

        private void Shake_Event(object[] obj = null)
        {
            StartCoroutine(Shake());
        }

        #endregion

        private IEnumerator Shake()
        {
            if(!shaking)
            {
                shaking = true;
                animator.SetBool("Shake1", true);
                yield return new WaitForSeconds(1f);
                animator.SetBool("Shake1", false);
                shaking = false;
            }
        }
    }
}