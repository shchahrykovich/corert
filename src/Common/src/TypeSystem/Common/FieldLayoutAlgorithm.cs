// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Internal.TypeSystem
{
    /// <summary>
    /// Pluggable field layout algorithm. Provides means to compute static/instance sizes for types,
    /// offsets for their fields and other type information that depends on type's fields.
    /// The information computed by this algorithm is exposed on various properties of
    /// <see cref="DefType"/> and <see cref="FieldDesc"/>.
    /// </summary>
    /// <remarks>
    /// The algorithms are expected to be directly used by <see cref="TypeSystemContext"/> derivatives
    /// only. The most obvious implementation of this algorithm that uses type's metadata to
    /// compute the answers is in <see cref="MetadataFieldLayoutAlgorithm"/>.
    /// </remarks>
    public abstract class FieldLayoutAlgorithm
    {
        /// <summary>
        /// Compute the instance field layout for a DefType. Must not depend on static field layout for any other type.
        /// </summary>
        public abstract ComputedInstanceFieldLayout ComputeInstanceLayout(DefType type, InstanceLayoutKind layoutKind);

        /// <summary>
        /// Compute the static field layout for a DefType. Must not depend on static field layout for any other type.
        /// </summary>
        public abstract ComputedStaticFieldLayout ComputeStaticFieldLayout(DefType type, StaticLayoutKind layoutKind);

        /// <summary>
        /// Compute whether the fields of the specified type contain a GC pointer.
        /// </summary>
        public abstract bool ComputeContainsGCPointers(DefType type);

        /// <summary>
        /// Compute the shape of a valuetype. The shape information is used to control code generation and allocation
        /// (such as vectorization, passing the valuetype by value across method calls, or boxing alignment).
        /// </summary>
        public abstract ValueTypeShapeCharacteristics ComputeValueTypeShapeCharacteristics(DefType type);

        /// <summary>
        /// If the type has <see cref="ValueTypeShapeCharacteristics.HomogenousFloatAggregate"/> characteristic, returns
        /// the element type of the homogenous float aggregate. This will either be System.Double or System.Float.
        /// </summary>
        public abstract DefType ComputeHomogeneousFloatAggregateElementType(DefType type);

        /// <summary>
        /// Compute whether '<paramref name="type"/>' is a ByRef-like value type (TypedReference, Span&lt;T&gt;, etc.).
        /// </summary>
        public abstract bool ComputeIsByRefLike(DefType type);
    }

    /// <summary>
    /// Specifies the level to which to compute the instance field layout.
    /// </summary>
    public enum InstanceLayoutKind
    {
        /// <summary>
        /// Compute instance sizes and alignments.
        /// </summary>
        TypeOnly,

        /// <summary>
        /// Compute instance sizes, alignments and field offsets.
        /// </summary>
        TypeAndFields
    }

    /// <summary>
    /// Specifies the level to which to compute static field layout.
    /// </summary>
    public enum StaticLayoutKind
    {
        /// <summary>
        /// Compute static region sizes.
        /// </summary>
        StaticRegionSizes,

        /// <summary>
        /// Compute static region sizes and static field offsets.
        /// </summary>
        StaticRegionSizesAndFields
    }

    public struct ComputedInstanceFieldLayout
    {
        public int PackValue;
        public LayoutInt FieldSize;
        public LayoutInt FieldAlignment;
        public LayoutInt ByteCountUnaligned;
        public LayoutInt ByteCountAlignment;

        /// <summary>
        /// If Offsets is non-null, then all field based layout is complete.
        /// Otherwise, only the non-field based data is considered to be complete
        /// </summary>
        public FieldAndOffset[] Offsets;
    }

    public struct StaticsBlock
    {
        public LayoutInt Size;
        public LayoutInt LargestAlignment;
    }

    public struct ComputedStaticFieldLayout
    {
        public StaticsBlock NonGcStatics;
        public StaticsBlock GcStatics;
        public StaticsBlock ThreadStatics;

        /// <summary>
        /// If Offsets is non-null, then all field based layout is complete.
        /// Otherwise, only the non-field based data is considered to be complete
        /// </summary>
        public FieldAndOffset[] Offsets;
    }

    /// <summary>
    /// Describes shape of a value type for code generation and allocation purposes.
    /// </summary>
    public enum ValueTypeShapeCharacteristics
    {
        None = 0x00,

        /// <summary>
        /// The structure is an aggregate of floating point values of the same type.
        /// </summary>
        HomogenousFloatAggregate = 0x01,
    }
}
