using Globe.Shared.Constants;
using Globe.Shared.Helpers;
using Microsoft.Extensions.Configuration;

namespace Globe.Shared.Extensions
{
    /// <summary>
    /// Provides extension methods for adding JSON configuration files
    /// with support for environment-specific settings.
    /// </summary>
    public static class ConfigurationExtension
    {
        /// <summary>
        /// Adds multiple JSON configuration files to the configuration builder,
        /// including environment-specific variations.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder to add files to.</param>
        /// <param name="basePath">The base directory where the configuration files are located.</param>
        /// <param name="configurationFiles">An array of configuration file names to add.</param>
        /// <param name="environmentName">The name of the environment to consider for environment-specific files.</param>
        public static void AddJsonFilesWithEnvironments(this IConfigurationBuilder configurationBuilder, string basePath, string[] configurationFiles, string environmentName)
        {
            foreach (var path in configurationFiles)
            {
                // Combine the base path with the configuration file name to get the absolute path
                string absoluteFilePath = Path.Combine(basePath, path);

                // Add the JSON file along with its environment-specific version (if it exists)
                configurationBuilder.AddJsonWithEnvironments(absoluteFilePath, environmentName);
            }
        }

        /// <summary>
        /// Adds a JSON configuration file and its environment-specific version (if it exists)
        /// to the configuration builder.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder to add the file to.</param>
        /// <param name="filePath">The absolute path of the configuration file.</param>
        /// <param name="environment">The environment name for the environment-specific configuration.</param>
        public static void AddJsonWithEnvironments(this IConfigurationBuilder configurationBuilder, string filePath, string environment)
        {
            // Check if the base JSON configuration file exists and add it to the configuration builder
            if (File.Exists(filePath))
            {
                configurationBuilder.AddJsonFile(filePath, optional: true);
            }

            // Modify the file path to include the environment name (e.g., appsettings.Development.json)
            string environmentFilePath = filePath.Replace(SystemConstants.JsonExtension, $".{StringManipulationHelper.ConvertToTitleCase(environment)}{SystemConstants.JsonExtension}");

            // Check if the environment-specific configuration file exists and add it to the configuration builder
            if (File.Exists(environmentFilePath))
            {
                configurationBuilder.AddJsonFile(environmentFilePath, optional: true);
            }
        }
    }
}
