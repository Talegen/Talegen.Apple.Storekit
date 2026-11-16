/*
 * Talegen Apple Storekit Library
 * (c) Copyright Talegen, LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/
namespace Talegen.Apple.Storekit.Models.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// A JSON converter for string-valued enumerations that honors <see cref="EnumMemberAttribute"/> values
    /// (e.g. <c>"DID_CHANGE_RENEWAL_PREF"</c>), which <see cref="JsonStringEnumConverter"/> does not support on .NET 8.
    /// </summary>
    /// <typeparam name="TEnum">Contains the enumeration type.</typeparam>
    /// <remarks>
    /// Reading is case-insensitive and also accepts the member name or its numeric value. When Apple sends a value
    /// that the library does not know yet, the enumeration's <c>Unknown</c> member is returned if one is defined,
    /// so new Apple values do not break notification processing; otherwise a <see cref="JsonException"/> is thrown.
    /// </remarks>
    public sealed class EnumMemberJsonConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : struct, Enum
    {
        private static readonly Dictionary<string, TEnum> ReadMap = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<TEnum, string> WriteMap = new Dictionary<TEnum, string>();
        private static readonly TEnum? UnknownValue;

        static EnumMemberJsonConverter()
        {
            foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var value = (TEnum)field.GetValue(null)!;
                string? memberValue = field.GetCustomAttribute<EnumMemberAttribute>()?.Value;
                bool isObsolete = field.GetCustomAttribute<ObsoleteAttribute>() != null;

                if (!string.IsNullOrEmpty(memberValue))
                {
                    ReadMap.TryAdd(memberValue, value);
                }

                ReadMap.TryAdd(field.Name, value);

                if (!isObsolete && !WriteMap.ContainsKey(value))
                {
                    WriteMap[value] = memberValue ?? field.Name;
                }

                if (field.Name == "Unknown")
                {
                    UnknownValue = value;
                }
            }
        }

        /// <inheritdoc />
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? text = reader.GetString();

                if (text != null && ReadMap.TryGetValue(text, out var value))
                {
                    return value;
                }

                if (UnknownValue.HasValue)
                {
                    return UnknownValue.Value;
                }

                throw new JsonException($"The value \"{text}\" is not a valid {typeof(TEnum).Name}.");
            }

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out long number))
            {
                return (TEnum)Enum.ToObject(typeof(TEnum), number);
            }

            throw new JsonException($"Unexpected token {reader.TokenType} when reading {typeof(TEnum).Name}.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            if (WriteMap.TryGetValue(value, out var text))
            {
                writer.WriteStringValue(text);
            }
            else
            {
                writer.WriteNumberValue(Convert.ToInt64(value));
            }
        }
    }
}
