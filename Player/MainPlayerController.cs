using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Player
{
    public class MainPlayerController : MonoBehaviour, DamagedAction
    {
        #region Property
        public static MainPlayerController Instance { get; private set; }

        [SerializeField]
        PlayerDataSTO playerData;

        Animator anim;
        Rigidbody2D rb;
        [SerializeField]
        float jumpForce;
        public float speed;
        public float pariForce;
        float move;
        [SerializeField]
        float horizontalForce;
        [SerializeField]
        float damage;
        [SerializeField]
        float deadPointDamage;

        bool isDash = true;
        string GROUND_TAG = "Ground";
        bool isGround = true;

        public float pariRange;

        public Transform pariPoint;
        public LayerMask enemyLayer;

        public bool right = true;
        bool rightBefore = true;
        bool isFlipped = false;
        public bool isParied = false;
        bool isMove = true;
        bool isAddMana = true;

        public float MaxHealth = 1;
        public float MaxMana = 0f;

        public GameObject healthBar;
        public GameObject manaBar;

        public bool isAttack;
        bool isTakeDamage;
        public int attackCount = 0;
        int takeDamage;
        bool isCombo;
        bool isCancelAttack;
        bool isTransition;

        bool isPari;
        bool isTakeParry;

        bool isAttackedByEnemy;

        Vector3 healthBefore;
        Vector3 manaAfter;

        public bool isHold = false;

        public Transform camStartFollow;

        [SerializeField]
        Transform startCutscenePoint;
        float distance;
        [SerializeField]
        PlayableDirector pd;
        //
        // DASH
        float dashTime;
        [SerializeField]
        float startDashTime;
        int direction;
        [SerializeField]
        float dashSpeed;
        // STUN
        bool isStuning = true;
        bool isStuned = true;
        //
        bool isTakeDamageByEnemies = true;
        // ATTACK
        [SerializeField]
        int countOfCombo;
        bool isTakeDamageToEnemy;
        [SerializeField]
        float normalDamage;
        List<Enemy> enemies = new List<Enemy>();
        List<Weapon> listTrap = new List<Weapon>();

        // TAG
        string ATTACK_TAG = "attack";
        string JUMP_TAG = "jump";
        string RUN_TAG = "run";
        string PARI_TAG = "parry";
        string HOLDPARI_TAG = "holdpari";
        string ATTACK_0 = "attack0";
        string ATTACK_1 = "attack1";
        string ATTACK_3 = "attack3";
        string DASH_TAG = "dash";
        string ENEMY_TAG = "Enemy";
        //
        // Animation VFX
        [SerializeField]
        Animator slash;
        //
        // Attack by Enemies
        [SerializeField]
        float manaAttacked;
        [SerializeField]
        float stunForce;
        [SerializeField]
        float verticleStunForce;
        //
        // Collision
        
        // Stun
        [SerializeField]
        float timeStun;
        // Die
        bool isDie;
        // heavy Attack
        bool isHeavyAttack;
        [SerializeField]
        float heavyAttMana;
        //
        // Sound Effect
        [SerializeField]
        AudioSource attackSoundEft;
        [SerializeField]
        AudioSource stunSoundEft;
        //
        [SerializeField]
        float endOfComboWaitTime;
        [SerializeField]
        float endEachSlashTime;
        [SerializeField]
        float normalAttackMana;
        [SerializeField]
        float manaBonus;
        [SerializeField]
        float dashMana;
        [SerializeField]
        float parryMana;

        bool isStunParry;

        public Coroutine normalAttackDelay;
        bool isDoubleJump = true;

        Vector3 respawnPlayerPos;

        Vector2 respawnScene0;
        Vector2 respawnScene1;
        Vector2 respawnScene2;
        //
        // Camera
        [SerializeField]
        Cinemachine.CinemachineVirtualCamera virtualCam;
        [SerializeField]
        float cameraSpeed;
        bool isGetCamPos = true;
        float oldCamY;
        [SerializeField]
        float maxCamY;

        #endregion

        #region Command Class
         
        // --------------------- COMMAND CLASS -----------------------------
        public abstract class Command 
        {
            public abstract void Execute(MainPlayerController player);
        }

        // Kế thừa class Command và ghi đè phương thức Execute theo mong muốn.
        public class JumpCommand : Command
        {
            public override void Execute(MainPlayerController player)
            {
                if (player.getIsMove())
                    player.Jump();
            }
        }

        public class HeavyAttackCommand : Command
        {
            public override void Execute(MainPlayerController player)
            {
                if (player.manaBar.transform.localScale.x >= player.heavyAttMana)
                    player.HeavyAttack();
            }
        }

        public class MoveCommand : Command
        {
            public override void Execute(MainPlayerController player)
            {
                //if (player.getIsMove())
                //    player.Move();
            }
        }

        public class AttackCommand : Command
        {
            public override void Execute(MainPlayerController player)
            {
                //if(player.manaBar.transform.localScale.x >= player.normalAttackMana)
                //    player.Attack();
            }
        }

        public class PariCommand : Command
        {
            public override void Execute(MainPlayerController player)
            {
                if (player.manaBar.transform.localScale.x >= 0.3f && player.isStunParry)
                    player.Parry();
            }
        }

        public class CancelPariCommand : Command
        {
            public override void Execute(MainPlayerController player)
            {
                player.cancelPari();
            }
        }

        public class DashCommand: Command
        {
            public override void Execute(MainPlayerController player)
            {
                if (player.manaBar.transform.localScale.x >= player.dashMana)
                    player.Dash();
            }
        }

        public class MoveCamera: Command
        {
            public override void Execute(MainPlayerController player)
            {
                player.MoveVirtualCam();
            }
        }

        // Kế thừa class Command và ghi đè phương thức Execute để không có hành động nào được thực hiện.
        public class DoNothing : Command
        {
            public override void Execute(MainPlayerController player)
            {
            }
        }

        #endregion

        // ------------------------ START/UPDATE -----------------------------

        private void Awake()
        {
            Instance = this;
            //
            // Dont destroy on load
            //foreach (var o in GameObject.FindGameObjectsWithTag("MainCamera"))
            //{
            //    DontDestroyOnLoad(o);
            //}
            //foreach (var o in GameObject.FindGameObjectsWithTag("Player"))
            //{
            //    DontDestroyOnLoad(o);
            //}


            Physics2D.IgnoreLayerCollision(7, 6, false);
            Physics2D.IgnoreLayerCollision(7, 9, false);
            rb = gameObject.GetComponent<Rigidbody2D>();
            anim = gameObject.GetComponent<Animator>();
            //
            // Update stat
            jumpForce = playerData.jumpForce;
            speed = playerData.speed;
            pariForce = playerData.pariForce;
            damage = playerData.damage;
            enemyLayer = playerData.enemyLayer;
            MaxHealth = playerData.MaxHealth;
            MaxMana = playerData.MaxMana;
            dashSpeed = playerData.dashSpeed;
            startDashTime = playerData.startDashTime;
            dashTime = playerData.dashTime;
            countOfCombo = playerData.countOfCombo;
            manaAttacked = playerData.manaAttacked;
            stunForce = playerData.stunForce;
            verticleStunForce = playerData.verticleStunForce;
            timeStun = playerData.timeStun;
            pariRange = playerData.pariRange;
            //
            //
            isDie = false;
            isHeavyAttack = true;
            isTakeParry = true;
            isPari = false;
            isStunParry = true;

            //respawnPlayerPos = transform.position;
            //LoadPlayer();

            respawnScene0 = new Vector2(-72.8f, -8.5f);
            respawnScene1 = new Vector2(-28.09f, 10.561f);


            playerController = new PlayerController();

        }
        

        // Start is called before the first frame update
        void Start()
        {
            isAttack = true;
            isTakeDamage = false;
            takeDamage = 0;
            isAttackedByEnemy = false;
            isCombo = false;
            isCancelAttack = false;
            isTransition = false;
        }


        #region Get Set Method

        // ----------------------- GET SET METHOD -----------------------------


        public bool isRight()
        {
            return right;
        }


        public bool getIsGround()
        {
            return isGround;
        }

        public bool getIsAttack()
        {
            return isAttack;
        }

        public bool getIsTakeDamageToEnenmy()
        {
            return isTakeDamageToEnemy;
        }

        public void setIsTakeDamageToEnemy(bool _isTakeDamage)
        {
            isTakeDamageToEnemy = _isTakeDamage;
        }

        public float getDamage()
        {
            return damage;
        }

        public bool getIsTakeParry()
        {
            return isTakeParry;
        }

        public void setIsTakeParry(bool _is)
        {
            isTakeParry = _is;
        }


        public void SetIsTransition(bool _is)
        {
            isTransition = _is;
        }

        public bool GetIsTransition()
        {
            return isTransition;
        }

        public bool GetCancelAttack()
        {
            return isCancelAttack;
        }

        public void SetCancelAttack(bool _is)
        {
            isCancelAttack = _is;
        }
        
        public void SetAttackCount(int _a)
        {
            attackCount = _a;
        }

        public bool GetIsCombo()
        {
            return isCombo;
        }

        public void SetIsCombo(bool _is)
        {
            isCombo = _is;
        }

        public bool GetIsAttackedByEnemy()
        {
            return isAttackedByEnemy;
        }

        public void SetIsAttackedByEnemy(bool _is)
        {
            isAttackedByEnemy = _is;
        }

        public bool GetIsDie()
        {
            return isDie;
        }
        
        public void setTakeDamageCount(int n)
        {
            this.takeDamage = n;
        }

        public bool getIsTakeDamage()
        {
            return this.isTakeDamage;
        }

        public void setIsTakeDamage(bool _is)
        {
            this.isTakeDamage = _is;
        }

        public Vector3 getRespawnPos()
        {
            return respawnPlayerPos;
        }

        public void setRespawnPos(Vector3 pos)
        {
            respawnPlayerPos = pos;
        }

        public void setStunParry(bool _is)
        {
            isStunParry = _is;
        }

        public float getNormalDamage()
        {
            return normalDamage;
        }

        public bool getIsHeavyAttack()
        {
            return isHeavyAttack;
        }

        public void setDamage(float _d)
        {
            damage = _d;
        }

        public void setIsHeavyAttack(bool _is)
        {
            isHeavyAttack = _is;
        }

        public float getDeadPointDamage()
        {
            return this.deadPointDamage;
        }

        public void setIsMove(bool _isMove)
        {
            isMove = _isMove;
        }

        public bool getIsMove()
        {
            return isMove;
        }

        public bool getIsPari()
        {
            return isPari;
        }

        public void setIsPari(bool pari)
        {
            isPari = pari;
        }

        public float getManaAttacked()
        {
            return this.manaAttacked;
        }

        public float getStunForce()
        {
            return this.stunForce;
        }

        public void setIsAttack(bool _isAtt)
        {
            isAttack = _isAtt;
        }

        #endregion


        #region METHOD
        public void TakeDamage(float damage)
        {
            if (!isDie)
            {
                // Decrease health
                MaxHealth -= damage;
                healthBefore = healthBar.transform.localScale;

                healthBar.transform.localScale = new Vector3(MaxHealth, healthBefore.y, healthBefore.z);
                //
                // Decrease mana
                //var tempMana = manaBar.transform.localScale;
                //manaBar.transform.localScale = new Vector3(tempMana.x - manaAttacked > 0f ? Mathf.Abs(tempMana.x - manaAttacked) : 0f,
                //    1f, 1f);

                if (MaxHealth <= 0f)
                    Die();
            }
        }
        


        public void ResetAttack()
        {
            attackCount = 0;
            isAttack = true;
            isCombo = false;
        }


        public void MoveVirtualCam()
        {
            if (isGetCamPos)
            {
                oldCamY = virtualCam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_ScreenY;
                isGetCamPos = false;
            }

            if(virtualCam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_ScreenY >= maxCamY)
                virtualCam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_ScreenY -= Time.deltaTime * cameraSpeed;
        }

        public void MoveVirtualCamBack()
        {
            virtualCam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_ScreenY = oldCamY;
            isGetCamPos = true;
        }

        public void Stun(float direction, float horizontalForce)
        {
            if (isStuning)
            {
                stunSoundEft.Play();
                isDash = false;
                isMove = false;
                isAttack = false;
                //rb.velocity = Vector2.right * direction * horizontalForce;
                Physics2D.IgnoreLayerCollision(7, 6, true);
                //
                //rb.velocity = Vector2.right * direction * horizontalForce;
                //transform.position += new Vector3(direction, 0f, 0f) * Time.deltaTime * horizontalForce;
                rb.velocity = Vector2.right * 0f;

                anim.SetTrigger("stun");

                rb.AddForce(new Vector2(horizontalForce * direction, verticleStunForce), ForceMode2D.Impulse);
                StartCoroutine(Stuning());
            }
            
        }

        public void StunParry(float direction, float horizontalForce)
        {
            if (isStunParry)
            {
                isStunParry = false;
                stunSoundEft.Play();
                
                //rb.velocity = Vector2.right * direction * horizontalForce;
                //Physics2D.IgnoreLayerCollision(7, 6, true);
                //
                //rb.velocity = Vector2.right * direction * horizontalForce;
                //transform.position += new Vector3(direction, 0f, 0f) * Time.deltaTime * horizontalForce;
                rb.velocity = Vector2.right * 0f;

                //anim.SetTrigger("stun");

                rb.AddForce(new Vector2((horizontalForce * direction)/2, verticleStunForce), ForceMode2D.Impulse);
                //StartCoroutine(Stuning());
            }
        }

        public IEnumerator Stuning()
        {
            //isTakeDamageByEnemies = false;
            //
            //float temp = damage;
            //
            yield return new WaitForSeconds(timeStun);
            Physics2D.IgnoreLayerCollision(7, 6, false);
            isStuning = true;
            isDash = true;
            //isTakeDamageByEnemies = true;
            StopCoroutine(Stuning());
            //
            isStuned = true;
            rb.velocity = Vector2.right * 0f;
            isMove = true;
            isAttack = true;
        }

        Vector2 movement;
        PlayerController playerController;
        Vector2 smoothInputVelocity;
        [SerializeField]
        float smoothSpeed = 0.2f;
        Vector2 curMove;

        private void OnEnable()
        {
            playerController.Player.Move.Enable();
        }

        private void OnDisable()
        {
            playerController.Player.Move.Disable();
        }

        private void FixedUpdate()
        {
            Move();
        }

        public virtual void Move()
        {
            if (getIsMove())
            {
                movement = playerController.Player.Move.ReadValue<Vector2>();
                curMove = Vector2.SmoothDamp(curMove, movement, ref smoothInputVelocity, smoothSpeed);
                
                if (curMove.x >= -1f && curMove.x <= 1f)
                {
                    //transform.position += new Vector3(move, 0f, 0f) * Time.deltaTime * speed;
                    float acceleration = 3f;
                    float deceleration = 2f;
                    float velPower = 2f;

                    var targetSpeed = curMove.x * speed;
                    var velocity = rb.velocity.x;

                    if (velocity > 8f)
                        velocity = 8f;

                    if (velocity < -8f)
                        velocity = -8f;

                    var speedDif = targetSpeed - velocity;
                    var accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
                    var movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

                    rb.AddForce(Vector2.right * movement);

                }


                if (curMove.x > 0)
                {
                    right = true;
                    //anim.SetBool(RUN_TAG, true);
                }
                if (curMove.x < 0)
                {
                    right = false;
                    //anim.SetBool(RUN_TAG, true);
                }
                if (curMove.x == 0)
                {
                    //anim.SetBool(RUN_TAG, false);
                }
                if (rightBefore != right)
                {
                    isFlipped = true;
                    rightBefore = right;
                }

                if (isFlipped)
                {
                    transform.Rotate(0f, 180f, 0f);
                    //attackPoint.transform.Rotate(0f, 180f, 0f);
                    //pariPoint.transform.Rotate(0f, 180f, 0f);
                    isFlipped = false;
                }
            }
        }

        
        public virtual void Parry()
        {
            if (isTakeParry && manaBar.transform.localScale.x >= parryMana && isStunParry)
            {
                isTakeParry = false;
                DescreaseMana(parryMana);
                anim.SetTrigger(PARI_TAG);

            }
            
        }

        void TakePari()
        {
            if (isPari)
            {
                var hitEnemies = Physics2D.OverlapCircleAll(pariPoint.position, pariRange, enemyLayer);
                if (hitEnemies.Length != 0)
                {
                    isParied = true;
                }
                else
                    isParied = false;

            }
        }

        public virtual void cancelPari()
        {
            anim.SetBool(PARI_TAG, false);
        }

        public virtual void Dash()
        {
            if (isDash && manaBar.transform.localScale.x >= dashMana)
            {
                isDash = false;
                DescreaseMana(dashMana);
                // Dash
                anim.SetTrigger(DASH_TAG);

            }
                
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(GROUND_TAG))
            {
                isGround = true;
            }

            if (collision.gameObject.CompareTag("CheckPoint"))
            {
                respawnPlayerPos = transform.position;
            }
            
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other.gameObject.CompareTag("Mana"))
            {
                if (isAddMana)
                {
                    if (MaxMana < 1f)
                    {
                        MaxMana += manaBonus;
                        isAddMana = false;
                        Destroy(other);
                    }
                }

                isAddMana = true;
                manaAfter = manaBar.transform.localScale;
                manaBar.transform.localScale = new Vector3(MaxMana, manaAfter.y, manaAfter.z);
            }
            
            
        }

        //void EnemyAttack()
        //{
        //    if (!isParied)
        //    {
        //        var hitEnemies = Physics2D.OverlapCircleAll(deadPoint.position, deadRange, enemyLayer);

        //        if(hitEnemies.Length != 0)
        //        {
        //            foreach (var enemy in hitEnemies)
        //            {
        //                TakeDamage(0.4f);
        //                Debug.Log("Attacked by " + enemy.name);
        //            }
        //            if (right)
        //                rb.AddForce(new Vector2(-pariForce, 0f), ForceMode2D.Impulse);
        //            else
        //                rb.AddForce(new Vector2(pariForce, 0f), ForceMode2D.Impulse);
        //        }
        //    }
        //    else
        //    {
        //        var hitEnemies = Physics2D.OverlapCircleAll(deadPoint.position, deadRange, enemyLayer);

        //        if (hitEnemies.Length != 0)
        //        {
        //            foreach (var enemy in hitEnemies)
        //            {
        //                TakeDamage(0.1f);
        //                Debug.Log("Attacked by " + enemy.name);
        //            }
        //            if (right)
        //                rb.AddForce(new Vector2(-pariForce, 0f), ForceMode2D.Impulse);
        //            else
        //                rb.AddForce(new Vector2(pariForce, 0f), ForceMode2D.Impulse);
        //        }
        //    }
        //}

        

        public virtual void Die()
        {
            Physics2D.IgnoreLayerCollision(7, 6, true);
            isStuning = false;
            isAttack = false;
            isMove = false;
            isDash = false;
            isDie = true;
            //
            //anim.SetBool("die", true);

            anim.SetTrigger("dieTrigger");
        }

        public void DestroyPlayer()
        {
            Destroy(gameObject);
            SavePlayer();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }

        public virtual void Attack()
        {
            if (isAttack && manaBar.transform.localScale.x >= normalAttackMana && !isCombo)
            {

                if (isCancelAttack && attackCount >= 1)
                    attackCount = 0;
                
                //Debug.Log("Attack + " + attackCount);
                

                if(attackCount != 0)
                    isCombo = true;

                // Truong hop Jump/Dash/Parry/... ngay khi TransitionAnimAttack dang chay


                isTakeDamage = true;
                isTakeDamageToEnemy = true;
                attackSoundEft.Play();
                isAttack = false;
                // Danh lan dau
                if (attackCount == 0)
                {
                    attackCount++;
                    anim.SetTrigger(ATTACK_0);
                    slash.SetTrigger("attack");
                }

                

                // Decreas mana
                DescreaseMana(normalAttackMana);
            }
        }

        public void DescreaseMana(float decreaseMana)
        {
            var tempSoulBar = manaBar.transform.localScale;
            manaBar.transform.localScale = new Vector3(tempSoulBar.x - decreaseMana >= 0f ? tempSoulBar.x - decreaseMana : 0f,
                tempSoulBar.y, tempSoulBar.z);
            MaxMana = tempSoulBar.x - decreaseMana >= 0f ? tempSoulBar.x - decreaseMana : 0f;
        }

        public virtual void HeavyAttack()
        {
            if (isAttack && isHeavyAttack && manaBar.transform.localScale.x >= heavyAttMana)
            {
                // Decreas mana
                DescreaseMana(heavyAttMana);
                
                //

                isHeavyAttack = false;
                isTakeDamageToEnemy = true;
                //
                setDamage(damage * 2f);
                //
                anim.SetTrigger("heavyAttack");

                isAttack = false;

            }
        }

        public IEnumerator InscreaseMana()
        {
            yield return new WaitForSeconds(2f);
            //
            // Inscrease
            var attackNum = 1f / normalAttackMana;

            var tempSoulBar = manaBar.transform.localScale;
            float du = tempSoulBar.x - normalAttackMana * attackNum >= 0f ? tempSoulBar.x - normalAttackMana * attackNum : 0f;
            while(tempSoulBar.x <= du + normalAttackMana * attackNum)
            {
                tempSoulBar.x += 0.01f;
                manaBar.transform.localScale = new Vector3(tempSoulBar.x, tempSoulBar.y, tempSoulBar.z);
            }
            MaxMana = du + normalAttackMana * attackNum;
                
            // 
        }

        public virtual void Jump()
        {
            if (isGround && getIsMove())
            {
            
                if(!isTransition)
                    anim.SetTrigger(JUMP_TAG);

                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

                isGround = false;
                isDoubleJump = true;
            }
            //else if(!isGround && isDoubleJump)
            //{
            //    isDoubleJump = false;
            //    anim.SetTrigger(JUMP_TAG);
            //    rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            //}
        }

        // Ti sua lai 
        public IEnumerator DelayAttack()
        {
            isAttack = false;
            //
            //if (attackCount > countOfCombo)
            //{
            //    yield return new WaitForSeconds(endOfComboWaitTime);
            //    attackCount = 0;
            //}
            //else
            //{
            //    yield return new WaitForSeconds(endEachSlashTime);
            //}

            yield return new WaitForSeconds(endEachSlashTime);

            isAttack = true;
            StopCoroutine(DelayAttack());
        }

        public IEnumerator DelayHeavyAtack()
        {
            yield return new WaitForSeconds(2f);

            isHeavyAttack = true;
            StopCoroutine(DelayHeavyAtack());
        }

        public IEnumerator DelayDash()
        {
            isDash = false;

            yield return new WaitForSeconds(0.12f);

            isDash = true;
            StopCoroutine(DelayDash());
        }

        public IEnumerator DelayStun()
        {
            //float temp = damage;
            //damage = 0f;
            yield return new WaitForSeconds(2f);
            //damage = temp;
            StopCoroutine(DelayStun());
        }

        private void OnDrawGizmosSelected()
        {
            if (pariPoint == null)
                return;
            Gizmos.DrawWireSphere(pariPoint.position, pariRange);
        }

        public void SavePlayer()
        {
            SaveSystem.SavePlayer(this);
        }

        public void LoadPlayer()
        {
            PlayerData data = SaveSystem.LoadPlayer();

            Vector3 pos;

            pos.x = data.position[0];
            pos.y = data.position[1];
            pos.z = data.position[2];

            transform.position = pos;
        }

        public void SwitchScene()
        {
            var index = SceneManager.GetActiveScene().buildIndex;
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                
                if(index != 1)
                SceneManager.LoadScene(1);
                Player.MainPlayerController.Instance.transform.position = respawnScene1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (index != 0)
                    SceneManager.LoadScene(0);
                Player.MainPlayerController.Instance.transform.position = respawnScene0;
            }
        }


        public void SetAttackAgian()
        {
            if(enemies != null)
            {
                foreach (var enemy in enemies)
                {
                    enemy.SetIsAttackByPlayer(true);
                }
                enemies.Clear();
            }
                
        }


        public void GetListEnemiesToAttackAgain(Enemy enemy)
        {
            this.enemies.Add(enemy);
        }
        

    }
    #endregion


}
