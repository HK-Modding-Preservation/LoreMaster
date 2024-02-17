using LoreMaster.Enums;
using Modding;
using System.Collections;
using UnityEngine;

namespace LoreMaster.LorePowers.CityOfTears;

public class HotStreakPower : Power
{
    #region Members

    private int _damageStacks = 0;
    private bool _currentlyRunning;
    private bool _hasHitEnemy;

    #endregion

    #region Constructors

    public HotStreakPower() : base("Hot Streak", Area.CityOfTears) { }

    #endregion

    #region Properties

    public override PowerRank Rank => PowerRank.Greater;

    #endregion

    #region Event Handler

    /// <summary>
    /// Event handler when the player slashes with the nail.
    /// </summary>
    private void NailSlash(Collider2D otherCollider, GameObject slash)
    {
        // This event is fired multiple times, therefore we check every instance if an enemy was hit
        if (otherCollider.gameObject.GetComponent<HealthManager>())
            _hasHitEnemy = true;
        // To prevent running multiple coroutines
        if (_currentlyRunning)
            return;
        _currentlyRunning = true;
        _runningCoroutine = LoreMaster.Instance.Handler.StartCoroutine(HitCooldown());
    }

    /// <summary>
    /// Event handler, when the game asks for the nail damage.
    /// </summary>
    private int EmpowerNail(string name, int damage)
    {
        if (string.Equals(name, "nailDamage"))
        {
            if (State == PowerState.Active)
                damage += _damageStacks;
            else
                damage = Mathf.Max(1, _damageStacks);
        }
        return damage;
    }

    #endregion

    #region Control

    /// <inheritdoc/>
    protected override void Enable()
    {
        ModHooks.SlashHitHook += NailSlash;
        ModHooks.GetPlayerIntHook += EmpowerNail;
    }

    /// <inheritdoc/>
    protected override void Disable()
    {
        ModHooks.SlashHitHook -= NailSlash;
        ModHooks.GetPlayerIntHook -= EmpowerNail;
        _damageStacks = 0;
        _currentlyRunning = false;
        _hasHitEnemy = false;
        UpdateNail();
    }

    /// <inheritdoc/>
    protected override void TwistEnable() => Enable();

    /// <inheritdoc/>
    protected override void TwistDisable() => Disable();

    #endregion

    #region Private Methods

    /// <summary>
    /// Waits for the hit to finish and then checks if an enemy was hit.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HitCooldown()
    {
        // Give the event handler time to acknowledge a hit.
        yield return new WaitForSeconds(0.25f);

        if (_hasHitEnemy)
        {
            if (_damageStacks < (PlayerData.instance.GetInt(nameof(PlayerData.instance.nailSmithUpgrades)) + 1) * 3)
                _damageStacks++;
        }
        else
            _damageStacks = 0;

        UpdateNail();
    }

    /// <summary>
    /// Updates the nail and resets the flags.
    /// </summary>
    private void UpdateNail()
    {
        LoreMaster.Instance.Handler.StartCoroutine(WaitThenUpdate());
        _hasHitEnemy = false;
        _currentlyRunning = false;
    }

    /// <summary>
    /// Wait a frame and then call upon a nail damage update.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitThenUpdate()
    {
        yield return null;
        PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
    }

    #endregion
}
