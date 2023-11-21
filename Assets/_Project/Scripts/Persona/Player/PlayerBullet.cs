using Persona.Enemy;
using Persona.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Common.CommonData;

namespace Persona.Player
{
    public class PlayerBullet : MonoBehaviour
    {
        #region VARIABLES

        [Header("Setup")]
        public SpriteRenderer sprite;
        public Rigidbody2D rb;
        public Color superColor;

        [Header("Sound")]
        public AudioSource audioSource;
        public AudioClip ricochetSound;

        //Private
        private bool super = false;

        //Events

        #endregion

        #region UNITY EVENTS

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(GameplayTags.Crystal.ToString()))
            {
                collision.gameObject.GetComponent<CrystalCapacitor>().DestroyCapacitor();
                DestroyNow();
            }

            if (collision.gameObject.CompareTag(GameplayTags.EnemyAttack.ToString()) &&
                super)
            {
                collision.gameObject.GetComponent<EnemyBullet>().SelfDestroy();
            }

            if (collision.gameObject.CompareTag(GameplayTags.Enemy.ToString()))
            {

                if(super)
                    collision.gameObject.GetComponent<EnemyControl>().Die();

                DestroyNow();
            }
        }

        #endregion

        #region FUNCTIONS

        public void Shoot(Transform _aim, bool _super = false, bool _superDeflect = false)
        {
            //print("Shoot");
            super = _super;
            sprite.color = _super ? superColor : Color.white;

            if (_superDeflect)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            rb.velocity = _aim.right * GameplayValues.PLAYER_ATTACK_SPEED;

            StartCoroutine(SelfDestroy());
        }

        private IEnumerator SelfDestroy()
        {
            yield return new WaitForSeconds(GameplayValues.PLAYER_ATTACK_LIFETIME);
            Destroy(gameObject);
        }

        private void DestroyNow()
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }

        #endregion

        #region SOUNd CONTROL

        private void PlayAudio(AudioClip _clip)
        {
            if (audioSource != null && _clip != null)
            {
                audioSource.clip = _clip;
                audioSource.loop = false;
                audioSource.Play();
            }
        }

        #endregion
    }
}