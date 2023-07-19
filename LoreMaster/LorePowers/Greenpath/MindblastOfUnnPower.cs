using KorzUtils.Helper;
using LoreMaster.Enums;
using LoreMaster.Manager;
using LoreMaster.UnityComponents;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoreMaster.LorePowers.Greenpath;

public class MindblastOfUnnPower : Power
{
    #region Members

    private List<HealthManager> _enemies = new();

    #endregion

    #region Constructors

    public MindblastOfUnnPower() : base("Mindblast of Unn", Area.Greenpath) { }

    #endregion

    #region Properties

    public override Action SceneAction => () => _enemies.Clear();

    #endregion

    #region Event handler

    private void AddMindblastDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        MindBlast mindBlast = self.gameObject.GetComponent<MindBlast>();
        if (State == PowerState.Active && mindBlast != null && hitInstance.DamageDealt > 0)
            hitInstance.DamageDealt += mindBlast.ExtraDamage;
        orig(self, hitInstance);
    }

    /// <summary>
    /// Updates the dream nail color.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="controller"></param>
    private void UpdateDreamNailColor(PlayerData data, HeroController controller)
    {
        string colorCode = string.Empty;
        colorCode += PlayerData.instance.GetBool("equippedCharm_30") ? 1 : 0;
        colorCode += PlayerData.instance.GetBool("equippedCharm_38") ? 1 : 0;
        colorCode += PlayerData.instance.GetBool("equippedCharm_28") ? 1 : 0;
        Color dreamNailColor = colorCode switch
        {
            "001" => Color.green,
            "010" => Color.red,
            "011" => Color.blue,
            "100" => Color.yellow,
            "101" => Color.cyan,
            "110" => new(1f, 0.4f, 0f),// Orange
            "111" => new(1f, 0f, 1f),// Purple
            _ => Color.white,
        };

        // Color all dream nail components accordingly (just for fun)
        foreach (tk2dSprite dreamNailComponent in HeroHelper.DreamNailSprites)
            dreamNailComponent.color = dreamNailColor;
    }

    /// <summary>
    /// Apply mindblast stacks to the target.
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="self"></param>
    private void Apply_Mindblast(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
    {
        orig(self);
        MindBlast mindBlast = self.GetComponent<MindBlast>();
        if (mindBlast == null)
            mindBlast = self.gameObject.AddComponent<MindBlast>();

        int extraDamage = 2;
        // 1 extra damage if dream wielder is equipped
        if (PlayerData.instance.GetBool("equippedCharm_30"))
            extraDamage++;
        // 2 extra damage if dream shield is equipped (like if this is ever going to happen)
        if (PlayerData.instance.GetBool("equippedCharm_38"))
            extraDamage += 2;
        // 3 extra damage if shape of unn is equipped
        if (PlayerData.instance.GetBool("equippedCharm_28"))
            extraDamage += 3;
        // Double the damage if awoken dreamnail has been acquired.
        if (PlayerData.instance.GetBool(nameof(PlayerData.instance.dreamNailUpgraded)))
            extraDamage *= 2;
        mindBlast.ExtraDamage += extraDamage;
    }

    private void HealthManager_Hit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
    {
        MindBlast mindBlast = self.gameObject.GetComponent<MindBlast>();
        if (!PlayerData.instance.GetBool("hasDreamNail") && !_enemies.Contains(self))
        {
            self.hp *= 3;
            _enemies.Add(self);
        }
        else if (PlayerData.instance.GetBool("hasDreamNail") && mindBlast == null)
            hitInstance.DamageDealt = 0;
        orig(self, hitInstance);
    }

    #endregion

    #region Control

    /// <inheritdoc/>
    protected override void Enable()
    {
        On.EnemyDreamnailReaction.RecieveDreamImpact += Apply_Mindblast;
        On.HealthManager.TakeDamage += AddMindblastDamage;
        ModHooks.CharmUpdateHook += UpdateDreamNailColor;
    }

    /// <inheritdoc/>
    protected override void Disable()
    {
        On.EnemyDreamnailReaction.RecieveDreamImpact -= Apply_Mindblast;
        On.HealthManager.TakeDamage -= AddMindblastDamage;
        ModHooks.CharmUpdateHook -= UpdateDreamNailColor;
        foreach (tk2dSprite dreamNailComponent in HeroHelper.DreamNailSprites)
            dreamNailComponent.color = Color.white;
    }

    /// <inheritdoc/>
    protected override void TwistEnable()
    {
        On.EnemyDreamnailReaction.RecieveDreamImpact += Apply_Mindblast;
        On.HealthManager.Hit += HealthManager_Hit;    
    }

    /// <inheritdoc/>
    protected override void TwistDisable()
    {
        On.EnemyDreamnailReaction.RecieveDreamImpact -= Apply_Mindblast;
        On.HealthManager.Hit -= HealthManager_Hit;
    }

    #endregion
}
