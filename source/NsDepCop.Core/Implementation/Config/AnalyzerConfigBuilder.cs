using System.Collections.Generic;
using System.Collections.Immutable;
using Codartis.NsDepCop.Core.Interface.Config;
using Codartis.NsDepCop.Core.Util;

namespace Codartis.NsDepCop.Core.Implementation.Config
{
    /// <summary>
    /// Builds analyzer config objects.
    /// </summary>
    internal class AnalyzerConfigBuilder
    {
        private readonly bool _isParserOverridden;

        public bool? IsEnabled { get; private set; }
        public IssueKind? IssueKind { get; private set; }
        public Importance? InfoImportance { get; private set; }
        public Parsers? Parser { get; private set; }

        public bool? ChildCanDependOnParentImplicitly { get; private set; }
        public Dictionary<NamespaceDependencyRule, TypeNameSet> AllowRules { get;  }
        public HashSet<NamespaceDependencyRule> DisallowRules { get;  }
        public Dictionary<Namespace, TypeNameSet> VisibleTypesByNamespace { get;  }
        public int? MaxIssueCount { get; private set; }

        public AnalyzerConfigBuilder(Parsers? overridingParser = null)
        {
            _isParserOverridden = overridingParser.HasValue;
            Parser = overridingParser;

            AllowRules = new Dictionary<NamespaceDependencyRule, TypeNameSet>();
            DisallowRules = new HashSet<NamespaceDependencyRule>();
            VisibleTypesByNamespace = new Dictionary<Namespace, TypeNameSet>();
        }

        public IAnalyzerConfig ToAnalyzerConfig()
        {
            return new AnalyzerConfig(
                IsEnabled ?? ConfigDefaults.IsEnabled,
                IssueKind ?? ConfigDefaults.IssueKind,
                InfoImportance ?? ConfigDefaults.InfoImportance,
                Parser ?? ConfigDefaults.Parser,
                ChildCanDependOnParentImplicitly ?? ConfigDefaults.ChildCanDependOnParentImplicitly,
                AllowRules.ToImmutableDictionary(),
                DisallowRules.ToImmutableHashSet(),
                VisibleTypesByNamespace.ToImmutableDictionary(),
                MaxIssueCount ?? ConfigDefaults.MaxIssueCount
                );
        }

        public AnalyzerConfigBuilder Combine(AnalyzerConfigBuilder analyzerConfigBuilder)
        {
            SetIsEnabled(analyzerConfigBuilder.IsEnabled);
            SetIssueKind(analyzerConfigBuilder.IssueKind);
            SetInfoImportance(analyzerConfigBuilder.InfoImportance);
            SetParser(analyzerConfigBuilder.Parser);

            SetChildCanDependOnParentImplicitly(analyzerConfigBuilder.ChildCanDependOnParentImplicitly);
            AddAllowRules(analyzerConfigBuilder.AllowRules);
            AddDisallowRules(analyzerConfigBuilder.DisallowRules);
            AddVisibleTypesByNamespace(analyzerConfigBuilder.VisibleTypesByNamespace);
            SetMaxIssueCount(analyzerConfigBuilder.MaxIssueCount);

            return this;
        }

        public AnalyzerConfigBuilder SetIsEnabled(bool? isEnabled)
        {
            if (isEnabled.HasValue)
                IsEnabled = isEnabled;
            return this;
        }

        public AnalyzerConfigBuilder SetIssueKind(IssueKind? issueKind)
        {
            if (issueKind.HasValue)
                IssueKind = issueKind;
            return this;
        }

        public AnalyzerConfigBuilder SetInfoImportance(Importance? infoImportance)
        {
            if (infoImportance.HasValue)
                InfoImportance = infoImportance;
            return this;
        }

        public AnalyzerConfigBuilder SetParser(Parsers? parser)
        {
            if (parser.HasValue && !_isParserOverridden)
                Parser = parser;
            return this;
        }

        public AnalyzerConfigBuilder SetChildCanDependOnParentImplicitly(bool? childCanDependOnParentImplicitly)
        {
            if (childCanDependOnParentImplicitly.HasValue)
                ChildCanDependOnParentImplicitly = childCanDependOnParentImplicitly;
            return this;
        }

        public AnalyzerConfigBuilder AddAllowRule(NamespaceDependencyRule namespaceDependencyRule, TypeNameSet typeNameSet = null)
        {
            AllowRules.AddOrUnion<NamespaceDependencyRule, TypeNameSet, string>(namespaceDependencyRule, typeNameSet);
            return this;
        }

        private AnalyzerConfigBuilder AddAllowRules(IEnumerable<KeyValuePair<NamespaceDependencyRule, TypeNameSet>> allowRules)
        {
            foreach (var keyValuePair in allowRules)
                AddAllowRule(keyValuePair.Key, keyValuePair.Value);
            return this;
        }

        public AnalyzerConfigBuilder AddDisallowRule(NamespaceDependencyRule namespaceDependencyRule)
        {
            DisallowRules.Add(namespaceDependencyRule);
            return this;
        }

        private AnalyzerConfigBuilder AddDisallowRules(IEnumerable<NamespaceDependencyRule> disallowRules)
        {
            foreach (var namespaceDependencyRule in disallowRules)
                AddDisallowRule(namespaceDependencyRule);
            return this;
        }

        public AnalyzerConfigBuilder AddVisibleTypesByNamespace(Namespace ns, TypeNameSet typeNameSet)
        {
            VisibleTypesByNamespace.AddOrUnion<Namespace, TypeNameSet, string>(ns, typeNameSet);
            return this;
        }

        private AnalyzerConfigBuilder AddVisibleTypesByNamespace(IEnumerable<KeyValuePair<Namespace, TypeNameSet>> visibleTypesByNamespace)
        {
            foreach (var keyValuePair in visibleTypesByNamespace)
                AddVisibleTypesByNamespace(keyValuePair.Key, keyValuePair.Value);
            return this;
        }

        public AnalyzerConfigBuilder SetMaxIssueCount(int? maxIssueCount)
        {
            if (maxIssueCount.HasValue)
                MaxIssueCount = maxIssueCount;
            return this;
        }
    }
}