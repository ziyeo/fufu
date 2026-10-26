using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Fufu.Scripts.Power;

public class ShadowSalonPower : CustomPowerModel
{
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://fufu/images/powers/shadow_salon.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/shadow_salon.png";

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);

        // 只在自己的回合结束触发
        if (side != Owner.Side)
            return;

        Creature player = Owner;
        if (player.CombatState == null)
            return;
        int permanentBuff = player.GetPowerAmount<SalonDamageBuffPower>();
        int tempBuff = player.GetPowerAmount<SalonTempDamageBuffPower>();
        int totalBuff = permanentBuff + tempBuff;
        int totalDamage = Amount * (4 + totalBuff); 
        var aliveEnemies = player.CombatState.Enemies.Where(e => e.IsAlive).ToList();

        // 对随机敌人造成伤害
        if (aliveEnemies.Any())
        {
            Creature target = aliveEnemies.First();
            await CreatureCmd.Damage(
                choiceContext,
                target,
                totalDamage,     
                ValueProp.Unblockable | ValueProp.Unpowered,
                player
            );
        }

        // 对自己造成伤害
        await CreatureCmd.Damage(
            choiceContext,
            player,
            Amount,            // 层数 × 1
            ValueProp.Unblockable | ValueProp.Unpowered,
            player
        );

        // 持续时间 -1（官方原版）
        await PowerCmd.TickDownDuration(this);
    }
}