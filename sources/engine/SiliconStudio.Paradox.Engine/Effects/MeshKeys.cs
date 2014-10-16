﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
namespace SiliconStudio.Paradox.Effects
{
    /// <summary>
    /// Defines keys associated with mesh used for compiling assets.
    /// </summary>
    public class MeshKeys
    {
        /// <summary>
        /// When compiling effect with an EffectLibraryAsset (pdxfxlib), set it to true to allow permutation based on the 
        /// parameters of all meshes.
        /// </summary>
        /// TODO: allow permutation for a specific mesh
        public static readonly ParameterKey<bool> UseParameters = ParameterKeys.New<bool>();
    }
}