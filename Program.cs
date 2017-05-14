﻿using System;
using build.Extensions;

namespace build
{
    class Program
    {
        static void Main(string[] args)
        {
            DeploySdk
                .StartOnlyProceedOnSuccess()
                .Call("npm", "config set prefix \"%HOME%\tools\"")
                .Call("npm", "install -g yarn")
                .Call("nuget", "restore \"%DEPLOYMENT_SOURCE%\\AzureFunctions.sln\"")
                .Call(@"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe", @"%DEPLOYMENT_SOURCE%\AzureFunctions\AzureFunctions.csproj"" /nologo /verbosity:m /t:Build /t:pipelinePreDeployCopyAllFilesToOneFolder /p:_PackageTempDir=""%DEPLOYMENT_TEMP%"";AutoParameterizationWebConfigConnectionStrings=false;Configuration=Release;UseSharedCompilation=false  /p:DeleteExistingFiles=False /p:SolutionDir=""%DEPLOYMENT_SOURCE%\.\\"" %SCM_BUILD_ARGS%")
                .ChangeDirectory(@"%DEPLOYMENT_SOURCE%\AzureFunctions.AngularClient")
                .Call("yarn", "intall", tries: 2)
                .ParallelCall(p => p
                   .Call("node_modules\\.bin\\ng", "build --prod --environment=prod --output-path=\"%ARTIFACTS%\\dist\\opt\"", tries: 2)
                   .Call("node_modules\\.bin\\ng", "build --output-path=\"%ARTIFACTS%\\dist\\notopt\"", tries: 2)
                )
                .UpdateProdFileNames()
                .Copy("%ARTIFACTS%\\dist\\opt", "%DEPLOYMENT_TEMP%")
                .CopySwaggerFiles()
                .Publish()
                .SetupTemplatesWebJob()
                .UpdateBuildTxt()
                .Run();
        }
    }
}
