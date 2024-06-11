[![OpenSSF Scorecard](https://api.scorecard.dev/projects/github.com/Azure/arm-template-parser/badge)](https://scorecard.dev/viewer/?uri=github.com/Azure/arm-template-parser)

# ARM Template Parser

## What is it?

This is a tool that leverages Microsoft libraries to parse ARM templates offline. It fills out the parameters and interprets any statements to produce an array of resources in json format. The use case for the tool is local parsing of templates as part of automation. Specifically for copying policy assignments from upstream modules to modules written in other IaC languages, such as Terraform and Bicep.

## How to install

Download the binary from the [latest release](https://github.com/Azure/arm-template-parser/releases). There are Linux or Windows versions:

* `Template.Parser.Cli`: Linux
* `Template.Parser.Cli.exe`: Windows

Save it somewhere in your path.

## How to use it?

You can find example usage over [here](https://github.com/Azure/terraform-azurerm-caf-enterprise-scale/blob/d678f4caae1d18bda54e93ad674a658eef6ef4a0/.github/scripts/Invoke-LibraryUpdatePolicyAssignmentArchetypes.ps1#L49).

To run the CLI task, you need to supply any default parameters you'd like to populate and point it at your ARM template.

Here is a PowerShell example:

```pwsh
$sourcePath = "./example-template.json"
$parametersSourcePath = "./example-template.param.json"

$parsedTemplate = & Template.Parser.Cli "-s $sourcePath" "-f $eslzArmParametersSourcePath" "-a" | Out-String | ConvertFrom-Json

foreach($resource in $parsedTemplate)
{
   # do somthing...
}
```

## Parameters

* `--sourceTemplate` or `s`: The fully qualified file path for the source ARM template.
* `--parameter` or `-p`: A parameter key value pair, in the format key=value.
* `--parameterFilePath` or `-f`: A parameter file path.
* `--location` or `-l`: The default location value for the template (e.g. uksouth). This defaults to `${default_location}` if no value is supplied.
* `--all` or `-a`: Whether to return all the resources found in the template. By default it will just return the first resource.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
