using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;


namespace EzrealHu3
{
    class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Skillshot R;
        public static Menu EzrealMenu, SettingsMenu;


        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Ezreal")
                return;

            Q = new Spell.Skillshot(SpellSlot.Q, 1190, SkillShotType.Linear, 250, Int32.MaxValue, 60);
            W = new Spell.Skillshot(SpellSlot.W, 990, SkillShotType.Linear, 250, Int32.MaxValue, 80);
            E = new Spell.Targeted(SpellSlot.E, 700);
            R = new Spell.Skillshot(SpellSlot.R, 2000, SkillShotType.Linear, 1, Int32.MaxValue, 160);

            EzrealMenu = MainMenu.AddMenu("Ezreal Hu3", "ezrealhu3");
            EzrealMenu.AddGroupLabel("Ezreal Hu3 3.5");
            EzrealMenu.AddSeparator();
            EzrealMenu.AddLabel("Made By MarioGK");

            SettingsMenu = EzrealMenu.AddSubMenu("Settings", "Settings");
            SettingsMenu.AddGroupLabel("Settings");
            SettingsMenu.AddLabel("Combo");
            SettingsMenu.Add("comboQ", new CheckBox("Use Q on Combo"));
            SettingsMenu.Add("comboW", new CheckBox("Use W on Combo"));
            SettingsMenu.Add("comboR", new CheckBox("Use R on Combo"));
            SettingsMenu.AddLabel("Harass");
            SettingsMenu.Add("harassQ", new CheckBox("Use Q on Harass"));
            SettingsMenu.Add("harassW", new CheckBox("Use W on Harass"));
            SettingsMenu.AddLabel("LastHit");
            SettingsMenu.Add("lasthitQ", new CheckBox("Use Q on LastHit"));
            SettingsMenu.Add("lasthitMana", new Slider("Mana % To Use Q LastHit", 30, 0, 100));
            SettingsMenu.AddLabel("LaneClear");
            SettingsMenu.Add("laneclearQ", new CheckBox("Use Q on LaneClear"));
            SettingsMenu.Add("laneclearMana", new Slider("Mana % To Use Q LaneClear", 30, 0, 100));
            SettingsMenu.AddLabel("KillSteal");
            SettingsMenu.Add("Qks", new CheckBox("Use Q KillSteal"));
            SettingsMenu.Add("Wks", new CheckBox("Use W KillSteal"));
            SettingsMenu.Add("Rks", new CheckBox("Use R KillSteal"));
            SettingsMenu.AddLabel("Draw");
            SettingsMenu.Add("Qd", new CheckBox("Draw Q"));
            SettingsMenu.Add("Wd", new CheckBox("Draw W"));
            SettingsMenu.Add("Rd", new CheckBox("Draw R Combo"));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;

        }
        private static void Game_OnTick(EventArgs args)
        {
            if (_Player.IsDead || MenuGUI.IsChatOpen) return;

            KillSteal();
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Harass)
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.LastHit)
            {
                LastHit();
            }
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.LaneClear)
            {
                LaneClear();
            }          
        }
        private static void KillSteal()
        {
            var useQ = SettingsMenu["Qks"].Cast<CheckBox>().CurrentValue;
            var useW = SettingsMenu["Wks"].Cast<CheckBox>().CurrentValue;
            var useR = SettingsMenu["Rks"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);

            if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsDead && !target.IsZombie && Q.GetPrediction(target).HitChance >= HitChance.Medium
                && target.Health <= Player.Instance.GetSpellDamage(target, SpellSlot.Q)) 
            {
                Q.Cast(target);
            }
            if (useW && W.IsReady() && target.IsValidTarget(W.Range) && !target.IsDead && !target.IsZombie && W.GetPrediction(target).HitChance >= HitChance.Medium
                && target.Health <= Player.Instance.GetSpellDamage(target, SpellSlot.W))
            {
                W.Cast(target);
            }
            if (useR && R.IsReady() && target.IsValidTarget(Q.Range) && !target.IsDead && !target.IsZombie && R.GetPrediction(target).HitChance >= HitChance.High
                && target.Health <= Player.Instance.GetSpellDamage(target, SpellSlot.R))
            {
                R.Cast(target);
            }
        }

        private static void Combo()
        {
            var useQ = SettingsMenu["comboQ"].Cast<CheckBox>().CurrentValue;
            var useW = SettingsMenu["comboW"].Cast<CheckBox>().CurrentValue;
            var useR = SettingsMenu["comboR"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);

            W.AllowedCollisionCount = int.MaxValue;

            if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsZombie && Q.GetPrediction(target).HitChance >= HitChance.High)
            {
                Q.Cast(target);
            }
            if (useW && W.IsReady() && target.IsValidTarget(W.Range) && !target.IsZombie && W.GetPrediction(target).HitChance >= HitChance.High)
            {
                W.Cast(target);
            }
            if (useR && R.IsReady() && target.IsValidTarget(R.Range) && !target.IsZombie && R.GetPrediction(target).HitChance >= HitChance.High
                && target.Health <= Player.Instance.GetSpellDamage(target, SpellSlot.R))
            {
                R.Cast(target);
            }
        }

        private static void Harass()
        {

            var useQ = SettingsMenu["harassQ"].Cast<CheckBox>().CurrentValue;
            var useW = SettingsMenu["harassW"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(1300, DamageType.Physical);

            if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsZombie && Q.GetPrediction(target).HitChance >= HitChance.Medium)
            {
                Q.Cast(target);
            }
            if (useW && W.IsReady() && target.IsValidTarget(W.Range) && !target.IsZombie && W.GetPrediction(target).HitChance >= HitChance.Medium)
            {
                W.Cast(target);
            }
        }

        private static void LastHit()
        {
            var useQ = SettingsMenu["lasthitQ"].Cast<CheckBox>().CurrentValue;
            var mana = SettingsMenu["lasthitMana"].Cast<Slider>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Minion>().OrderBy(m => m.Health).Where(m => m.IsEnemy);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && !minion.IsValidTarget(_Player.AttackRange) 
                    && Player.Instance.ManaPercent > mana && minion.Health <= Player.Instance.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast(minion);
                }
            }

        }
        private static void LaneClear()
        {
            var useQ = SettingsMenu["laneclearQ"].Cast<CheckBox>().CurrentValue;
            var mana = SettingsMenu["laneclearMana"].Cast<Slider>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Minion>().OrderBy(m => m.Health).Where(m => m.IsEnemy);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && !minion.IsValidTarget(_Player.AttackRange)
                    && Player.Instance.ManaPercent > mana && minion.Health <= Player.Instance.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast(minion);
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_Player.IsDead) return;

            var drawQ = SettingsMenu["Qd"].Cast<CheckBox>().CurrentValue;
            var drawW = SettingsMenu["Wd"].Cast<CheckBox>().CurrentValue;
            var drawR = SettingsMenu["Rd"].Cast<CheckBox>().CurrentValue;

            if (drawQ)
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = Q.Range }.Draw(_Player.Position);
            }
            if (drawW)
            {
                new Circle() { Color = Color.Black, BorderWidth = 1, Radius = W.Range }.Draw(_Player.Position);
            }
            if (drawR)
            {
                new Circle() { Color = Color.Pink, BorderWidth = 1, Radius = R.Range }.Draw(_Player.Position);
            }
        }
    }
}