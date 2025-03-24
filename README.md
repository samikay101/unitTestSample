📘 How to Run Tests in a CI/CD Pipeline using VSTest in Azure DevOps

Here’s a **clear, step-by-step documentation section** that explains:

---

## 🤔 What is `VSTest@3`?

`VSTest@3` is a built-in task in **Azure DevOps Pipelines** designed to **run automated tests** as part of your CI/CD workflow. It wraps around the `VSTest.Console.exe` runner, which is Microsoft's official tool for executing unit tests in Visual Studio-based projects.

In a CI/CD environment, `VSTest@3` ensures:

✅ **Automatic discovery of test DLLs** (with a wildcard like `*Tests.dll`)  
✅ **Remote test execution** on build agents (hosted or self-hosted)  
✅ **Generation of detailed test reports** (`.trx` format) that integrate with Azure DevOps' test reporting features  
✅ **Advanced configurations**, such as:
- Running tests in parallel
- Filtering or excluding specific tests
- Collecting code coverage
- Diagnostic logging

---

## 📌 How is `VSTest@3` Different from `dotnet test`?

Here’s a feature-by-feature comparison to help clarify why to use `VSTest@3` in pipelines instead of `dotnet test`:

| **Feature**                     | `dotnet test` (VS Code)         | `VSTest@3` (Azure DevOps)                                     |
|----------------------------------|----------------------------------|----------------------------------------------------------------|
| ✅ **Runs tests?**              | Yes                              | Yes                                                            |
| 🚫 **Runs in CI/CD?**           | No (meant for local dev)         | Yes (meant for pipelines)                                      |
| ✅ **Finds tests automatically?** | Yes                              | No (you must specify `*Tests.dll`)                             |
| 🚫 **Generates test reports?**  | No (by default)                  | Yes (produces `.trx` results for Azure DevOps)                |
| ⚠️ **Can exclude tests?**       | Limited (some options)           | Yes (supports `testFiltercriteria`)                            |
| 🚫 **Supports code coverage?**  | No (by default)                  | Yes (`codeCoverageEnabled: true`)                              |
| ⚠️ **Supports parallel execution?** | Limited                       | Yes                                                            |

---
Certainly! Here's a more **technical and precise rewrite** in **Markdown**, using proper formatting, indentation, and emphasis for clarity and professionalism:

---

## Step 1: 🗂️ Organize Your Repository Structure

To ensure a clean and maintainable codebase that supports testability and CI/CD automation, structure your repository as follows:

### 🔧 Project Layout Guidelines

- **Ensure the Logic App project and the Unit Test project** are in distinct directories.
  
 ```markdown
```plaintext
├── MyLogicApp/                      # Azure Logic App Standard project
│   ├── .vscode/                    
│   ├── Artifacts/                  
│   ├── lib/                        
│   ├── MyWorkflowSample/           # Workflow definitions folder
│   │   └── workflow.json           # JSON definition of the workflow (triggers, actions, etc.)
│   ├── workflow-designtime/       
│   │   ├── host.json               
│   │   └── local.settings.json     
│   ├── .funcignore                 
│   ├── .gitignore                  
│   ├── connections.json            
│   ├── host.json                   
│   ├── local.settings.json         
│   └── parameters.json             
├── Tests/                           # Unit and integration test project
│   ├── MyLogicAppTestProject/       # Main test project folder
│   │   ├── bin/                   # Compiled binary output (autogenerated)
│   │   ├── obj/                   # Intermediate build files (autogenerated)
│   │   ├── MySampleWorkflow/      # Folder grouping tests for a specific workflow
│   │   │   └── MockOutputs/       # Contains mock data for test execution
│   │   │   └── MyWorkflowTest.cs  # C# test class for workflow unit testing
│   ├── TestExecutor.cs            # Shared utility class to execute tests
│   ├── MyLogicAppTestProject.csproj # C# project file for test project
│   └── Tests.sln                  # Visual Studio solution file for test projects
├── deployments/                    # CI/CD deployment folder
│   ├── MyLogicAppDeployment/       # Deployment artifacts for Logic App
│   │   ├── pipelines/              # CI/CD pipeline definitions
│   │   │   └── CI-Pipeline.yml     # Pipeline to build
```
- The unit test project (`MyLogicApp.Tests`) must **reference** the main Logic App project to access workflow definitions and shared components.

### 📁 Add Deployment and Pipeline Structure

Within the root of your repository:

1. **Create a `deployments/` directory** to hold all DevOps-related assets.
2. Inside `deployments/`, create a folder specific to your Logic App (e.g., `MyLogicApp`).
3. Within `MyLogicApp/`, create a subfolder named `pipelines/`.
4. Inside `pipelines/`, create a new YAML file for your CI configuration:
   ```
   deployments/
     └── MyLogicApp/
         └── pipelines/
             └── CI-Pipeline.yml
   ```

> ✅ This structure promotes separation of concerns, simplifies CI configuration management, and scales well for multiple workflows or environments.

---

```markdown
 Creating the Azure DevOps Pipeline (YAML)

This section will guide customers through writing the `azure-pipelines.yml` file.

### 1. Define the Trigger

The trigger section tells Azure DevOps when to run the pipeline.

```yaml
trigger:
  branches:
    include:
      - main        # Run pipeline on push to main branch
      - devFeatureBranch     # Run pipeline on push to feature branch branch
```

### 2. Define the Build Agent

This selects a Microsoft-hosted agent with the latest Windows environment to run your build and tests.

```yaml
pool:
  vmImage: 'windows-latest'  # Use the latest Windows VM for build/test
```

### 3. Set Up .NET SDK

This step installs the required .NET SDK version to ensure your build and tests run in the correct environment.

```yaml
steps:
- task: UseDotNet@2
  displayName: 'Install .NET SDK'
  inputs:
    packageType: 'sdk'
    version: '6.x'  # Adjust this to match your project’s .NET version
```

### 4. Restore Dependencies

Restores all project dependencies, ensuring that all required packages are downloaded before the build.

```yaml
- script: dotnet restore
  displayName: 'Restore NuGet Packages'
  workingDirectory: ./Tests
```

### 5. Build the Project

Compiles the solution in Release mode. The `--no-restore` flag is used since dependencies were already restored in the previous step.

```yaml
- script: dotnet build --configuration Release --no-restore
  displayName: 'Build Solution'
  workingDirectory: ./Tests
```

### 6. Run Unit Tests Using VSTest@3

```yaml
- task: VSTest@3
  displayName: 'Run Unit Tests'
  inputs:
    testSelector: 'testAssemblies'
    testAssemblyVer2: '**\*Tests.dll'  # Run all test assemblies
    searchFolder: '$(Build.SourcesDirectory)/LogicAppProject/tests' #TODO: Update 
    codeCoverageEnabled: true
    platform: '$(BuildPlatform)'
    configuration: '$(BuildConfiguration)'
```

**Explanation:**
- **Test Selection:** Uses a wildcard to pick up all assemblies ending in `Tests.dll`.
- **Search Folder:** Specifies where test assemblies are located.
- **Code Coverage:** Enables code coverage reporting, providing insights into how much code is being tested.

### 7. Publish Test Results

Publishes the test results to Azure DevOps, so test reports can be viewed in the pipeline summary.

```yaml
- task: PublishTestResults@2
  displayName: 'Publish Test Results'
  inputs:
    testResultsFormat: 'VSTest'
    testResultsFiles: '**/TestResults/*.trx'
    mergeTestResults: true
```
```
