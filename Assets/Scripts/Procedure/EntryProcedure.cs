using Flower;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine.SceneManagement;

namespace Procedure
{
    public class EntryProcedure:ProcedureBase
    {
        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (MyGameEntry.Scene)
            {
                MyGameEntry.CustomScene.LoadScene("MainScene", LoadSceneMode.Additive);
                ChangeState<RunningProcedure>(procedureOwner);
            }
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }
    }
}