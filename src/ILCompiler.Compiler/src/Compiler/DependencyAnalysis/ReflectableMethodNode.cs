﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

using ILCompiler.DependencyAnalysisFramework;

using Internal.TypeSystem;

using Debug = System.Diagnostics.Debug;

namespace ILCompiler.DependencyAnalysis
{
    /// <summary>
    /// Represents a method that doesn't have a body, but we need to track it because it's reflectable.
    /// </summary>
    internal class ReflectableMethodNode : DependencyNodeCore<NodeFactory>
    {
        private MethodDesc _method;

        public ReflectableMethodNode(MethodDesc method)
        {
            Debug.Assert(method.IsAbstract || method.IsPInvoke);
            _method = method;
        }

        public MethodDesc Method => _method;

        public override IEnumerable<DependencyListEntry> GetStaticDependencies(NodeFactory factory)
        {
            DependencyList dependencies = null;
            CodeBasedDependencyAlgorithm.AddDependenciesDueToReflectability(ref dependencies, factory, _method);
            return dependencies;
        }
        protected override string GetName(NodeFactory factory)
        {
            return "Reflectable method: " + _method.ToString();
        }

        public override bool InterestingForDynamicDependencyAnalysis => false;
        public override bool HasDynamicDependencies => false;
        public override bool HasConditionalStaticDependencies => false;
        public override bool StaticDependenciesAreComputed => true;
        public override IEnumerable<CombinedDependencyListEntry> GetConditionalStaticDependencies(NodeFactory factory) => null;
        public override IEnumerable<CombinedDependencyListEntry> SearchDynamicDependencies(List<DependencyNodeCore<NodeFactory>> markedNodes, int firstNode, NodeFactory factory) => null;
    }
}