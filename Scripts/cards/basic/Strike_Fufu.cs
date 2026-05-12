using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.CardPools;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using Fufu.Scripts.Relics;
using MegaCrit.Sts2.Core.ValueProps;


namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class Strike_Fufu : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Basic;
    private const TargetType target = TargetType.AnyEnemy;
    private const bool showInLibrary = true;
    public static bool _hasAppliedInitialBuffs { get; set; } = false;
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];

    public override string PortraitPath => "res://fufu/images/card_portraits/Strike_Fufu.png";

    public Strike_Fufu() : base(energyCost, type, rarity, target, showInLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        
        if (cardPlay.Target != null)
        {
            Creature player = base.Owner.Creature;
            await base.OnPlay(choiceContext, cardPlay);
           // if (!_hasAppliedInitialBuffs)
           // {
            //_hasAppliedInitialBuffs = true;

            // // 设置 元素能量
            // await PowerCmd.SetAmount<ElementEnergyPower>(
            //     player, 
            //     Fufu.FufuModCode.Character.FufuMod.ElementEnergyValue, 
            //     player, 
            //     null
            // );

            // // 设置 气氛值
            // await PowerCmd.SetAmount<AtmospherePower>(
            //     player, 
            //     Fufu.FufuModCode.Character.FufuMod.AtmosphereValue, 
            //     player, 
            //     null
            //);
            //}
            if (FufuModCode.Character.FufuMod.CurrentAttr != FufuAttr.Huang){
                FufuModCode.Character.FufuMod.CurrentAttr = FufuAttr.Huang;
                await HuangMangOriginSeal.SwitchToHuangAttribute(player,choiceContext);
            }
            
            await HuangMangOriginSeal.RefreshStateIcon(player,choiceContext);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext, player, 1, player, this);           
        }
        else
            return;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}