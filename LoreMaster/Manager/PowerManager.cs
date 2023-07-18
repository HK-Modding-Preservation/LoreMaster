using LoreMaster.Enums;
using LoreMaster.LorePowers;
using LoreMaster.LorePowers.Ancient_Basin;
using LoreMaster.LorePowers.CityOfTears;
using LoreMaster.LorePowers.Crossroads;
using LoreMaster.LorePowers.Deepnest;
using LoreMaster.LorePowers.Dirtmouth;
using LoreMaster.LorePowers.FogCanyon;
using LoreMaster.LorePowers.FungalWastes;
using LoreMaster.LorePowers.Greenpath;
using LoreMaster.LorePowers.HowlingCliffs;
using LoreMaster.LorePowers.KingdomsEdge;
using LoreMaster.LorePowers.Peaks;
using LoreMaster.LorePowers.QueensGarden;
using LoreMaster.LorePowers.RestingGrounds;
using LoreMaster.LorePowers.Waterways;
using LoreMaster.LorePowers.WhitePalace;
using LoreMaster.SaveManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoreMaster.Manager;

/// <summary>
/// Manager for handling the powers.
/// </summary>
internal static class PowerManager
{
    #region Members

    private static Dictionary<string, Power> _powerList = new()
    {
        // Dirtmouth/King's Pass
        {"TUT_TAB_01", new WellFocusedPower() },
        {"TUT_TAB_02", new ScrewTheRulesPower() },
        {"TUT_TAB_03", new TrueFormPower() },
        {"BRETTA", new CaringShellPower() },
        {"GRAVEDIGGER", new RequiemPower() },
        // Crossroads
        {"PILGRIM_TAB_01", new ReluctantPilgrimPower() },
        {"COMPLETION_RATE_UNLOCKED", new GreaterMindPower() },
        {"MYLA", new DiamondDashPower() },
        {"MENDERBUG", new BestMenderInTheWorldPower() },
        // Greenpath
        {"GREEN_TABLET_01", new TouchGrassPower() },
        {"GREEN_TABLET_02", new GiftOfUnnPower() },
        {"GREEN_TABLET_03", new MindblastOfUnnPower() },
        {"GREEN_TABLET_05", new CamouflagePower() },
        {"GREEN_TABLET_06", new ReturnToUnnPower() },
        {"GREEN_TABLET_07", new GraspOfLifePower() },
        // Fungal Wastes
        {"FUNG_TAB_04", new OneOfUsPower() },
        {"FUNG_TAB_01", new PaleLuckPower() },
        {"FUNG_TAB_02", new ImposterPower() },
        {"FUNG_TAB_03", new UnitedWeStandPower() },
        {"MANTIS_PLAQUE_01", new MantisStylePower() },
        {"MANTIS_PLAQUE_02", new EternalValorPower() },
        {"PILGRIM_TAB_02", new GloryOfTheWealthPower() },
        {"WILLOH", new BagOfMushroomsPower() },
        // City of Tears
        {"RUIN_TAB_01", new HotStreakPower() },
        {"FOUNTAIN_PLAQUE_DESC", new TouristPower()},
        {"RUINS_MARISSA_POSTER", new MarissasAudiencePower() },
        {"MAGE_COMP_01", new SoulExtractEfficiencyPower() },
        {"MAGE_COMP_02", new OverwhelmingPowerPower() },
        {"MAGE_COMP_03", new PureSpiritPower() },
        {"LURIAN_JOURNAL", new EyeOfTheWatcherPower() },
        {"EMILITIA", new HappyFatePower() },
        {"MARISSA", new BlessingOfTheButterflyPower() },
        {"POGGY", new DeliciousMealPower() },
        // Waterways
        {"DUNG_DEF_SIGN", new EternalSentinelPower() },
        {"FLUKE_HERMIT", new RelentlessSwarmPower() },
        // Crystal Peaks
        {"QUIRREL", new DiamondCorePower() },
        // Resting Grounds
        {"DREAMERS_INSPECT_RG5", new DreamBlessingPower() },
        {"TISO", new DramaticEntrancePower() },
        // Howling Cliffs
        {"CLIFF_TAB_02", new LifebloodOmenPower() },
        {"JONI", new JonisProtectionPower() },
        {"STAG_EGG_INSPECT", new StagAdoptionPower() },
        // Fog Canyon
        {"ARCHIVE_01", new FriendOfTheJellyfishPower() },
        {"ARCHIVE_02", new JellyBellyPower() },
        {"ARCHIVE_03", new JellyfishFlowPower() },
        // Ancient Basin
        {"ABYSS_TUT_TAB_01", new WeDontTalkAboutShadePower() },
        // Deepnest
        {"MASKMAKER", new MaskOverchargePower() },
        {"MIDWIFE", new InfestedPower() },
        {"ZOTE", new LaughableThreatPower() },
        // Kingdom's Edge
        {"MR_MUSH_RIDDLE_TAB_NORMAL", new WisdomOfTheSagePower() },
        {"BARDOON", new ConcussiveStrikePower() },
        {"HIVEQUEEN", new YouLikeJazzPower() },
        // Queen's Garden
        {"XUN_GRAVE_INSPECT", new FlowerRingPower() },
        {"QUEEN", new QueenThornsPower() },
        {"MOSSPROPHET", new FollowTheLightPower() },
        {"GRASSHOPPER", new GrassBombardementPower() },
        // White Palace
        {"WP_WORKSHOP_01", new ShadowForgedPower() },
        {"WP_THRONE_01", new ShiningBoundPower() },
        {"PLAQUE_WARN", new DiminishingCursePower() },
        {"POP", new SacredShellPower()},
    };

    #endregion

    #region Properties

    public static List<Power> ObtainedPowers { get; set; } = new();

    /// <summary>
    /// Gets or sets the flag that indicates if powers can be activated. This is used for end cutscenes.
    /// </summary>
    public static bool CanPowersActivate { get; set; } = true;

    /// <summary>
    /// Gets or sets the control state of the power inventory page.
    /// </summary>
    public static PowerControlState ControlState { get; set; }

    public static Dictionary<string, PowerRank> GlobalPowerStates { get; set; }

    #endregion

    #region Methods

    #region Power Information

    /// <summary>
    /// Check if the key is binded to a power and possibly add it.
    /// </summary>
    /// <param name="key">The ingame language key that is used. For NPC this mod uses just the names of the npc</param>
    /// <param name="power">The power that matches the key, if no match has been found, this will be null.</param>
    /// <param name="collectIfPossible">If <see langword="true"/>, the power will automatically be added if it isn't in <see cref="ObtainedPowers"/>.</param>
    /// <returns>True if a matching power was found</returns>
    public static bool GetPowerByKey(string key, out Power power, bool collectIfPossible = true)
    {
        power = null;
        if (string.IsNullOrEmpty(key))
            return false;
        if (_powerList.TryGetValue(key.ToUpper(), out power))
            try
            {
                if (collectIfPossible && !ObtainedPowers.Contains(power))
                {
                    if (power is EyeOfTheWatcherPower watcherPower)
                        watcherPower.EyeActive = true;
                    ObtainedPowers.Add(power);
                    power.EnablePower();
                    UpdateTracker(SettingManager.Instance.CurrentArea);
                    LorePage.UpdateLorePage();
                }
                return true;
            }
            catch (Exception exception)
            {
                LoreMaster.Instance.LogError(exception.Message);
            }
        return false;
    }

    public static bool GetPowerByName(string name, out Power power, bool ignoreWhiteSpaces = true, bool collectIfPossible = true)
    {
        power = null;
        if (string.IsNullOrEmpty(name))
            return false;
        if (_powerList.Values.FirstOrDefault(x => string.Equals(name, ignoreWhiteSpaces ? x.PowerName.Replace(" ", "") : x.PowerName, StringComparison.CurrentCultureIgnoreCase)) is Power foundPower)
            try
            {
                power = foundPower;
                if (collectIfPossible && !ObtainedPowers.Contains(foundPower))
                {
                    if (power is EyeOfTheWatcherPower watcherPower)
                        watcherPower.EyeActive = true;
                    if (power is GreaterMindPower)
                        ObtainedPowers.Insert(0, power);
                    else
                        ObtainedPowers.Add(power);
                    power.EnablePower();
                    UpdateTracker(SettingManager.Instance.CurrentArea);
                    LorePage.UpdateLorePage();
                }
                return true;
            }
            catch (Exception exception)
            {
                LoreMaster.Instance.LogError(exception.Message);
            }
        else
            LoreMaster.Instance.Log("Couldn't find power: " + name);
        power = null;
        return false;
    }

    internal static IEnumerable<Power> GetAllPowers() => _powerList.Values;

    public static bool HasObtainedPower(string key, bool onlyActive = true)
    {
        if (_powerList.TryGetValue(key, out Power power))
            return ObtainedPowers.Contains(power) && (!onlyActive || power.State == PowerState.Active);
        return false;
    }

    internal static void DisableAllPowers()
    {
        CanPowersActivate = false;
        foreach (Power power in ObtainedPowers)
            power.DisablePower(true);
    }

    /// <summary>
    /// Loads the acquired powers and tags of powers in the save file.
    /// </summary>
    /// <param name="saveData"></param>
    internal static void LoadPowers(LoreMasterLocalSaveData saveData)
    {
        ObtainedPowers.Clear();
        foreach (string key in saveData.Tags.Keys)
        {
            _powerList[key].StayTwisted = saveData.Tags[key].Item2;
        }

        foreach (string key in saveData.ObtainedPowerKeys)
        {
            // Since this method would normally activate the power instantly, we add the power later. This is because I'm unsure when local settings are loaded.
            if (string.Equals(key, "dream warrior"))
                ObtainedPowers.Add(new PlaceholderPower());
            GetPowerByKey(key, out Power pow, false);
            if (pow != null)
                ObtainedPowers.Add(pow);
        }
    }

    /// <summary>
    /// Loads power specific data in the save file.
    /// </summary>
    /// <param name="saveData"></param>
    internal static void LoadPowerData(LocalPowerSaveData saveData)
    {
        if (saveData == null)
            return;
        GloryOfTheWealthPower.GloryCost = saveData.GloryCost;
        StagAdoptionPower.Instance.CanSpawnStag = saveData.CanSpawnStag;
    }

    /// <summary>
    /// Saves the acquired powers and tags.
    /// </summary>
    internal static void SavePowers(ref LoreMasterLocalSaveData saveData)
    {
        foreach (string key in _powerList.Keys)
        {
            if (ObtainedPowers.Contains(_powerList[key]))
                saveData.ObtainedPowerKeys.Add(key);
        }

        // Place the fake powers in the save data as well.
        if (ObtainedPowers.Any(x => x is PlaceholderPower))
            for (int i = 0; i < ObtainedPowers.Count(x => x is PlaceholderPower); i++)
                saveData.ObtainedPowerKeys.Add("dream warrior");
    }

    /// <summary>
    /// Prepare the save data for power specific data.
    /// </summary>
    internal static LocalPowerSaveData PreparePowerData() => new()
    {
        CanSpawnStag = StagAdoptionPower.Instance.CanSpawnStag,
        GloryCost = GloryOfTheWealthPower.GloryCost
    };


    internal static void AddPower(string key, Power power) => _powerList.Add(key, power);

    #endregion

    /// <summary>
    /// Let all powers execute their behaviour for entering a new room.
    /// </summary>
    public static void ExecuteSceneActions()
    {
        foreach (Power power in ObtainedPowers)
            if (power.State != PowerState.Disabled && power.SceneAction != null)
                try
                {
                    if (power.State == PowerState.Twisted || (SettingManager.Instance.GameMode != GameMode.Heroic && SettingManager.Instance.GameMode != GameMode.Disabled))
                        power.SceneAction.Invoke();
                }
                catch (Exception exception)
                {
                    LoreMaster.Instance.LogError("Error while executing scene action for " + power.PowerName + ": " + exception.Message + "StackTrace: " + exception.StackTrace);
                }
    }

    /// <summary>
    /// Updates the lore tracker.
    /// </summary>
    public static void UpdateTracker(Area areaToUpdate)
    {
        try
        {
            if (ObtainedPowers.Contains(_powerList["COMPLETION_RATE_UNLOCKED"]))
            {
                GreaterMindPower logPower = (GreaterMindPower)_powerList["COMPLETION_RATE_UNLOCKED"];
                if (logPower.State == PowerState.Active)
                    logPower.UpdateLoreCounter(ObtainedPowers, _powerList.Values, areaToUpdate, true);
            }
        }
        catch (Exception exception)
        {
            LoreMaster.Instance.LogError(exception.Message);
        }
    }

    #endregion
}
