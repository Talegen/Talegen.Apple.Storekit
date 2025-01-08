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
    using System.IdentityModel.Tokens.Jwt;
    using System.Text.Json;

    /// <summary>
    /// This class contains extension methods for the Apple payload.
    /// </summary>
    public static class PayloadExtensions
    {
        /// <summary>
        /// This method is used to decode the Apple payload.
        /// </summary>
        /// <typeparam name="T">Contains the data type to decode the payload JSON into.</typeparam>
        /// <param name="jwtToken">Contains the JWT payload.</param>
        /// <returns>Returns the payload model.</returns>
        public static T DecodeJwtPayload<T>(this string jwtToken, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);
            var payload = token.Payload.SerializeToJson();

            return JsonSerializer.Deserialize<T>(payload, jsonSerializerOptions);
        }

    }
}
