using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Power;

public class WorldWaterGodPower : CustomPowerModel
{
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://fufu/images/powers/world_water_god.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/world_water_god.png";
    /// <summary>
    /// 回合结束后，每有10元素能量，给【沙龙伤害Buff】+1。
    /// </summary>
    // 回合结束：给沙龙加伤害
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        await base.AfterTurnEnd(choiceContext, side);
        if (side != Owner.Side) return;
        Creature player = base.Owner;
        if (player.CombatState == null)
            return;

        // 每10元素 = +1
        int element = Owner.GetPowerAmount<ElementEnergyPower>();
        int add = element / 10;

        if (add > 0)
        {
            var power = await PowerCmd.Apply<SalonTempDamageBuffPower>(choiceContext, player, add * Amount, player, null);
            if (power != null)
            {              
                power.Duration = 1;               
            }
        }
    }
}