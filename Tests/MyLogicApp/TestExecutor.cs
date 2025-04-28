using Microsoft.Azure.Workflows.UnitTesting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MyLogicApp.Tests
{
    /// <summary>
    /// Creates a UnitTestExecutor with paths that work both locally and in CI.
    /// </summary>
    public class TestExecutor
    {
        /// <summary>The workspace root that holds the Logic App project.</summary>
        private readonly string rootDirectory;

        /// <summary>The Logic App folder name (e.g. MyLogicApp).</summary>
        private readonly string logicAppName;

        /// <summary>The workflow sub-folder name (e.g. sampleWorkflow).</summary>
        private readonly string workflow;

        /// <param name="configPath">
        /// Relative path to the XML config (e.g. "sampleWorkflow/testSettings.config").
        /// </param>
        public TestExecutor(string configPath)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddXmlFile(configPath, optional: false, reloadOnChange: false)
                .Build();

            // WorkspacePath may contain the token $(WS_ROOT) or a literal path.
            var wsPathFromConfig = configuration["TestSettings:WorkspacePath"];

            // Replace token with env-var if present, otherwise fall back to assembly base dir.
            var resolvedWsPath =
                   string.Equals(wsPathFromConfig, "$(WS_ROOT)", StringComparison.OrdinalIgnoreCase)
                       ? Environment.GetEnvironmentVariable("WS_ROOT")
                          ?? AppContext.BaseDirectory
                       : wsPathFromConfig;

            // Normalise separators and remove any trailing slash.
            this.rootDirectory = Path.GetFullPath(resolvedWsPath.TrimEnd(
                                 Path.DirectorySeparatorChar,
                                 Path.AltDirectorySeparatorChar));

            this.logicAppName = configuration["TestSettings:LogicAppName"];
            this.workflow     = configuration["TestSettings:WorkflowName"];
        }

        #region Unit-test executor creation

        public UnitTestExecutor Create()
        {
            // Build full paths to the required JSON files.
            var workflowDefinitionPath = Path.Combine(
                    rootDirectory, logicAppName, workflow, "workflow.json");

            var connectionsPath  = Path.Combine(rootDirectory, logicAppName, "connections.json");
            var parametersPath   = Path.Combine(rootDirectory, logicAppName, "parameters.json");
            var localSettingsPath= Path.Combine(rootDirectory, logicAppName, "local.settings.json");

            return new UnitTestExecutor(
                workflowFilePath:     workflowDefinitionPath,
                connectionsFilePath:  connectionsPath,
                parametersFilePath:   parametersPath,
                localSettingsFilePath:localSettingsPath);
        }

        #endregion
    }
}
