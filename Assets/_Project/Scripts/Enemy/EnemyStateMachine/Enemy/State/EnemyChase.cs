using System.Linq;
using UnityEngine;

public class EnemyChase : BaseState
{
    protected EnemyFSM fsm;

    public string sfxName;
    public float sfxInterval;
    protected float sfxCooldown;

    public SfxChannelPlayer ApproachSfx { get; private set; }

    protected Ray ray;
    protected RaycastHit hit;

    public override void Init(FiniteStateMachine fsm)
    {
        this.fsm = fsm as EnemyFSM;
        ApproachSfx = !ApproachSfx ? GetComponentsInChildren<SfxChannelPlayer>().ToList().Find(obj => obj.sfxName == sfxName).Init() : ApproachSfx;
        ray = new Ray();
        hit = new RaycastHit();
    }

    public override void Enter()
    {
        fsm.Anima.SetTrigger("IsRun");
        if (!fsm.Pv || fsm.Pv.IsMine)
        {
            fsm.Agent.enabled = true;
            if (fsm.Agent.enabled) fsm.Agent.isStopped = false;
        }
        else
        {
            fsm.Agent.enabled = false;
        }
    }

    public override void UpdateLogic()
    {
        if (!fsm.Pv || fsm.Pv.IsMine)
        {
            fsm.Agent.SetDestination(GameManager.Instance.GetNearestPlayerPosition(transform.position));
        }
        
        foreach (var i in fsm.attackStates)
        {
            if (i.CanAttackPlayer())
            {
                fsm.CurrentState = i;
                break;
            }
        }

        sfxCooldown -= Time.deltaTime;
        if (sfxCooldown < 0)
        {
            sfxCooldown = sfxInterval;
            ApproachSfx.PlaySfx();
        }
    }

    public override void UpdatePhysics()
    {

    }

    public override void Exit()
    {
        if (fsm.Agent.enabled) fsm.Agent.isStopped = true;
        fsm.Agent.enabled = false;
    }


}
