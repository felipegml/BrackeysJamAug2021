using DG.Tweening;
using Persona.Level;
using Persona.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Common.CommonData;

namespace Persona.Enemy
{
    public class EnemyControl : MonoBehaviour
    {
        #region VARIABLES

        [Header("Setup")]
        public Rigidbody2D rb;
        public CircleCollider2D collider;
        public Animator animator;
        public Transform mouseCircle;
        public Transform aimCircle;
        public EnemyBullet attackPrefab;
        public GameObject enemyShootEffect;

        [Header("Data")]
        public float enemySpeed = 5f;
        public float attackInterval = 2f;
        public int totalHP = 1;

        [Header("Sound")]
        public AudioSource audioSource;
        public AudioClip attackSound;

        //Private
        private bool paused = false;
        private float currentTime = 0;
        private Tween moveTween;

        //Private - Attack
        private Transform attackContainer;
        private EnemyPortal portal;

        //Private - Data
        private int currentPathIndex = 0;
        private int currentPoint = 0;
        private Vector2 playerPos = Vector2.zero;
        private bool attacking = false;

        //Events
        public static event CustomEvent Died_Event;

        #endregion

        #region UNITY EVENTS

        void Start()
        {
            //Add Events
            PlayerControl.MoveUpdate_Event += PlayerControl_MoveUpdate_Event;
        }

        void OnDestroy()
        {
            //Remove Events
            PlayerControl.MoveUpdate_Event -= PlayerControl_MoveUpdate_Event;
        }

        private void Update()
        {
            if (!paused)
            {
                if (currentTime < (attacking ? GameplayValues.ENEMY_PREATTACK_TIME : attackInterval))
                    currentTime += Time.deltaTime;
                else
                {
                    currentTime = 0;

                    if(!attacking)
                    {
                        attacking = true;
                        Attack();
                    }
                    else
                    {
                        CreateBullet();
                        attacking = false;
                    }
                }
            }
        }

        void FixedUpdate()
        {
            UpdateWeaponMovement();
        }

        //Collider
        private void OnCollisionEnter2D(Collision2D collision)
        {
            /*if (collision.gameObject.CompareTag(GameplayTags.EnemyAttack.ToString()))
            {
                if(collision.gameObject.name == GameplayTags.PlayerAttack.ToString())
                    TakeDamage();
            }*/
        }

        #endregion

        #region EVENTS

        private void PlayerControl_MoveUpdate_Event(object[] obj = null)
        {
            Vector3 _pos = (Vector3)obj[0];
            playerPos = new Vector2(_pos.x, _pos.y);
        }

        #endregion

        #region SETUP

        public void Setup(Transform _attackContainer, EnemyPortal _portal)
        {
            attackContainer = _attackContainer;
            portal = _portal;
            currentPathIndex = portal.GetPathIndex();

            Move();
        }

        public void Pause()
        {
            paused = true;
            moveTween.Pause();
        }

        public void Resume()
        {
            paused = false;
            moveTween.Play();
        }

        #endregion

        #region MOVE CONTROL

        public void Move()
        {
            float _time = portal.GetPathTime(currentPathIndex, currentPoint, enemySpeed);
            currentPoint = portal.GetPathNextIndex(currentPathIndex, currentPoint);

            Vector2 _pos = portal.GetNextPoint(currentPathIndex, currentPoint);

            moveTween = transform.DOMove(_pos, _time)
                                 .SetEase(Ease.Linear)
                                 .OnComplete(() =>
                                 {
                                     if (currentPoint != 0)
                                         Move();
                                     else
                                         Die();
                                 });
        }

        public void UpdateWeaponMovement()
        {
            Vector2 _lookDir = playerPos - new Vector2(mouseCircle.position.x, mouseCircle.position.y);
            float _angle = Mathf.Atan2(_lookDir.x, _lookDir.y) * Mathf.Rad2Deg + 90f;
            mouseCircle.rotation = Quaternion.Euler(0, 0, -_angle);
        }

        #endregion

        #region ATTACK CONTROL

        private void Attack()
        {
            enemyShootEffect.SetActive(true);
        }

        private void CreateBullet()
        {
            enemyShootEffect.SetActive(false);
            PlayAttack();

            EnemyBullet _bullet = Instantiate<EnemyBullet>(attackPrefab, aimCircle.position, aimCircle.rotation, attackContainer);
            _bullet.name = GameplayTags.EnemyAttack.ToString();
            _bullet.Shoot(aimCircle);
        }

        #endregion

        #region ACTIONS CONTROL

        public void TakeDamage()
        {
            totalHP--;

            if (totalHP <= 0)
                Die();
        }

        public void Die()
        {
            Died_Event?.Invoke();
            StartCoroutine(Die_End());
        }

        public IEnumerator Die_End()
        {
            collider.enabled = false;
            moveTween.Kill();
            animator.SetBool(PlayerAnimTag.Dead.ToString(), true);
            yield return new WaitForSeconds(GameplayValues.ENEMY_DIE_TIME);
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

        private void PlayAttack() => PlayAudio(attackSound);

        #endregion
    }
}