using System;

namespace CommonConfigGen
{
    /// <summary>
    /// Apply to a partial ScriptableObject-derived class to auto-generate
    /// public read-only properties for all eligible private fields.
    /// Each class in the hierarchy should carry this attribute independently;
    /// only fields declared directly on the attributed class are generated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class CommonConfigAttribute : Attribute { }

    /// <summary>
    /// Apply to a private field to exclude it from getter generation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SkipAttribute : Attribute { }

    /// <summary>
    /// Single source of truth for attribute FQNs.
    /// Uses nameof() against the real classes above so renaming propagates automatically.
    /// Shared with the generator project via a project reference.
    /// </summary>
    public static class CommonConfigConstants
    {
        public const string Namespace = nameof(CommonConfigGen);
        public const string GenerateAttrName = nameof(CommonConfigAttribute);
        public const string SkipAttrName = nameof(SkipAttribute);
        public const string GenerateAttrFqn = Namespace + "." + GenerateAttrName;
        public const string SkipAttrFqn = Namespace + "." + SkipAttrName;
    }
}
