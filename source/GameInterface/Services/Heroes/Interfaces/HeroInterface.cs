﻿using Common;
using Common.Extensions;
using Common.Logging;
using Common.Serialization;
using GameInterface.Serialization;
using GameInterface.Serialization.External;
using GameInterface.Services.Heroes.Data;
using GameInterface.Services.ObjectManager;
using GameInterface.Services.Registry;
using Serilog;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;

namespace GameInterface.Services.Heroes.Interfaces;

public interface IHeroInterface : IGameAbstraction
{
    byte[] PackageMainHero();
    bool TryResolveHero(string controllerId, out string heroId);
    void SwitchMainHero(string heroId);
    /// <summary>
    /// Unpacks and properly constructs hero in the game
    /// </summary>
    /// <param name="bytes">Hero as bytes</param>
    /// <returns>Hero string identifier</returns>
    NewPlayerData UnpackHero(string controllerId, byte[] bytes);
}

internal class HeroInterface : IHeroInterface
{
    private static readonly ILogger Logger = LogManager.GetLogger<HeroInterface>();
    private readonly IObjectManager objectManager;
    private readonly IBinaryPackageFactory binaryPackageFactory;
    private readonly IHeroRegistry heroRegistry;

    public HeroInterface(
        IObjectManager objectManager,
        IBinaryPackageFactory binaryPackageFactory,
        IHeroRegistry heroRegistry)
    {
        this.objectManager = objectManager;
        this.binaryPackageFactory = binaryPackageFactory;
        this.heroRegistry = heroRegistry;
    }

    public byte[] PackageMainHero()
    {
        Hero.MainHero.StringId = string.Empty;
        Hero.MainHero.PartyBelongedTo.StringId = string.Empty;

        HeroBinaryPackage package = binaryPackageFactory.GetBinaryPackage<HeroBinaryPackage>(Hero.MainHero);
        return BinaryFormatterSerializer.Serialize(package);
    }

    public NewPlayerData UnpackHero(string controllerId, byte[] bytes)
    {
        Hero hero = null;

        GameLoopRunner.RunOnMainThread(() => {
            hero = UnpackMainHeroInternal(bytes);
        },
        blocking: true);

        // TODO not saving correctly on server
        heroRegistry.TryRegisterHeroController(controllerId, hero.StringId);

        var playerData = new NewPlayerData() {
            HeroData = bytes,
            HeroStringId = hero.StringId,
            PartyStringId = hero.PartyBelongedTo.StringId,
            CharacterObjectStringId = hero.CharacterObject.StringId,
            ClanStringId = hero.Clan.StringId
        };

        return playerData;
    }

    private Hero UnpackMainHeroInternal(byte[] bytes)
    {
        HeroBinaryPackage package = BinaryFormatterSerializer.Deserialize<HeroBinaryPackage>(bytes);
        var hero = package.Unpack<Hero>(binaryPackageFactory);

        SetupNewHero(hero);

        return hero;
    }

    public bool TryResolveHero(string controllerId, out string heroId) => heroRegistry.TryGetControlledHero(controllerId, out heroId);

    public void SwitchMainHero(string heroId)
    {
        if(objectManager.TryGetObject(heroId, out Hero resolvedHero))
        {
            Logger.Information("Switching to new hero: {heroName}", resolvedHero.Name.ToString());

            ChangePlayerCharacterAction.Apply(resolvedHero);
        }
        else
        {
            Logger.Warning("Could not find hero with id of: {guid}", heroId);
        }
    }

    private void SetupNewHero(Hero hero)
    {
        SetupHeroWithObjectManagers(hero);
        SetupNewParty(hero);
    }

    private static readonly Action<CampaignObjectManager, Hero> CampaignObjectManager_AddHero = typeof(CampaignObjectManager)
    .GetMethod("AddHero", BindingFlags.Instance | BindingFlags.NonPublic)
    .BuildDelegate<Action<CampaignObjectManager, Hero>>();
    private static readonly Action<CampaignObjectManager, MobileParty> CampaignObjectManager_AddMobileParty = typeof(CampaignObjectManager)
        .GetMethod("AddMobileParty", BindingFlags.Instance | BindingFlags.NonPublic)
        .BuildDelegate<Action<CampaignObjectManager, MobileParty>>();
    private void SetupHeroWithObjectManagers(Hero hero)
    {
        objectManager.AddNewObject(hero, out string heroId);
        objectManager.AddNewObject(hero.PartyBelongedTo, out string partyId);

        var campaignObjectManager = Campaign.Current?.CampaignObjectManager;
        if (campaignObjectManager == null)
        {
            Logger.Error("{type} was null when trying to register a {managedType}", typeof(CampaignObjectManager), typeof(Hero));
            return;
        }

        CampaignObjectManager_AddHero(campaignObjectManager, hero);

        var party = hero.PartyBelongedTo;

        CampaignObjectManager_AddMobileParty(campaignObjectManager, party);

        var partyBase = party.Party;

        partyBase.Visuals.OnStartup(partyBase);
        partyBase.Visuals.SetMapIconAsDirty();
    }

    private void SetupNewParty(Hero hero)
    {
        var party = hero.PartyBelongedTo;
        party.IsVisible = true;
        party.Party.Visuals.SetMapIconAsDirty();

        typeof(MobileParty).GetMethod("RecoverPositionsForNavMeshUpdate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(party, null);
        typeof(MobileParty).GetProperty("CurrentNavigationFace").SetValue(
            party,
            Campaign.Current.MapSceneWrapper.GetFaceIndex(party.Position2D));

        typeof(MobilePartyAi).GetMethod("OnGameInitialized", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(party.Ai, null);

        CampaignEventDispatcher.Instance.OnPartyVisibilityChanged(party.Party);
    }
}
