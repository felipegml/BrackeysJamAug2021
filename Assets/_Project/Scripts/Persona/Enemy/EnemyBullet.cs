using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Common.CommonData;

namespace Persona.Enemy
{
    public class EnemyBullet : MonoBehaviour
    {
        #region VARIABLES

        [Header("Setup")]
        public SpriteRenderer sprite;
        public Rigidbody2D rb;
        public CircleCollider2D collider;
        public Animator animator;

        [Header("Data")]
        public Color enemyColor;
        public Color playerColor;

        [Header("Sound")]
        public AudioSource audioSource;
        public AudioClip ricochetSound;

        //Private
        private bool paused = false;
        private Vector2 currentVelocity;
        private float currentTime = 0;

        //Private - Data
        private bool isEnemy = true;

        #endregion

        #region UNITY EVENTS

        private void Update()
        {
            if(!paused && GameplayValues.ENEMY_BULLET_LIFETIME > 0)
            {
                if (currentTime < GameplayValues.ENEMY_BULLET_LIFETIME)
                    currentTime += Time.deltaTime;
                else
                    SelfDestroy();
            }
        }

        void FixedUpdate()
        {
            rb.velocity = GameplayValues.ENEMY_ATTACK_SPEED * (rb.velocity.normalized);
        }

        //Collider
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(GameplayTags.EnemyAttack.ToString()))
            {
                if (name != collision.gameObject.name)
                    SelfDestroy();
            }
            
            if (collision.gameObject.CompareTag(GameplayTags.PlayerAttack.ToString()))
            {
                //isEnemy = false;
                //ChangeSideColor();
                PlayRicochet();
            }
            
            if (collision.gameObject.CompareTag(GameplayTags.Shredder.ToString()))
            {
                SelfDestroy();
            }
            
            if (collision.gameObject.CompareTag(GameplayTags.Player.ToString()))
            {
                SelfDestroy();
            }
        }

        #endregion

        #region SETUP

        public void Pause()
        {
            paused = true;
            currentVelocity = rb.velocity;
            rb.velocity = Vector2.zero;
        }

        public void Resume()
        {
            paused = false;
            rb.velocity = currentVelocity;
        }

        #endregion

        #region FUNCTIONS

        public void Shoot(Transform _aim)
        {
            //print("Shoot");
            rb.velocity = -_aim.right * GameplayValues.ENEMY_ATTACK_SPEED;
        }

        public void Reflect(Collision2D collision)
        {
            Vector2 _wallNormal = collision.contacts[0].normal;
            Vector2 m_dir = Vector2.Reflect(rb.velocity, _wallNormal);
            rb.velocity = m_dir * GameplayValues.ENEMY_ATTACK_SPEED;
        }

        public void ChangeSideColor()
        {
            sprite.color = isEnemy ? enemyColor : playerColor;
            name = isEnemy ? GameplayTags.EnemyAttack.ToString() : GameplayTags.PlayerAttack.ToString();
        }

        public void SelfDestroy()
        {
            collider.enabled = false;
            rb.velocity = Vector2.zero;

            StartCoroutine(DestoyEnd());
        }

        private IEnumerator DestoyEnd()
        {
            animator.SetBool(PlayerAnimTag.Dead.ToString(), true);
            yield return new WaitForSeconds(.5f);
            Destroy(gameObject);
        }

        #endregion

        #region SOUNd CONTROL

        private void PlayAudio(AudioClip _clip)
        {
            if (audioSource != null && _clip != null)
            {
                if(!audioSource.isPlaying)
                {
                    audioSource.clip = _clip;
                    audioSource.loop = false;
                    audioSource.Play();
                }
            }
        }

        private void PlayRicochet() => PlayAudio(ricochetSound);


        #endregion
    }
}