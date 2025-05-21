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
    using System.Threading.Tasks;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Models;
    using Talegen.Apple.Storekit.Models.Api;


    /// <summary>
    /// This class contains extension methods for consumption related operations.
    /// </summary>
    public static class ConsumptionExtensions
    {
        /// <summary>
        /// This method is used to convert a number of minutes consumed to an <see cref="AppleTimeUsed"/> enumeration.
        /// </summary>
        /// <param name="minutesConsumed">Contains the minutes.</param>
        /// <returns>Returns the <see cref="AppleTimeUsed"/> result.</returns>
        public static AppleTimeUsed ToAppleTimeUsed(this int minutesConsumed)
        {
            AppleTimeUsed result;

            if (minutesConsumed < 0)
            {
                result = AppleTimeUsed.Undeclared;
            }
            else if (minutesConsumed <= 5)
            {
                result = AppleTimeUsed.BetweenZeroAndFiveMinutes;
            }
            else if (minutesConsumed <= 60)
            {
                result = AppleTimeUsed.BetweenFiveAndSixtyMinutes;
            }
            else if (minutesConsumed <= 360)
            {
                result = AppleTimeUsed.BetweenOneAndSixHours;
            }
            else if (minutesConsumed <= 1440)
            {
                result = AppleTimeUsed.BetweenSixAndTwentyFourHours;
            }
            else if (minutesConsumed <= 5860)
            {
                result = AppleTimeUsed.BetweenOneAndFourDays;
            }
            else if (minutesConsumed <= 23040)
            {
                result = AppleTimeUsed.BetweenFourAndSixteenDays;
            }
            else
            {
                result = AppleTimeUsed.OverSixteenDays;
            }

            return result;
        }

        /// <summary>
        /// This method is used to convert a decimal amount to an <see cref="AppleDollarAmount"/> enumeration.
        /// </summary>
        /// <param name="amount">Contains the amount to evaluate.</param>
        /// <returns>Returns the <see cref="AppleDollarAmount"/> result.</returns>
        public static AppleDollarAmount ToAppleDollarAmount(this decimal amount)
        {
            AppleDollarAmount result;
            var absAmount = Math.Abs(amount);

            if (absAmount == 0M)
            {
                result = AppleDollarAmount.ZeroDollars;
            }
            else if (absAmount <= 49.99M)
            {
                result = AppleDollarAmount.BetweenOneCentAnd50Dollars;
            }
            else if (absAmount <= 99.99M)
            {
                result = AppleDollarAmount.Between50And100Dollars;
            }
            else if (absAmount <= 499.99M)
            {
                result = AppleDollarAmount.Between100And500Dollars;
            }
            else if (absAmount <= 999.99M)
            {
                result = AppleDollarAmount.Between500And1000Dollars;
            }
            else if (absAmount <= 1999.99M)
            {
                result = AppleDollarAmount.Between1000And2000Dollars;
            }
            else
            {
                result = AppleDollarAmount.Over2000Dollars;
            }

            return result;
        }
        
        /// <summary>
        /// This method will return the Apple account tenure based on the created date time.
        /// </summary>
        /// <param name="createdDateTime">Contains the account created date time in UTC.</param>
        /// <returns>Returns the appropriate <see cref="AppleAccountTenure"/> value.</returns>
        public static AppleAccountTenure ToAppleAccountTenure(this DateTime createdDateTime)
        {
            AppleAccountTenure result;

            int days = (DateTime.UtcNow - createdDateTime).Days;
            
            if (days <= 3)
            {
                result = AppleAccountTenure.BetweenZeroAndThreeDays;
            }
            else if (days <= 10)
            {
                result = AppleAccountTenure.BetweenThreeAndTenDays;
            }
            else if (days <= 30)
            {
                result = AppleAccountTenure.BetweenTenAndThirtyDays;
            }
            else if (days <= 90)
            {
                result = AppleAccountTenure.BetweenThirtyAndNinetyDays;
            }
            else if (days <= 180)
            {
                result = AppleAccountTenure.BetweenNinetyAndOneHundredEightyDays;
            }
            else if (days <= 365)
            {
                result = AppleAccountTenure.BetweenOneHundredEightyAndThreeHundredSixtyFiveDays;
            }
            else
            {
                result = AppleAccountTenure.OverThreeHundredSixtyFiveDays;
            }

            return result;
        }

        /// <summary>
        /// This method is used to send consumption information about a consumable 
        /// in-app purchase or auto-renewable subscription to the App Store after 
        /// your server receives a consumption request notification.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="transactionId">Contains a transaction identifier.</param>
        /// <param name="environment">Contains the environment override to use for the request.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns></returns>
        public static async Task SendConsumptionInfoAsync(this IAppStoreServerApiClient client, string transactionId, ConsumptionRequest request, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(nameof(client));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(transactionId));
            string endpoint = $"/inApps/v1/transactions/consumption/{transactionId}";
            await client.MakeRequest(endpoint, HttpMethod.Put, requestBody: request, environment: environment, cancellationToken: cancellationToken);
        }
    }
}
