using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;


namespace Fufu.Scripts.Power;

public class OperaStancePower : CustomPowerModel
{
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://fufu/images/powers/opera_stance.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/opera_stance.png";
    // 完全参照你的 ShadowSalonPower 写法！
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);

        // 只在自己回合触发
        if (side != Owner.Side)
            return;

        // 获取气氛
        int atmosphere = Owner.GetPowerAmount<ElementEnergyPower>();
        if (atmosphere < 15)
            return;

        // 计算力量
        int count = atmosphere / 15;
        int total = count * (int)Amount;

        // ✅ 官方原生写法（完全参照Rupture）
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, total, Owner, null);
    }
}