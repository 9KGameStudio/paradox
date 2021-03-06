﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SiliconStudio.Core.Reflection
{
    /// <summary>
    /// Provides access members of a type.
    /// </summary>
    public interface ITypeDescriptor
    {
        void Initialize();

        ITypeDescriptorFactory Factory { get; }

        /// <summary>
        /// Gets the type described by this instance.
        /// </summary>
        /// <value>The type.</value>
        Type Type { get; }

        /// <summary>
        /// Gets the members of this type.
        /// </summary>
        /// <value>The members.</value>
        IEnumerable<IMemberDescriptor> Members { get; }

        /// <summary>
        /// Gets the member count.
        /// </summary>
        /// <value>The member count.</value>
        int Count { get; }

        /// <summary>
        /// Gets the category of this descriptor.
        /// </summary>
        /// <value>The category.</value>
        DescriptorCategory Category { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has members.
        /// </summary>
        /// <value><c>true</c> if this instance has members; otherwise, <c>false</c>.</value>
        bool HasMembers { get; }

        /// <summary>
        /// Gets the <see cref="IMemberDescriptor"/> with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The member.</returns>
        IMemberDescriptor this[string name] { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a compiler generated type.
        /// </summary>
        /// <value><c>true</c> if this instance is a compiler generated type; otherwise, <c>false</c>.</value>
        bool IsCompilerGenerated { get; }

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <value>The style.</value>
        DataStyle Style { get; }

        /// <summary>
        /// Determines whether this instance contains a member with the specified member name.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns><c>true</c> if this instance contains a member with the specified member name; otherwise, <c>false</c>.</returns>
        bool Contains(string memberName);
    }
}