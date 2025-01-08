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
namespace Talegen.Apple.Storekit.Client
{
    using System;
    using System.Net;

    /// <summary>
    /// This class represents an exception that is thrown when an error occurs calling the Apple API.
    /// </summary>
    public class AppleApiException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppleApiException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public AppleApiException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleApiException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AppleApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleApiException"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="innerException">The inner exception.</param>
        public AppleApiException(HttpStatusCode statusCode, Exception? innerException = null)
            : base($"Failed to call API with HttpStatusCode={statusCode}", innerException)
        {
            this.HttpStatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleApiException"/> class.
        /// </summary>
        /// <param name="httpStatusCode">The status code.</param>
        /// <param name="apiError">The API error.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AppleApiException(HttpStatusCode httpStatusCode, AppleApiError apiError, string message, Exception? innerException = null)
            : base($"Failed to call API with error=\"{message}\"", innerException)
        {
            this.HttpStatusCode = httpStatusCode;
            this.ErrorCode = apiError?.ErrorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleApiException"/> class.
        /// </summary>
        /// <param name="httpStatusCode">The status code.</param>
        /// <param name="errorCode">The API error.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AppleApiException(HttpStatusCode httpStatusCode, AppleErrorCodes errorCode, string message, Exception? innerException = null)
            : base($"Failed to call API with error=\"{message}\"", innerException)
        {
            this.HttpStatusCode = httpStatusCode;
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        public HttpStatusCode? HttpStatusCode { get; }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public AppleErrorCodes? ErrorCode { get; }

        /// <summary>
        /// Returns a string representation of the exception.
        /// </summary>
        /// <returns>Returns the string</returns>
        public override string ToString()
        {
            return $"{base.ToString()} HttpStatusCode={this.HttpStatusCode} ErrorCode={this.ErrorCode}";
        }
    }
}
