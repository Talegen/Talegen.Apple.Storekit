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
    using System.Text.Json;
    using Microsoft.IdentityModel.Tokens;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Models.Api;

    /// <summary>
    /// This class contains extension methods for the Apple payload.
    /// </summary>
    public static class PayloadExtensions
    {
        /// <summary>
        /// This method is used to decode the payload of an Apple JWS (JSON Web Signature) string.
        /// </summary>
        /// <typeparam name="T">Contains the data type to decode the payload JSON into.</typeparam>
        /// <param name="jwtToken">Contains the JWS compact serialization.</param>
        /// <param name="jsonSerializerOptions">Contains optional serializer options.</param>
        /// <returns>Returns the payload model.</returns>
        /// <remarks>
        /// <b>This method does not verify the signature.</b> Use it only for data you received directly from the
        /// App Store Server API over TLS. For data that reaches you from anywhere else (App Store Server Notifications,
        /// or transactions sent up from a device), use <see cref="Client.SignedDataVerifier"/> instead.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when the value is not a JWS compact serialization.</exception>
        public static T DecodeJwtPayload<T>(this string jwtToken, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(jwtToken);

            string[] parts = jwtToken.Split('.');

            if (parts.Length != 3)
            {
                throw new ArgumentException("The value is not a JWS compact serialization.", nameof(jwtToken));
            }

            byte[] payload;

            try
            {
                payload = Base64UrlEncoder.DecodeBytes(parts[1]);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("The JWS payload is not valid base64url.", nameof(jwtToken), ex);
            }

            return JsonSerializer.Deserialize<T>(payload, jsonSerializerOptions ?? AppStoreServerApiClient.JsonOptions)!;
        }

        /// <summary>
        /// This method decodes a notification request body <b>without verifying its signature</b>.
        /// </summary>
        /// <param name="body">Contains the notification body posted by the App Store.</param>
        /// <returns>Returns the decoded notification payload.</returns>
        /// <remarks>Prefer <see cref="Client.SignedDataVerifier.VerifyAndDecodeNotification(string)"/> in production.</remarks>
        public static AppleResponseBodyV2DecodedPayload DecodeUnverified(this AppleResponseBodyV2 body)
        {
            ArgumentNullException.ThrowIfNull(body);
            return body.SignedPayload.DecodeJwtPayload<AppleResponseBodyV2DecodedPayload>();
        }

        /// <summary>
        /// This method converts a nullable UNIX time in milliseconds to a UTC <see cref="DateTime"/>.
        /// </summary>
        /// <param name="unixTimeMilliseconds">Contains the UNIX time in milliseconds.</param>
        /// <returns>Returns the UTC date time, or <c>null</c> when no value is provided.</returns>
        public static DateTime? ToUtcDateTime(this long? unixTimeMilliseconds)
        {
            return unixTimeMilliseconds.HasValue
                ? DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds.Value).UtcDateTime
                : null;
        }
    }
}
