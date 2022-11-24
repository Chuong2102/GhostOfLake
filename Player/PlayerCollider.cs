using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    [SerializeField]
    Player.MainPlayerController player;
    Vector3 killerSoulPos;
    bool isCombat = false;

    List<Enemy> enemies = new List<Enemy>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach(var enemy in collision.gameObject.GetComponentsInParent<Enemy>())
        {
            if (enemy.GetIsAttackedByPlayer() && player.getIsTakeDamageToEnenmy())
            {
                player.GetListEnemiesToAttackAgain(enemy);

                // Dead Point
                if (collision.gameObject.name == "deadPoint")
                {
                    if (collision.gameObject.activeSelf)
                    {
                        enemy.TakeDamage(Player.MainPlayerController.Instance.getDeadPointDamage());
                        collision.gameObject.SetActive(false);
                        enemy.setDeadPointCounter(0);
                    }
                }
                // Mana
                if (enemy.getKillerSoulPos() != null)
                    killerSoulPos = enemy.getKillerSoulPos().position;

                enemy.TakeDamage(Player.MainPlayerController.Instance.getDamage());



                //Instance soul
                if (!enemy.getIsParry())
                {
                    for (int i = 0; i <= 3; i++)
                        if (player.transform.position.x < enemy.transform.position.x)
                            Instantiate(enemy.getKillerSoulParticle(), killerSoulPos, Quaternion.Euler(new Vector3(-69.3f, -90f, 90f))).Play();
                        else
                            Instantiate(enemy.getKillerSoulParticle(), killerSoulPos, Quaternion.Euler(new Vector3(-112.5f, -90f, 90f))).Play();
                }

                //player.setIsTakeDamageToEnemy(false);
                // Stun enemy

                if(enemy.GetIsStun())
                    enemy.Stun();
            }
        }
        

        if (player.getIsTakeDamage())
        {
            // Destroy trap
            if (collision.gameObject.CompareTag("TrapWeapon"))
            {
                collision.GetComponent<Weapon>().DestroyTrap();
            }
        }

    }
}
