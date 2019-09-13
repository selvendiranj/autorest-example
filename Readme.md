## ClientApi code generator using autorest and swagger

### Prerequisites

* dotnet cli
* autorest cli ```npm install -g autorest```
* Asp.Net core Rest api project
* swagger integration to the rest api
* A client project (can be consoleApp or webapi/webapp)

**Create Sample Api project**
```csharp
namespace Sample.Api.Controllers
{
    class output
    {
        public string[] values { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FooController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(output), 200)]
        [SwaggerOperation(OperationId = "Foos_GetAll")]
        public ActionResult<IEnumerable<string>> Get()
        {
            var result = new output() { values = new string[] { "foo", "bar" } };
            return new JsonResult(result);
        }
    }
}
```

**Integrate swagger**

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSwaggerGen(c =>
    {
        c.EnableAnnotations();
        c.SwaggerDoc("v1", new Info { Title = "Sample API", Version = "v1" });
    });

    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    //...

    app.UseHttpsRedirection();
    app.UseMvc();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample API");
    });
}
```

**Sample.Api - msbuild target which generates swagger.json**
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

    <PropertyGroup>
        <TargetFramework>netcoreapp2.2</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Microsoft.AspNetCore.App" />
        <PackageReference Include="Swashbuckle.AspNetCore" Version="4.0.1" />
        <PackageReference Include="Swashbuckle.AspNetCore.Annotations" Version="4.0.1" />
    </ItemGroup>

    <ItemGroup>
        <DotNetCliToolReference Include="Swashbuckle.AspNetCore.Cli" Version="4.0.1" />
    </ItemGroup>

    <Target Name="ToSwaggerJson" AfterTargets="PostBuildEvent">
        <!-- set security on binaries-->
        <Exec Command="dotnet swagger tofile --output $(OutputPath)swagger.json $(OutputPath)$(AssemblyName).dll v1"/>
    </Target>
</Project>
```

**Client - msbuild target which generates client code from swagger.json**
```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>netcoreapp2.2</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Microsoft.Rest.ClientRuntime" Version="2.3.20" />
    </ItemGroup>

    <ItemGroup>
        <Folder Include="Generated\" />
    </ItemGroup>

    <Target Name="FromSwaggerJson" BeforeTargets="PreBuildEvent">
        <PropertyGroup>
            <theApiReference Condition="$(theApiReference) == ''">Sample.Api</theApiReference>
        </PropertyGroup>
        <Delete Files="Generated\*.*" />
        <Exec Command="autorest --input-file=..\$(theApiReference)\$(OutputPath)swagger.json ^
                                --output-folder=Generated ^
                                --namespace=$(AssemblyName) ^
                                --csharp"/>
    </Target>

</Project>
```

**After building both projects, use below the simple code to call the APIs like methods**
```csharp
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");

        SampleAPI api = new SampleAPI() { BaseUri = new Uri("http://localhost:5000") };
        IFoos fooApi = new Foos(api);

        var result = fooApi.GetAllAsync().Result;
    }
}
```