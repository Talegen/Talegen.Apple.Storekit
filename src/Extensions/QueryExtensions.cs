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
namespace Talegen.Apple.Storekit.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    /// This class contains extension methods for converting objects to query strings.
    /// </summary>
    internal static class QueryExtensions
    {
        /// <summary>
        /// This method is used to convert an object to a query string.
        /// </summary>
        /// <param name="request">Contains the request object.</param>
        /// <param name="separator">Contains a separator for array properties.</param>
        /// <returns>Returns a query string.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToQueryString(this object request, string separator = ",")
        {
            if (request == null)
                throw new ArgumentNullException("request");

            // Get all properties on the object
            var properties = request.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.GetValue(request, null) != null)
                .ToDictionary(x => x.Name, x => x.GetValue(request, null));

            // Get names for all IEnumerable properties (excl. string)
            var enumerableProperties = properties
                .Where(x => !(x.Value is string) && x.Value is IEnumerable)
                .Select(x => x.Key)
                .ToList();

            // Concat all IEnumerable properties into a comma separated string
            foreach (var key in enumerableProperties)
            {
                var valueType = properties[key]?.GetType();

                if (valueType == null)
                {
                    continue;
                }

                var valueElemType = valueType.IsGenericType
                                        ? valueType.GetGenericArguments()[0]
                                        : valueType.GetElementType();
                
                if (valueElemType != null && valueElemType.IsPrimitive || valueElemType == typeof(string))
                {
                    var enumerable = properties[key] as IEnumerable;
#pragma warning disable CS8604 // Possible null reference argument.
                    properties[key] = string.Join(separator, enumerable.Cast<object>());
#pragma warning restore CS8604 // Possible null reference argument.
                }
            }

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join("&", properties
                .Where(x => x.Value != null)
                .Select(x => string.Concat(
                    Uri.EscapeDataString(x.Key), "=",
                    Uri.EscapeDataString(x.Value?.ToString() ?? string.Empty))));
        }

        /// <summary>
        /// This method is used to convert an object to a dictionary.
        /// </summary>
        /// <param name="request">Contains the object to convert.</param>
        /// <returns>Returns a dictionary of parameter names and values.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Dictionary<string, string> ToQueryDictionary(this object request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // Get all properties on the object
            return request.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.GetValue(request, null) != null)
                .ToDictionary(x => x.Name, x => x.GetValue(request, null)?.ToString() ?? string.Empty);
        }
    }
}
