﻿// Copyright (c) 2014-2015 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
namespace SiliconStudio.Paradox.Physics
{
    [Flags]
    public enum CollisionFilterGroupFlags
    {
        DefaultFilter = 0x1,

        StaticFilter = 0x2,

        KinematicFilter = 0x4,

        DebrisFilter = 0x8,

        SensorTrigger = 0x10,

        CharacterFilter = 0x20,

        CustomFilter1 = 0x40,

        CustomFilter2 = 0x80,

        CustomFilter3 = 0x100,

        CustomFilter4 = 0x200,

        CustomFilter5 = 0x400,

        CustomFilter6 = 0x800,

        CustomFilter7 = 0x1000,

        CustomFilter8 = 0x2000,

        CustomFilter9 = 0x4000,

        CustomFilter10 = 0x8000,

        AllFilter = 0xFFFF
    }
}