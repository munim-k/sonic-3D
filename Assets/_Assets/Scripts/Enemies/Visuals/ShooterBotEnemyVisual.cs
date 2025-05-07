using UnityEngine;
using UnityEngine.UI;

public class ShooterBotEnemyVisual :MonoBehaviour
{
    [SerializeField] private  ShooterBotEnemy enemy;
    [SerializeField] private Image healthBar;
    [SerializeField] private Transform model;
    [SerializeField] private Animator animator;

    private ShooterBotEnemy.State state;
    private void Start()
    {
        animator.SetTrigger("Idle");
        enemy.OnDamage += OnDamage;
        enemy.OnAttack += OnAttack;
        enemy.OnStateChange += OnStateChange;
    }
    private void Update()
    {
        switch (state)
        {
            case ShooterBotEnemy.State.Attack:

                Vector3 targetDirection = Player.CharacterInstance.playerBehaviourTree.modelTransform.transform.position - transform.position;
                targetDirection.y = 0;
                targetDirection.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                model.rotation = Quaternion.Slerp(model.rotation, targetRotation, Time.deltaTime * 5f);
                
                break;
            default:
                break;
        }
    }
    private void OnStateChange(ShooterBotEnemy.State s)
    {
        state = s;

        switch (state)
        {
            case ShooterBotEnemy.State.Idle:
                animator.SetTrigger("Idle");
                break;
            case ShooterBotEnemy.State.Attack:
                break;
            case ShooterBotEnemy.State.Dead:
                animator.SetBool("Dead",true);
                break;
            default:
                break;
        }
    }

    private  void OnDamage()
    {
        healthBar.fillAmount = enemy.GetHealthNormalized() ;
        animator.SetTrigger("Damage");
    }
    private void OnAttack()
    {
        animator.SetTrigger("Attack");
    }

    private void OnDestroy()
    {
        enemy.OnDamage -= OnDamage;
    }

}
