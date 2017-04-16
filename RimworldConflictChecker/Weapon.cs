using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimworldConflictChecker
{
    public class Weapon
    {
        public Weapon()
        {
            //defName = name;
        }

        public string ModName { get; set; }
        public bool ModEnabled { get; set; }
        public string ParentName { get; set; }
        public string DefName { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public float AccuracyTouch { get; set; }
        public float AccuracyShort { get; set; }
        public float AccuracyMedium { get; set; }
        public float AccuracyLong { get; set; }
        public float RangedWeapon_Cooldown { get; set; }
        public string ProjectileDef { get; set; }
        public float WarmupTime { get; set; }
        public float Range { get; set; }
        public float BurstShotCount { get; set; }
        public float TicksBetweenBurstShots { get; set; }
        public float Forcedmiss { get; set; }
        public float Damage { get; set; } //from projectile
        public float Dps_touch
        {
            get { return ((Damage * BurstShotCount) / (WarmupTime + RangedWeapon_Cooldown + (BurstShotCount * (TicksBetweenBurstShots / 60)))) * AccuracyTouch; }
        }

        //public void test()
        //{
        //    var item = new Weapon();
        //    item.ModEnabled = true;
        //    item.ModEnabled2 = true;
        //}
    }
}
