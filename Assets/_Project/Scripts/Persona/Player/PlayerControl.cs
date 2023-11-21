using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Common.CommonData;

namespace Persona.Player
{
    public class PlayerControl : MonoBehaviour
    {
        #region VARIABLES

        [Header("Setup")]
        public SpriteRenderer avatar;
        public PlayerWeaponControl weapon;
        public PlayerBullet attackPrefab;

        [Header("Components")]
        public Rigidbody2D playerRb;
        public Animator playerAnimator;

        [Header("Data")]
        public PlayerState playerState = PlayerState.Static;

        [Header("PowerUP")]
        public GameObject shieldUP;

        [Header("Sound")]
        public AudioSource audioSource;
        public AudioClip teleportSound;
        public AudioClip dieSound;
        public AudioSource stepAudioSource;
        public AudioSource powerUpAudioSource;

        //Private - Move
        private bool mirrored = false;
        private Vector2 movement = Vector2.zero;
        private Vector2 mousePos = Vector2.zero;

        //Private - Attack
        private Transform attackContainer;
        private bool superAttack = false;
        private bool superDeflect = false;

        //Events
        public static event CustomEvent MoveUpdate_Event;
        public static event CustomEvent Died_Event;
        public static event CustomEvent PowerUp_Event;

        #endregion

        #region UNITY EVENTS

        private void Awake()
        {
            //Hide avatar
            ShowHideAvatarPlayer(false);
        }

        void Update()
        {
            GetInput();
        }

        void FixedUpdate()
        {
            MovePlayer();
            UpdateWeaponMovement();
        }

        //Collider
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(GameplayTags.EnemyAttack.ToString()))
            {
                if (collision.gameObject.name == GameplayTags.EnemyAttack.ToString() && 
                    playerState == PlayerState.Control)
                    Die();
            }

            if (collision.gameObject.CompareTag(GameplayTags.PowerUp.ToString()))
            {
                powerUpAudioSource.Play();
                SetupPowerUp(collision.gameObject.name);
                Destroy(collision.gameObject);
            }
        }

        #endregion

        #region EVENTS

        //Input
        public void GetInput()
        {
            if(playerState != PlayerState.Static)
            {
                //Move
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");

                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                //Action
                if (Input.GetMouseButtonDown(0))
                    BatAttack();
            }
        }

        #endregion

        #region SETUP

        public void Setup(Transform _attackContainer, bool _revive = false)
        {
            attackContainer = _attackContainer;

            TeleportIn(_revive);
        }

        public void UpdatePlayerState(PlayerState _state)
        {
            playerState = _state;
        }

        public void ShowHideAvatarPlayer(bool _show = true)
        {
            avatar.GetComponent<SpriteRenderer>().color = _show ? Color.white : Color.clear;
        }

        #endregion

        #region MOVE CONTROL

        //Player basic movement
        public void MovePlayer()
        {
            if (movement.x != 0 || movement.y != 0)
                stepAudioSource.gameObject.SetActive(true);
            else
                stepAudioSource.gameObject.SetActive(false);

            playerRb.MovePosition(playerRb.position + movement * GameplayValues.PLAYER_SPEED * Time.fixedDeltaTime);

            if(weapon.transform.rotation.eulerAngles.z > 90 && 
               weapon.transform.rotation.eulerAngles.z < 270)
                mirrored = true;
            else
                mirrored = false;

            FlipPlayer();
            UpdateWalkAnimation();

            MoveUpdate_Event?.Invoke(new object[] { transform.position });
        }

        private void FlipPlayer()
        {
            if (!mirrored)
                avatar.transform.localScale = new Vector3(1, 1, 1);
            else
                avatar.transform.localScale = new Vector3(-1, 1, 1);
        }

        //Aim
        public void UpdateWeaponMovement()
        {
            Vector2 _lookDir = mousePos - new Vector2(weapon.transform.position.x, weapon.transform.position.y);
            float _angle = Mathf.Atan2(_lookDir.x, _lookDir.y) * Mathf.Rad2Deg - 90f;
            weapon.transform.rotation = Quaternion.Euler(0, 0, -_angle);
        }

        #endregion

        #region ATTACK CONTROL

        //Player bat control
        public void BatAttack()
        {
            weapon.Attack(BatAttackComplete);
        }

        public void BatAttackComplete(Transform _aim)
        {
            CreateAttackEffect(_aim);
        }

        private void CreateAttackEffect(Transform _aim)
        {
            PlayerBullet _bullet = Instantiate<PlayerBullet>(attackPrefab, _aim.position, _aim.rotation, attackContainer);
            _bullet.Shoot(_aim, superAttack, superDeflect);
        }

        #endregion

        #region ACTIONS CONTROL

        public void Die()
        {
            StopAllCoroutines();
            UpdatePlayerState(PlayerState.Static);
            movement = Vector2.zero;

            PlayDie();
            playerAnimator.SetBool(PlayerAnimTag.Dead.ToString(), true);
            StartCoroutine(EraseBody());

            Died_Event?.Invoke();
        }

        public IEnumerator EraseBody()
        {
            yield return new WaitForSeconds(GameplayValues.PLAYER_TIME_ERASED);
            ShowHideAvatarPlayer(false);
            playerAnimator.SetBool(PlayerAnimTag.Dead.ToString(), false);
            yield return new WaitForEndOfFrame();
            playerAnimator.SetBool(PlayerAnimTag.GetIn.ToString(), true);
        }

        public IEnumerator NextLevel()
        {
            yield return new WaitForEndOfFrame();
            TeleportOut();
        }

        #endregion

        #region ANIMATION CONTROL

        private void UpdateWalkAnimation()
        {
            if (movement.x != 0 || movement.y != 0)
                playerAnimator.SetBool(PlayerAnimTag.Walk.ToString(), true);
            else
                playerAnimator.SetBool(PlayerAnimTag.Walk.ToString(), false);
        }

        private void TeleportIn(bool _revive)
        {
            ShowHideAvatarPlayer();
            PlayTeleport();
            playerAnimator.SetBool(PlayerAnimTag.GetIn.ToString(), false);
            StartCoroutine(TeleportIn_End(_revive));
        }

        private IEnumerator TeleportIn_End(bool _revive)
        {
            yield return new WaitForSeconds(GameplayValues.PLAYER_UNLOCK_TIME);
            
            if(!_revive)
                UpdatePlayerState(PlayerState.Control);
            else
                StartCoroutine(GodMode());
        }

        private void TeleportOut()
        {
            movement = Vector2.zero;
            UpdatePlayerState(PlayerState.Static);
            playerAnimator.SetBool(PlayerAnimTag.GetOut.ToString(), true);
        }

        #endregion

        #region SOUND CONTROL

        private void PlayAudio(AudioClip _clip)
        {
            if(audioSource != null && _clip != null)
            {
                audioSource.clip = _clip;
                audioSource.loop = false;
                audioSource.Play();
            }
        }

        private void PlayTeleport() => PlayAudio(teleportSound);

        private void PlayDie() => PlayAudio(dieSound);

        #endregion

        #region POWER UP CONTROL

        private void SetupPowerUp(string _name)
        {
            if (_name == PowerUp.ShieldPowerUP.ToString())
                StartCoroutine(GodMode());
            else if (_name == PowerUp.LiveUP.ToString())
                PowerUp_Event?.Invoke(new object[] { PowerUp.LiveUP });
            else if (_name == PowerUp.SwordUP.ToString())
                StartCoroutine(SuperSword());
            else if (_name == PowerUp.BombUP.ToString())
                PowerUp_Event?.Invoke(new object[] { PowerUp.BombUP });
            else if (_name == PowerUp.DeflectUP.ToString())
                StartCoroutine(SuperDeflect());
        }

        public IEnumerator GodMode()
        {
            UpdatePlayerState(PlayerState.NoDamage);
            shieldUP.SetActive(true);
            yield return new WaitForSeconds(GameplayValues.PLAYER_GODMODE_TIME);
            shieldUP.SetActive(false);
            UpdatePlayerState(PlayerState.Control);
        }

        public IEnumerator SuperSword()
        {
            superAttack = true;
            weapon.SuperSword(true);
            yield return new WaitForSeconds(GameplayValues.PLAYER_SWORDUP_TIME);
            superAttack = false;
            weapon.SuperSword(false);
        }

        public IEnumerator SuperDeflect()
        {
            superDeflect = true;
            weapon.SuperDeflect(true);
            yield return new WaitForSeconds(GameplayValues.PLAYER_SWORDUP_TIME);
            weapon.SuperDeflect(false);
            superDeflect = false;
        }

        #endregion
    }
}