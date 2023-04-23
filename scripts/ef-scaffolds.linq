<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

string projectFolder = new DirectoryInfo(Util.CurrentQueryPath).Parent!.Parent!.CreateSubdirectory("GPTCodingAssistant").ToString();
string provider = "Microsoft.EntityFrameworkCore.SqlServer";

Directory.SetCurrentDirectory(projectFolder);

{
	string contextName = "ChatGPTDB";
	string connectionStringName = "ChatGPTSqlServerConnectionString";
	string options = string.Join(" ", new[]
	{
		"--data-annotations",
		"--force",
		$"--context {contextName}",
		"--output-dir DB",
		"--verbose",
	});
	string cmd = $"dotnet ef dbcontext scaffold Name={connectionStringName} {provider} {options}";
	Util.Cmd(cmd.Dump("command text"));
}