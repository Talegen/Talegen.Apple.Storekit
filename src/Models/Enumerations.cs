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
namespace Talegen.Apple.Storekit.Models
{
    /// <summary>
    /// This enumeration defines the environment types for the Apple App Store API.
    /// </summary>
    public enum EnvironmentType
    {
        /// <summary>
        /// Contains the Production environment.
        /// </summary>
        Production,

        /// <summary>
        /// Contains the Sandbox environment.
        /// </summary>
        Sandbox,

        /// <summary>
        /// Contains the Local Testing environment.
        /// </summary>
        LocalTesting
    }
}
