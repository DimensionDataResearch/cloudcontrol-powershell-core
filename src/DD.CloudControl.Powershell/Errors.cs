using System;
using System.IO;
using System.Management.Automation;

namespace DD.CloudControl.Powershell
{
    using Client;
    using Client.Models;

    /// <summary>
    ///     Factory methods for well-known <see cref="ErrorRecord"/>s.
    /// </summary>
    public static class Errors
    {
        /// <summary>
        ///     Create an <see cref="ErrorRecord"/> for when a connection already exists with the specified name.
        /// </summary>
        /// <param name="name">
        ///     The connection name.
        /// </param>
        /// <returns>
        ///     The configured <see cref="ErrorRecord"/>.
        /// </returns>
        public static ErrorRecord ConnectionExists(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Connection name cannot be null, empty, or entirely composed of whitespace.", nameof(name));

            return new ErrorRecord(
                new Exception($"A connection named '{name}' already exists."),
                errorId: "CloudControl.Connection.Exists",
                errorCategory: ErrorCategory.ResourceExists,
                targetObject: name
            );
        }

		/// <summary>
        ///     Create an <see cref="ErrorRecord"/> for when the ConnectionName parameter was not supplied to a Cmdlet and no default connection has been configured.
        /// </summary>
        /// <param name="cmdlet">
        ///     The calling Cmdlet.
        /// </param>
        /// <returns>
        ///     The configured <see cref="ErrorRecord"/>.
        /// </returns>
        public static ErrorRecord ConnectionRequired(PSCmdlet cmdlet)
        {
			if (cmdlet == null)
				throw new ArgumentNullException(nameof(cmdlet));

            return new ErrorRecord(
                new Exception("The ConnectionName parameter was not specified and no default connection has been configured."),
                errorId: "CloudControl.Connection.Required",
                errorCategory: ErrorCategory.InvalidArgument,
                targetObject: cmdlet.MyInvocation.MyCommand.Name
            );
        }

		/// <summary>
        ///     Create an <see cref="ErrorRecord"/> for when a connection does exist with the specified name.
        /// </summary>
        /// <param name="name">
        ///     The connection name.
        /// </param>
        /// <returns>
        ///     The configured <see cref="ErrorRecord"/>.
        /// </returns>
        public static ErrorRecord ConnectionDoesNotExist(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Connection name cannot be null, empty, or entirely composed of whitespace.", nameof(name));

            return new ErrorRecord(
                new Exception($"A connection named '{name}' does not exist."),
                errorId: "CloudControl.Connection.DoesNotExist",
                errorCategory: ErrorCategory.ObjectNotFound,
                targetObject: name
            );
        }

        /// <summary>
        ///     Create an <see cref="ErrorRecord"/> for when a resource was not found by Id.
        /// </summary>
        /// <typeparam name="TResource">
		/// 	The type of resource that was not found.
        /// </param>
        /// <param name="resourceId">
        ///     The Id of the resource that was not found.
        /// </param>
		/// <param name="message">
        ///     An optional (custom) error message.
        /// </param>
		/// <returns>
        ///     The configured <see cref="ErrorRecord"/>.
        /// </returns>
        public static ErrorRecord ResourceNotFoundById<TResource>(Guid resourceId, string message = null)
        {
            return ResourceNotFoundById<TResource>(resourceId.ToString(), message);
        }

		/// <summary>
        ///     Create an <see cref="ErrorRecord"/> for when a resource was not found by Id.
        /// </summary>
        /// <typeparam name="TResource">
		/// 	The type of resource that was not found.
        /// </param>
        /// <param name="resourceId">
        ///     The Id of the resource that was not found.
        /// </param>
		/// <param name="message">
        ///     An optional (custom) error message.
        /// </param>
		/// <returns>
        ///     The configured <see cref="ErrorRecord"/>.
        /// </returns>
        public static ErrorRecord ResourceNotFoundById<TResource>(string resourceId, string message = null)
        {
            if (String.IsNullOrWhiteSpace(resourceId))
                throw new ArgumentException("Resource Id cannot be null, empty, or entirely composed of whitespace.", nameof(resourceId));

            Type resourceType = typeof(TResource);
			string resourceTypeName = resourceType.Name;
            string resourceTypeDescription = ResourceTypes.GetDescription(resourceType);

            return new ErrorRecord(
                new Exception(message ?? $"No {resourceTypeDescription} was found with Id '{resourceId}'."),
                errorId: $"CloudControl.{resourceTypeName}.NotFound",
                errorCategory: ErrorCategory.ObjectNotFound,
                targetObject: resourceId
            );
        }

		/// <summary>
        ///     Create an <see cref="ErrorRecord"/> for when a resource was not found with the specified name.
        /// </summary>
        /// <param name="resourceType">
        ///     The type of resource that was not found.
        /// </param>
        /// <param name="resourceName">
        ///     The name of the resource that was not found.
        /// </param>
		/// <param name="message">
        ///     An optional (custom) error message.
        /// </param>
        /// <returns>
        ///     The configured <see cref="ErrorRecord"/>.
        /// </returns>
        public static ErrorRecord ResourceNotFoundByName<TResource>(string resourceName, string message = null)
        {
			if (String.IsNullOrWhiteSpace(resourceName))
                throw new ArgumentException("Resource name cannot be null, empty, or entirely composed of whitespace.", nameof(resourceName));

			Type resourceType = typeof(TResource);
			string resourceTypeName = resourceType.Name;
            string resourceTypeDescription = ResourceTypes.GetDescription(resourceType);

            return new ErrorRecord(
                new Exception(message ?? $"No {resourceTypeDescription} named '{resourceName}' was found."),
                errorId: $"CloudControl.{resourceTypeName}.NotFound",
                errorCategory: ErrorCategory.ObjectNotFound,
                targetObject: resourceName
            );
        }

        /// <summary>
		///		Create an <see cref="ErrorRecord"/> for when an unrecognised parameter set is encountered by a Cmdlet.
		/// </summary>
		/// <param name="cmdlet">
		///		The Cmdlet.
		/// </param>
		/// <returns>
		///		The configured <see cref="ErrorRecord"/>.
		/// </returns>
		public static ErrorRecord UnrecognizedParameterSet(PSCmdlet cmdlet)
		{
			if (cmdlet == null)
				throw new ArgumentNullException(nameof(cmdlet));

			return new ErrorRecord(
				new ArgumentException($"Unrecognised parameter-set: '{cmdlet.ParameterSetName}'."),
				errorId: "UnrecognisedParameterSet",
				errorCategory: ErrorCategory.InvalidArgument,
				targetObject: cmdlet.ParameterSetName
			);
		}

		/// <summary>
		///		Create an <see cref="ErrorRecord"/> for when requested functionality is not implemented.
		/// </summary>
		/// <param name="messageOrFormat">
		///		A message or message format specifier describing what is not implemented (and why).
		/// </param>
		/// <param name="formatArguments">
		///		Optional message format arguments.
		/// </param>
		/// <exception cref="ArgumentException">
		///		<paramref name="messageOrFormat"/> is <c>null</c>, empty, or entirely composed of whitespace.
		/// </exception>
		/// <returns>
		///		The configured <see cref="ErrorRecord"/>.
		/// </returns>
		public static ErrorRecord NotImplemented(string messageOrFormat, params object[] formatArguments)
		{
			if (String.IsNullOrWhiteSpace(messageOrFormat))
				throw new ArgumentException("Argument cannot be null, empty, or composed entirely of whitespace: 'messageOrFormat'.", nameof(messageOrFormat));

            string message = String.Format(messageOrFormat, formatArguments);

			return new ErrorRecord(
				new NotImplementedException(message),
				errorId: "NotImplemented",
				errorCategory: ErrorCategory.NotImplemented,
				targetObject: null
			);
		}

		/// <summary>
		///		Create an <see cref="ErrorRecord"/> for when a file was not found.
		/// </summary>
		/// <param name="file">
		///		A <see cref="FileInfo"/> representing the file.
		/// </param>
		/// <param name="description">
		///		A short description (sentence fragment) of the file that was not found.
		/// </param>
		/// <param name="errorCodePrefix">
		///		An optional string to prepend to the error code.
		/// </param>
		/// <returns>
		///		The configured <see cref="ErrorRecord"/>.
		/// </returns>
		public static ErrorRecord FileNotFound(FileInfo file, string description, string errorCodePrefix = "")
		{
			if (file == null)
				throw new ArgumentNullException(nameof(file));

			if (String.IsNullOrWhiteSpace(description))
				throw new ArgumentException("Argument cannot be null, empty, or composed entirely of whitespace: 'description'.", nameof(description));

			return new ErrorRecord(
				new FileNotFoundException($"Cannot find {description} file '{file.FullName}'.",
					fileName: file.FullName
				),
				errorId: errorCodePrefix + "FileNotFound",
				errorCategory: ErrorCategory.ObjectNotFound,
				targetObject: file
			);
		}

        /// <summary>
        ///     Create an <see cref="ErrorRecord"/> representing an error from the CloudControl API.
        /// </summary>
        /// <param name="client">
        ///     The CloudControl API client.
        /// </param>
        /// <param name="apiResponse">
        ///     The API response from CloudControl.
        /// </param>
        /// <returns>
        ///     The configured <see cref="ErrorRecord"/>.
        /// </returns>
        public static ErrorRecord CloudControlApi(CloudControlClient client, ApiResponseV2 apiResponse)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            if (apiResponse == null)
                throw new ArgumentNullException(nameof(apiResponse));

            return new ErrorRecord(
                exception: CloudControlException.FromApiV2Response(apiResponse, statusCode: 0),
                errorId: $"CloudControl.API.{apiResponse.ResponseCode}",
                errorCategory: ErrorCategory.NotSpecified,
                targetObject: client.BaseAddress?.AbsoluteUri ?? "Unknown"
            );
        }
    }
}