using Microsoft.Xrm.Sdk;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PluginTest
{
    /// <summary>
    /// Plugin development guide: https://docs.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://docs.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>
    public class Plugin1 : PluginBase
    {
        public Plugin1() : base(typeof(Plugin1))
        {

            
        }

        public Plugin1(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(Plugin1))
        {
            // TODO: Implement your custom configuration handling
            // https://docs.microsoft.com/powerapps/developer/common-data-service/register-plug-in#set-configuration-data
        }        // Entry point for custom business logic execution
        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            localPluginContext.Trace("ExecuteDataversePlugin: Plugin execution started");

            if (localPluginContext == null)
            {
                throw new ArgumentNullException(nameof(localPluginContext));
            }

            var context = localPluginContext.PluginExecutionContext;
            localPluginContext.Trace($"ExecuteDataversePlugin: Message name: {context.MessageName}");

            // Check if this is an update operation on a contact
            if (context.MessageName.ToUpper() != "UPDATE")
            {
                localPluginContext.Trace("ExecuteDataversePlugin: Not an UPDATE operation, exiting");
                return;
            }

            localPluginContext.Trace("ExecuteDataversePlugin: Processing UPDATE operation");

            // Check for the Target entity parameter
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                var entity = (Entity)context.InputParameters["Target"];
                localPluginContext.Trace($"ExecuteDataversePlugin: Target entity found - LogicalName: {entity.LogicalName}");

                // Check if this is a contact entity
                if (entity.LogicalName == "contact")
                {
                    localPluginContext.Trace("ExecuteDataversePlugin: Contact entity detected, calling FormatMobilePhone");
                    try
                    {
                        FormatMobilePhone(entity, localPluginContext);
                        localPluginContext.Trace("ExecuteDataversePlugin: FormatMobilePhone completed successfully");
                    }
                    catch (Exception ex)
                    {
                        localPluginContext.Trace($"ExecuteDataversePlugin: Exception in FormatMobilePhone: {ex.GetType().Name} - {ex.Message}");
                        localPluginContext.Trace($"ExecuteDataversePlugin: Stack trace: {ex.StackTrace}");

                        var remote = localPluginContext.PluginExecutionContext.ToRemoteExecutionContext();
                        localPluginContext.TracingService.Trace("PluginExecutionContext: " + remote.ToJson());

                        throw; // Re-throw to maintain original behavior
                    }
                }
                else
                {
                    localPluginContext.Trace($"ExecuteDataversePlugin: Not a contact entity (LogicalName: {entity.LogicalName}), skipping");
                }
            }
            else
            {
                localPluginContext.Trace("ExecuteDataversePlugin: No Target entity parameter found or invalid type");
            }

            localPluginContext.Trace("ExecuteDataversePlugin: Plugin execution completed");
        }/// <summary>
         /// Formats the mobile phone number to have a nice appearance with spaces
         /// </summary>
         /// <param name="entity">The contact entity</param>
         /// <param name="localPluginContext">The plugin context</param>
        private void FormatMobilePhone(Entity entity, ILocalPluginContext localPluginContext)
        {
            localPluginContext.Trace("FormatMobilePhone: Starting phone formatting process");

            // Check if the mobilephone field is being updated
            if (!entity.Contains("mobilephone"))
            {
                localPluginContext.Trace("FormatMobilePhone: Mobile phone field not present in update, skipping");
                return;
            }

            var mobilePhone = entity.GetAttributeValue<string>("mobilephone");
            localPluginContext.Trace($"FormatMobilePhone: Retrieved mobile phone value: '{mobilePhone}'");

            // Skip if mobile phone is null or empty
            if (string.IsNullOrWhiteSpace(mobilePhone))
            {
                localPluginContext.Trace("FormatMobilePhone: Mobile phone is null or empty, skipping formatting");
                return;
            }

            localPluginContext.Trace($"FormatMobilePhone: Original mobile phone: {mobilePhone}");

            try
            {
                localPluginContext.Trace("FormatMobilePhone: Calling FormatPhoneNumber method");
                // Format the phone number
                var formattedPhone = FormatPhoneNumber(mobilePhone, localPluginContext);
                localPluginContext.Trace($"FormatMobilePhone: FormatPhoneNumber returned: '{formattedPhone}'");

                if (formattedPhone != mobilePhone)
                {
                    entity["mobilephone"] = formattedPhone;
                    localPluginContext.Trace($"FormatMobilePhone: Updated mobile phone to: {formattedPhone}");
                }
                else
                {
                    localPluginContext.Trace("FormatMobilePhone: No formatting changes needed");
                }
            }
            catch (Exception ex)
            {
                localPluginContext.Trace($"FormatMobilePhone: Exception occurred during formatting: {ex.GetType().Name} - {ex.Message}");
                throw; // Re-throw to maintain original behavior
            }
        }        /// <summary>
                 /// Formats a phone number string to look nice with spaces and proper formatting
                 /// Supports various formats and tries to create a consistent, readable format
                 /// </summary>
                 /// <param name="phoneNumber">The raw phone number string</param>
                 /// <param name="localPluginContext">The plugin context for tracing</param>
                 /// <returns>Formatted phone number</returns>
        private string FormatPhoneNumber(string phoneNumber, ILocalPluginContext localPluginContext)
        {
            localPluginContext.Trace($"FormatPhoneNumber: Starting with input: '{phoneNumber}'");

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                localPluginContext.Trace("FormatPhoneNumber: Input is null or whitespace, returning as-is");
                return phoneNumber;
            }

            localPluginContext.Trace("FormatPhoneNumber: Cleaning phone number - removing non-digit characters");
            // Remove all non-digit characters except + at the beginning
            var cleanNumber = string.Empty;
            var hasCountryCode = phoneNumber.TrimStart().StartsWith("+");
            localPluginContext.Trace($"FormatPhoneNumber: Has country code detected: {hasCountryCode}");

            foreach (char c in phoneNumber)
            {
                if (char.IsDigit(c))
                    cleanNumber += c;
                else if (c == '+' && cleanNumber.Length == 0)
                    cleanNumber += c;
            }

            localPluginContext.Trace($"FormatPhoneNumber: Cleaned number: '{cleanNumber}' (length: {cleanNumber.Length})");

            var digitOnlyLength = cleanNumber.Replace("+", "").Length;
            localPluginContext.Trace($"FormatPhoneNumber: Digit-only length: {digitOnlyLength}");

            // Check if we have any digits at all after cleaning
            if (digitOnlyLength == 0)
            {
                localPluginContext.Trace("FormatPhoneNumber: No digits found after cleaning, returning empty string");
                return string.Empty;
            }

            try
            {
                // Format based on length and country code presence
                if (hasCountryCode)
                {
                    localPluginContext.Trace("FormatPhoneNumber: Processing international format");
           
                    if (cleanNumber.Length >= 10)
                    {
                        localPluginContext.Trace("FormatPhoneNumber: Clean number length >= 10, proceeding with international formatting");
                        // Example: +1 234 567 8900 or +44 20 1234 5678
                        localPluginContext.Trace($"FormatPhoneNumber: About to extract country code from: '{cleanNumber}'");
                        var countryCode = cleanNumber.Substring(0, cleanNumber.StartsWith("+1") ? 2 : 3);
                        localPluginContext.Trace($"FormatPhoneNumber: Extracted country code: '{countryCode}'");

                        var remaining = cleanNumber.Substring(countryCode.Length);
                        localPluginContext.Trace($"FormatPhoneNumber: Remaining digits after country code: '{remaining}' (length: {remaining.Length})");

                        if (remaining.Length == 10) // US/Canada format
                        {
                            localPluginContext.Trace("FormatPhoneNumber: Formatting as US/Canada format");
                            return $"{countryCode} ({remaining.Substring(0, 3)}) {remaining.Substring(3, 3)}-{remaining.Substring(6)}";
                        }
                        else if (remaining.Length >= 8) // International format
                        {
                            localPluginContext.Trace("FormatPhoneNumber: Formatting as international format");
                            return $"{countryCode} {remaining.Substring(0, 2)} {remaining.Substring(2, 4)} {remaining.Substring(6)}";
                        }
                    }
                    // If international format but too short, return as-is
                    localPluginContext.Trace($"FormatPhoneNumber: International number too short ({cleanNumber.Length} chars), returning as-is");
                    return cleanNumber;
                }
                else
                {
                    localPluginContext.Trace("FormatPhoneNumber: Processing domestic format");

                    if (cleanNumber.Length == 10)
                    {
                        localPluginContext.Trace("FormatPhoneNumber: Formatting as 10-digit US format");
                        // US format: (123) 456-7890
                        return $"({cleanNumber.Substring(0, 3)}) {cleanNumber.Substring(3, 3)}-{cleanNumber.Substring(6)}";
                    }
                    else if (cleanNumber.Length == 7)
                    {
                        localPluginContext.Trace("FormatPhoneNumber: Formatting as 7-digit local format");
                        // Local format: 123-4567
                        return $"{cleanNumber.Substring(0, 3)}-{cleanNumber.Substring(3)}";
                    }
                    else
                    {
                        localPluginContext.Trace($"FormatPhoneNumber: Number has {cleanNumber.Length} digits, not standard length - returning as-is");
                        // Return the cleaned number as-is for non-standard lengths
                        return cleanNumber;
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                localPluginContext.Trace($"FormatPhoneNumber: ArgumentOutOfRangeException caught - {ex.Message}");
                localPluginContext.Trace($"FormatPhoneNumber: Exception details - cleanNumber: '{cleanNumber}', length: {cleanNumber.Length}");
                throw; // Re-throw to maintain original behavior
            }
            catch (Exception ex)
            {
                localPluginContext.Trace($"FormatPhoneNumber: Unexpected exception: {ex.GetType().Name} - {ex.Message}");
                throw; // Re-throw to maintain original behavior
            }
        }


    }
    
}
