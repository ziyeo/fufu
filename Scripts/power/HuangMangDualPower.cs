using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Fufu.FufuModCode.Character;

namespace Fufu.Scripts.Power;

public class HuangMangDualPower : CustomPowerModel
{
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://fufu/images/powers/huangmang_dual.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/huangmang_dual.png";
    // "荒：回合结束召唤1沙龙。芒：回合结束召唤1歌者。";

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);
        if (side != Owner.Side) return;

        Creature player = Owner;
        if (player == null) return;

        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            // 荒：召唤1沙龙
            await PowerCmd.Apply<ShadowSalonPower>(choiceContext, player, 1, player, null);
        }
        else
        {
            // 芒：召唤1歌者
            await PowerCmd.Apply<BasicSingerPower>(choiceContext, player, 1, player, null);
        }
    }
}