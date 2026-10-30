using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Models;

namespace Squora.Forms.Blazor.Utillities
{
    /// <summary>
    /// Resolves the state a rule set puts a component in. Same semantics as the React renderer:
    /// HIDE/DISABLE apply when the condition matches, SHOW/ENABLE apply only while it matches.
    /// A missing value, or a condition that cannot be evaluated, does not match.
    /// </summary>
    internal static class RuleEvaluator
    {
        /// <returns>HIDE when hidden (wins over everything), DISABLE when shown but disabled, otherwise SHOW.</returns>
        internal static RuleEffect Resolve(IEnumerable<DynamicFormModelRule>? rules, object? data)
        {
            if (rules == null) { return RuleEffect.SHOW; }

            var root = ToJObject(data);
            var disabled = false;
            foreach (var rule in rules)
            {
                var matches = Matches(rule, root);
                switch (rule.Effect)
                {
                    case RuleEffect.HIDE when matches:
                    case RuleEffect.SHOW when !matches:
                        return RuleEffect.HIDE;
                    case RuleEffect.DISABLE when matches:
                    case RuleEffect.ENABLE when !matches:
                        disabled = true;
                        break;
                }
            }
            return disabled ? RuleEffect.DISABLE : RuleEffect.SHOW;
        }

        private static bool Matches(DynamicFormModelRule rule, JObject root)
        {
            try
            {
                var value = root.SelectToken(JsonSchemaPathHelpers.ConvertSchemaPathToJsonPath(rule.Condition.Scope));
                if (value == null) { return false; }
                return IsValid(value, rule.Condition.Schema.ToString());
            }
            catch (Exception ex) when (ex is JsonException or JSchemaException or ArgumentException)
            {
                return false;
            }
        }

        // Every component evaluates its rules on every render, and the schema library only allows 1000 validations an hour without a licence:
        // past that every validation fails, which showed as rules that stopped reacting. The answer is the same for the same schema and value,
        // so it is asked once.
        private const int MaxCached = 5000;
        private static readonly ConcurrentDictionary<(string Schema, string Value), bool> cache = new();
        private static readonly ConcurrentDictionary<string, JSchema> schemas = new();

        private static bool IsValid(JToken value, string schema)
        {
            var key = (schema, value.ToString(Formatting.None));
            if (cache.TryGetValue(key, out var known)) { return known; }

            var valid = value.IsValid(schemas.GetOrAdd(schema, s => JSchema.Parse(s)));
            if (cache.Count >= MaxCached) { cache.Clear(); }
            cache[key] = valid;
            return valid;
        }

        private static JObject ToJObject(object? data)
        {
            if (data == null) { return new JObject(); }
            return JObject.Parse(JsonConvert.SerializeObject(data));
        }
    }
}
