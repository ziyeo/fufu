using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class PureWaterSlash : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType target = TargetType.AnyEnemy;
    private const bool showInLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];

    public override string PortraitPath => "res://fufu/images/card_portraits/PureWaterSlash.png";

    public PureWaterSlash() : base(energyCost, type, rarity, target, showInLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        if (cardPlay.Target == null)
            return;

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) return;
        Player players = player.Player;

        // 造成 6 点伤害
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 角色为【荒属性】时：召唤 1 名基础沙龙（持续 2 回合）
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            bool hasSalonBond = player.HasPower<SalonBondPower>();
            int salonBond = player.GetPowerAmount<SalonBondPower>();
            await PowerCmd.Apply<ShadowSalonPower>(choiceContext,player, 2, player, this);
            if(hasSalonBond)
                await CardPileCmd.Draw(choiceContext, salonBond, players);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 +4
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}