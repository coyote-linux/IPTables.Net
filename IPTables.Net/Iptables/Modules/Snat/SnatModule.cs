﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using IPTables.Net.Iptables.DataTypes;

namespace IPTables.Net.Iptables.Modules.Snat
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicFields)]
    public class SnatModule : ModuleBase, IIpTablesModule, IEquatable<SnatModule>
    {
        private const string OptionToSource = "--to-source";
        private const string OptionRandom = "--random";
        private const string OptionPersisent = "--persistent";

        public bool Persistent = false;
        public bool Random = false;
        public IPPortOrRange ToSource = new IPPortOrRange(IPAddress.Any);

        public SnatModule(int version) : base(version)
        {
        }

        public bool Equals(SnatModule other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Persistent.Equals(other.Persistent) && Random.Equals(other.Random) &&
                   ToSource.Equals(other.ToSource);
        }

        public bool NeedsLoading => false;

        public int Feed(CommandParser parser, bool not)
        {
            switch (parser.GetCurrentArg())
            {
                case OptionToSource:
                    ToSource = IPPortOrRange.Parse(parser.GetNextArg());
                    return 1;

                case OptionRandom:
                    Random = true;
                    return 0;

                case OptionPersisent:
                    Persistent = true;
                    return 0;
            }

            return 0;
        }

        public string GetRuleString()
        {
            var sb = new StringBuilder();

            if (!Equals(ToSource.LowerAddress, IPAddress.Any))
            {
                if (sb.Length != 0)
                    sb.Append(" ");
                sb.Append(OptionToSource + " ");
                sb.Append(ToSource);
            }

            if (Random)
            {
                if (sb.Length != 0)
                    sb.Append(" ");
                sb.Append(OptionRandom);
            }

            if (Persistent)
            {
                if (sb.Length != 0)
                    sb.Append(" ");
                sb.Append(OptionPersisent);
            }

            return sb.ToString();
        }

        public static HashSet<string> GetOptions()
        {
            var options = new HashSet<string>
            {
                OptionToSource,
                OptionRandom,
                OptionPersisent
            };
            return options;
        }

        public static ModuleEntry GetModuleEntry()
        {
            return GetTargetModuleEntryInternal("SNAT", typeof(SnatModule), GetOptions, (version) => new SnatModule(version));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SnatModule) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Persistent.GetHashCode();
                hashCode = (hashCode * 397) ^ Random.GetHashCode();
                hashCode = (hashCode * 397) ^ ToSource.GetHashCode();
                return hashCode;
            }
        }
    }
}