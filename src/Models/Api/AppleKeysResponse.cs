﻿/*
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
namespace Talegen.Apple.Storekit.Models.Api
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// This class represents the Apple Key Response object.
    /// </summary>
    public class AppleKeysResponse
    {
        /// <summary>
        /// Gets or sets the keys.
        /// </summary>
        [JsonPropertyName("keys")]
        public List<AppleKey> Keys { get; set; }
    }
}
